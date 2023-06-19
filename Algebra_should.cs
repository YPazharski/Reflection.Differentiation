using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace Reflection.Differentiation
{
    [TestFixture]
    public class Algebra_should
    {
        void AssertDerivativeEqualToNumericDerivative(Expression<Func<double, double>> function)
        {
            var f = function.Compile();
            double eps = 1e-7;
            var dfExpression = Algebra.Differentiate(function);
            var df = dfExpression.Compile();
            for (double x = 0; x < 5; x += 0.1)
            {
                Assert.AreEqual((f(x + eps) - f(x)) / eps, df(x), 1e-5, $"Error on function {function.Body}");
            }
        }

        [Test, Order(1)]
        public void DifferentiateConstant()
        {
            AssertDerivativeEqualToNumericDerivative(z => 42);
        }

        [Test, Order(2)]
        public void DifferentiateParameter()
        {
            AssertDerivativeEqualToNumericDerivative(z => z);
        }

        [Test, Order(3)]
        public void DifferentiateLinearFunction()
        {
            AssertDerivativeEqualToNumericDerivative(z => z * 5);
        }

        //[Test, Order(4)]
        //public void DifferentiateQuadraticFunction()
        //{
        //    AssertDerivativeEqualToNumericDerivative(z => z * 5 * z);
        //}

        //[Test, Order(5)]
        //public void DifferentiateSum()
        //{
        //    AssertDerivativeEqualToNumericDerivative(z => z + z);
        //}

        //[Test, Order(6)]
        //public void DifferentiateSumAndProduct()
        //{
        //    AssertDerivativeEqualToNumericDerivative(z => 5 * z + z * z);
        //}

        //[Test, Order(7)]
        //public void DifferentiateSin1()
        //{
        //    AssertDerivativeEqualToNumericDerivative(z => Math.Sin(z));
        //}

        //[Test, Order(8)]
        //public void DifferentiateSin2()
        //{
        //    AssertDerivativeEqualToNumericDerivative(z => Math.Sin(z * z + z));
        //}

        //[Test, Order(9)]
        //public void DifferentiateCos1()
        //{
        //    AssertDerivativeEqualToNumericDerivative(z => Math.Cos(z));
        //}

        //[Test, Order(10)]
        //public void DifferentiateCos2()
        //{
        //    AssertDerivativeEqualToNumericDerivative(z => Math.Cos(z * z + z));
        //}

        //[Test, Order(11)]
        //public void DifferentiateComplexExpression()
        //{
        //    AssertDerivativeEqualToNumericDerivative(z => Math.Cos(2 * z + z) + 2 * Math.Sin(3 * z + z) + Math.Sin(z + 1) * Math.Cos(z + 2) * 3);
        //}

        //[Test, Order(12)]
        //public void InformativeMessage_OnNotSupportedSyntax()
        //{
        //    var ex = Assert.Throws<ArgumentException>(() => Algebra.Differentiate(z => z.ToString().Length));
        //    Assert.That(ex.Message, Does.Contain("ToString"));
        //}

        //[Test, Order(13)]
        //public void InformativeMessage_OnUnknownFunction()
        //{
        //    var ex = Assert.Throws<ArgumentException>(() => Algebra.Differentiate(z => Math.Max(z, 2*z)));
        //    Assert.That(ex.Message, Does.Contain("Max"));
        //}
    }
}
