using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Reflection.Differentiation
{
    public static class AlgebraAlt
    {
        private static Dictionary<ExpressionType, Func<Expression, Expression>> diffFuncs;

        private static void InitializeExprDiffFuncs()
        {
            diffFuncs = new Dictionary<ExpressionType, Func<Expression, Expression>>
            {
                [ExpressionType.Multiply] = (expr) =>
                {
                    var e = (BinaryExpression)expr;
                    if (e.Left is ConstantExpression || e.Right is ConstantExpression)
                    {
                        var constExpr = (e.Left is ConstantExpression) ? e.Left : e.Right;
                        var otherExpr = (e.Left is ConstantExpression) ? e.Right : e.Left;
                        return Expression.Multiply((ConstantExpression)constExpr, diffFuncs[otherExpr.NodeType](otherExpr));
                    }
                    else
                    {
                        return Expression.Add(
                            Expression.Multiply(e.Left, diffFuncs[e.Right.NodeType](e.Right)),
                            Expression.Multiply(e.Right, diffFuncs[e.Left.NodeType](e.Left))
                            );
                    }
                },

                [ExpressionType.Add] = (expr) =>
                {
                    var e = (BinaryExpression)expr;
                    return Expression.Add(diffFuncs[e.Left.NodeType](e.Left), diffFuncs[e.Right.NodeType](e.Right));
                },

                [ExpressionType.Call] = (expr) =>
                {
                    var e = (MethodCallExpression)expr;
                    if (e.Method.Name == "Sin")
                    {
                        return Expression.Multiply(
                            Expression.Call(null, typeof(Math).GetMethod("Cos", new[] { typeof(double) }), e.Arguments[0]),
                            diffFuncs[e.Arguments[0].NodeType](e.Arguments[0])
                            );
                    }
                    if (e.Method.Name == "Cos")
                    {
                        return Expression.Multiply(
                            Expression.Multiply(
                                Expression.Call(null, typeof(Math).GetMethod("Sin", new[] { typeof(double) }), e.Arguments[0]),
                                Expression.Constant((double)-1)
                                ),
                            diffFuncs[e.Arguments[0].NodeType](e.Arguments[0])
                            );
                    }
                    throw new ArgumentException(expr.ToString() + "have unexpexted call");
                },

                [ExpressionType.Constant] = (expr) => Expression.Constant((double)0),
                [ExpressionType.Parameter] = (expr) => Expression.Constant((double)1)
            };
        }
        static AlgebraAlt()
        {
            InitializeExprDiffFuncs();
        }

        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> funcExpr)
        {
            var expr = diffFuncs[funcExpr.Body.NodeType](funcExpr.Body);
            var lambda = Expression.Lambda<Func<double, double>>(expr, funcExpr.Parameters);
            return lambda;
        }
    }
}
