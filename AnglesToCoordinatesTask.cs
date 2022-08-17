using System;
using System.Drawing;
using NUnit.Framework;

namespace Manipulation
{
    public static class AnglesToCoordinatesTask
    {
        /// <summary>
        /// По значению углов суставов возвращает массив координат суставов
        /// в порядке new []{elbow, wrist, palmEnd}
        /// </summary>
        public static PointF[] GetJointPositions(double shoulder, double elbow, double wrist)
        {
            var elbowH = elbow + shoulder - Math.PI; // угол между Forearm и осью X
            var wristH = wrist + elbowH - Math.PI; // угол между Palm и осью X

            var elbowPos = GetPointPosition(
                shoulder, Manipulator.UpperArm, new PointF(0,0));
            var wristPos = GetPointPosition(
                elbowH, Manipulator.Forearm, elbowPos);
            var palmEndPos = GetPointPosition(
                wristH, Manipulator.Palm, wristPos);
            return new PointF[]
            {
                elbowPos,
                wristPos,
                palmEndPos
            };
        }

        public static PointF GetPointPosition(double cornerToXAxis,
            float lengthLine, PointF previousPoint)
            => new PointF(
                (float)Math.Cos(cornerToXAxis) * lengthLine + previousPoint.X,
                (float)Math.Sin(cornerToXAxis) * lengthLine + previousPoint.Y);
        
    }

    [TestFixture]
    public class AnglesToCoordinatesTask_Tests
    {
        [TestCase(-0.349, 10f, 0f, 0f, 9.4f, -3.42f)]
        [TestCase(0, 0f, 0f, 0f, 0f, 0f)]
        public void TestGetPointPosition(
            double cornerToXAxis, float lengthLine,
            float previousPointX, float previousPointY, 
            float expectedPointX, float expectedPointY)
        {
            var previousPoint = new PointF(previousPointX, previousPointY);
            var expectedPoint = new PointF(expectedPointX, expectedPointY);
            var actualPoint = AnglesToCoordinatesTask
                .GetPointPosition(cornerToXAxis, lengthLine, previousPoint);
            Assert.AreEqual(expectedPointX, (float)Math.Round(actualPoint.X, 2));
            Assert.AreEqual(expectedPointY, (float)Math.Round(actualPoint.Y, 2));

        }


        // Доработайте эти тесты!
        // С помощью строчки TestCase можно добавлять новые тестовые данные.
        // Аргументы TestCase превратятся в аргументы метода.
        [TestCase(Math.PI / 2, Math.PI / 2, Math.PI, Manipulator.Forearm + Manipulator.Palm, Manipulator.UpperArm)]
        public void TestGetJointPositions(double shoulder, double elbow, double wrist, double palmEndX, double palmEndY)
        {
            var joints = AnglesToCoordinatesTask.GetJointPositions(shoulder, elbow, wrist);
            Assert.AreEqual(palmEndX, joints[2].X, 1e-5, "palm endX");
            Assert.AreEqual(palmEndY, joints[2].Y, 1e-5, "palm endY");
            //Assert.Fail("TODO: проверить, что расстояния между суставами равны длинам сегментов манипулятора!");
        }
    }
}