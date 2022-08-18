using System;
using NUnit.Framework;

namespace Manipulation
{
    public class TriangleTask
    {
        /// <summary>
        /// Возвращает угол (в радианах) между сторонами a и b в треугольнике со сторонами a, b, c 
        /// </summary>
        public static double GetABAngle(double a, double b, double c)
        {
            if (c == 0 && (a != 0 || b != 0)) return 0;
            if (!IsDegenerateTriangle(a, b, c))
            {
                return Math.Acos((a * a + b * b - c * c)
                                / (2 * a * b));
            }
            return double.NaN;
        }

        public static bool IsDegenerateTriangle(double a, double b, double c)
            => !((a + b > c) && (a + c > b) && (b + c > a));
    }

    [TestFixture]
    public class TriangleTask_Tests
    {
        [TestCase(1, 2, 3, true)]
        [TestCase(0, 2, 3, true)]
        [TestCase(2, 2, 3, false)]
        public void IsDegenerateTriangleTest(
            double a, double b, double c, bool expectedValue)
        {
            var actualResult = TriangleTask.IsDegenerateTriangle(a, b, c);
            Assert.AreEqual(expectedValue, actualResult);
        }

        [TestCase(3, 4, 5, Math.PI / 2)]
        [TestCase(1, 1, 1, Math.PI / 3)]
        [TestCase(0, 4, 5, double.NaN)]
        [TestCase(1, 1, 0, 0)]
        // добавьте ещё тестовых случаев!
        public void TestGetABAngle(double a, double b, double c, double expectedAngle)
        {
            var actualResult = TriangleTask.GetABAngle(a, b, c);
            Assert.AreEqual(expectedAngle, actualResult, 10e-6);
        }
    }
}