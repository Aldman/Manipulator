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
            var wristPos = PointD.GetNextPointPosition
                (Math.PI - alpha, Manipulator.Palm,
                new PointD(x, y));
            var shoulderWristLength = GetLengthFromStartCoordinate
                (wristPos.X, wristPos.Y);
            if (!IsDegenerateTriangle(Manipulator.Forearm,
                Manipulator.UpperArm, shoulderWristLength))
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
                if (!AreAnglesNaN(shoulder, elbow, wrist))
                    return new[] { shoulder, elbow, wrist };
            }
            return new[] { double.NaN, double.NaN, double.NaN };
        }

        public static bool IsDegenerateTriangle(double a, double b, double c)
            => !((a + b > c) && (a + c > b) && (b + c > a));

        public static bool AreAnglesNaN(
            double shoulder, double elbow, double wrist)
            => (double.IsNaN(shoulder) || double.IsNaN(elbow ) || double.IsNaN(wrist));

        public static double GetLengthFromStartCoordinate
            (double x, double y)
        => Math.Sqrt(x * x + y * y);
    }

    public struct PointD
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public PointD(double x, double y)
        {
            X = x; Y = y;
        }

        public static PointD GetNextPointPosition(double cornerToXAxis,
            float lengthLine, PointD previousPoint)
            => new PointD(
                Math.Cos(cornerToXAxis) * lengthLine + previousPoint.X,
                Math.Sin(cornerToXAxis) * lengthLine + previousPoint.Y);
    }

    [TestFixture]
    public class ManipulatorTask_Tests
    {
        [Test]
        public void TestMoveManipulatorTo()
        {
            Random random = new Random();
            for (int i = 0; i < 11; i++)
            {
                var x = random.Next(-300, 301);
                var y = random.Next(-300, 301);
                var alpha = random.Next(-1, 2)
                    * random.NextDouble()
                    * 2 * Math.PI;
                var resultAngles = ManipulatorTask
                    .MoveManipulatorTo(x, y, alpha);
                var resultPositions = AnglesToCoordinatesTask
                    .GetJointPositions(shoulder: resultAngles[0],
                    elbow: resultAngles[1], wrist: resultAngles[2]);
                var palmEndPos = resultPositions[resultPositions.Length - 1];
                if (!ManipulatorTask.AreAnglesNaN
                    (resultAngles[0], resultAngles[1], resultAngles[2]))
                {
                    Assert.AreEqual(x,palmEndPos.X, 10e-4);
                    Assert.AreEqual(y, palmEndPos.Y, 10e-4);
                }
                else Assert.AreEqual(true, ManipulatorTask.AreAnglesNaN
                    (resultAngles[0], resultAngles[1], resultAngles[2]));
            }
        }
    }
}