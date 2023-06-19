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
            var operationName = nodeType.ToString();
            if (operationName == "Call")
            {
                var call = body as MethodCallExpression;
                operationName = call.Method.Name;
            }
            var result = nameof(Differentiate) + operationName;
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
            if (body is null)
                throw new ArgumentException();
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

        static Expression<Func<double, double>> DifferentiateAdd(Expression<Func<double, double>> expression)
        {
            //der(u + v) = der(u) + der(v)
            var body = expression.Body as BinaryExpression;
            if (body is null)
                throw new ArgumentException();
            var u = Expression.Lambda<Func<double, double>>(body.Left, expression.Parameters);
            var v = Expression.Lambda<Func<double, double>>(body.Right, expression.Parameters);
            var derU = Differentiate(u);
            var derV = Differentiate(v);
            var sum = Expression.Add(derU.Body, derV.Body);
            var result = Expression.Lambda<Func<double, double>>(sum, expression.Parameters);
            return result;
        }

        static Expression<Func<double, double>> DifferentiateSin(Expression<Func<double, double>> expression)
        {
            //der(y = sin(x)) = (y = cos(x))
            //der(y = g(f(x))) = der(y = g(f)) * der(y = f(x)) 
            var body = expression.Body as MethodCallExpression;
            if (body is null)
                throw new ArgumentException();
            var sinParam = body.Arguments[0];
            var sinParamLambda = Expression.Lambda<Func<double, double>>(sinParam, expression.Parameters);
            var sinParamDerivative = Differentiate(sinParamLambda);
            var cosMethodInfo = typeof(Math).GetMethod("Cos", new Type[] { typeof(double) });
            var cosCall = Expression.Call(cosMethodInfo, sinParam);
            var resultBody = Expression.Multiply(cosCall, sinParamDerivative.Body);
            var result = Expression.Lambda<Func<double, double>>(resultBody, expression.Parameters);
            return result;
        }
    }
}
