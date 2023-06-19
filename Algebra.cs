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
        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> expression)
        {
            var body = expression.Body;
            var nodeType = body.NodeType;
            var nodeTypeName = nodeType.ToString();
            var difFuncName = nameof(Differentiate) + nodeTypeName;
            var difFunc = typeof(Algebra).GetMethod(difFuncName, 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (difFunc is null)
                throw new ArgumentNullException(difFuncName + "not supported");
            var result = difFunc.Invoke(null, new object[] {expression});
            return (Expression<Func<double, double>>)result;
        }

        static Expression<Func<double, double>> DifferentiateConstant(Expression<Func<double, double>> expression)
        {
            Expression<Func<double, double>> result = x => 0;
            return result;
        }

        static Expression<Func<double, double>> DifferentiateParameter(Expression<Func<double, double>> expression)
        {
            Expression<Func<double, double>> result = x => 1;
            return result;
        }
    }
}
