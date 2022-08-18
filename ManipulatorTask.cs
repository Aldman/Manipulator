using System;
using NUnit.Framework;

namespace Manipulation
{
    public static class ManipulatorTask
    {
        const float maxLengthManipulator = Manipulator.Forearm
                + Manipulator.Palm + Manipulator.UpperArm;

        /// <summary>
        /// Возвращает массив углов (shoulder, elbow, wrist),
        /// необходимых для приведения эффектора манипулятора в точку x и y 
        /// с углом между последним суставом и горизонталью, равному alpha (в радианах)
        /// См. чертеж manipulator.png!
        /// </summary>
        public static double[] MoveManipulatorTo(double x, double y, double alpha)
        {
            // Используйте поля Forearm, UpperArm, Palm класса Manipulator
            return new[] { double.NaN, double.NaN, double.NaN };
        }

        private static bool CanMove(double xWrist, double yWrist)
        => Math.Sqrt(xWrist * xWrist + yWrist * yWrist) < maxLengthManipulator;
    }

    [TestFixture]
    public class ManipulatorTask_Tests
    {
        [Test]
        public void TestMoveManipulatorTo()
        {
            Assert.Fail("Write randomized test here!");
        }
    }
}