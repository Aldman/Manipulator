using System;
using System.Drawing;
using NUnit.Framework;

namespace Manipulation
{
    public static class ManipulatorTask
    {
        /// <summary>
        /// Возвращает массив углов (shoulder, elbow, wrist),
        /// необходимых для приведения эффектора манипулятора в точку x и y 
        /// с углом между последним суставом и горизонталью, равному alpha (в радианах)
        /// См. чертеж manipulator.png!
        /// </summary>
        public static double[] MoveManipulatorTo(double x, double y, double alpha)
        {
            var wristPos = AnglesToCoordinatesTask
                .GetPointPosition(Math.PI - alpha, Manipulator.Palm,
                new PointF((float)x, (float)y));
            var shoulderWristLength = GetLengthFromStartCoordinate
                (wristPos.X, wristPos.Y);
            if (!TriangleTask.IsDegenerateTriangle(
                Manipulator.Forearm, Manipulator.UpperArm,
                shoulderWristLength))
            {
                var elbow = TriangleTask.GetABAngle(a: Manipulator.Forearm,
                    b: Manipulator.UpperArm,
                    c: shoulderWristLength);
                var firstPieceOfShoulder = TriangleTask.GetABAngle
                    (a: Manipulator.UpperArm,
                    b: shoulderWristLength,
                    c: Manipulator.Forearm);
                var secondPieceOfShoulder = Math.Atan2(wristPos.Y, wristPos.X);
                var shoulder = firstPieceOfShoulder + secondPieceOfShoulder;
                var wrist = -alpha - shoulder - elbow;
                if (shoulder != double.NaN && elbow != double.NaN && wrist != double.NaN)
                    return new[] { shoulder, elbow, wrist };
            }
            return new[] { double.NaN, double.NaN, double.NaN };
        }

        public static double GetLengthFromStartCoordinate
            (double x, double y)
        => Math.Sqrt(x * x + y * y);
    }



    [TestFixture]
    public class ManipulatorTask_Tests
    {
        [Test]
        public void TestMoveManipulatorTo()
        {
            double expectedTriangle1 = 0;
            double expectedTriangle2 = 0;
            double expectedTriangle3 = 0;
            Random random = new Random();
            for (int i = 0; i < 11; i++)
            {
                //var actualTriangles = ManipulatorTask
                //    .MoveManipulatorTo()
            }
            //Assert.Fail("Write randomized test here!");
        }
    }
}