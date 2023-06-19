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
            Expression<Func<double, double>> result = null;
            if (nodeType == ExpressionType.Constant)
                result = x => 0;
            return result;
        }
    }
}
