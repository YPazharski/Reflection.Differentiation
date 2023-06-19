using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reflection.Differentiation
{

    public static class Algebra
    {
        /// <summary>
        /// Add new private static method in Algebra class named "Differentiate" + ((operation name) or (Method name))
        /// to support new operations.
        /// </summary>
        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> expression)
        {
            if (expression == null) throw new ArgumentNullException();
            var difFuncName = GetDifFuncName(expression);
            var difFunc = typeof(Algebra).GetMethod
                (
                    difFuncName, 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static 
                );
            if (difFunc is null)
                throw new ArgumentNullException(difFuncName + " not supported");
            var result = difFunc.Invoke(null, new object[] {expression});
            return (Expression<Func<double, double>>)result;
        }

        static string GetDifFuncName(Expression<Func<double, double>> expression)
        {
            var body = expression.Body;
            var nodeType = body.NodeType;
            var nodeTypeName = nodeType.ToString();
            var result = nameof(Differentiate) + nodeTypeName;
            return result;
        }

        static Expression<Func<double, double>> DifferentiateConstant(Expression<Func<double, double>> expression)
        {
            //der(y = Constant) = (y = 0)
            Expression<Func<double, double>> result = x => 0;
            return result;
        }

        static Expression<Func<double, double>> DifferentiateParameter(Expression<Func<double, double>> expression)
        {
            // der(y = x) = (y = 1) 
            Expression<Func<double, double>> result = x => 1;
            return result;
        }

        static Expression<Func<double, double>> DifferentiateMultiply(Expression<Func<double, double>> expression)
        {
            //der(u * v) = der(u) * v + u * der(v)
            var body = expression.Body as BinaryExpression;
            var u = Expression.Lambda<Func<double, double>>(body.Left, expression.Parameters);
            var v = Expression.Lambda<Func<double, double>>(body.Right, expression.Parameters);
            var derU = Differentiate(u);
            var derV = Differentiate(v);
            var leftPart = Expression.Multiply(derU.Body, v.Body);
            var rightPart = Expression.Multiply(u.Body, derV.Body);
            var sum = Expression.Add(leftPart, rightPart);
            var result = Expression.Lambda<Func<double, double>>(sum, expression.Parameters);
            return result;
        }
    }
}
