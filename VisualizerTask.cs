using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Manipulation
{
    public static class VisualizerTask
    {
        public static double X = 220;
        public static double Y = -100;
        public static double Alpha = 0.05;
        public static double Wrist = 2 * Math.PI / 3;
        public static double Elbow = 3 * Math.PI / 4;
        public static double Shoulder = Math.PI / 2;

        public static Brush UnreachableAreaBrush = new SolidBrush(Color.FromArgb(255, 255, 230, 230));
        public static Brush ReachableAreaBrush = new SolidBrush(Color.FromArgb(255, 230, 255, 230));
        public static Pen ManipulatorPen = new Pen(Color.Black, 3);
        public static Brush JointBrush = Brushes.Gray;

        public static void KeyDown(Form form, KeyEventArgs key)
        {
            // TODO: Добавьте реакцию на QAWS и пересчитывать Wrist
            if (IsKeyEqualsQAWS(key))
            {
                switch (key.KeyCode)
                {
                    case Keys.Q:
                        Shoulder += 0.1;
                        break;
                    case Keys.A:
                        Shoulder -= 0.1;
                        break;
                    case Keys.W:
                        Elbow += 0.1;
                        break;
                    case Keys.S:
                        Elbow -= 0.1;
                        break;
                    default: break;
                }
                Wrist = -Alpha - Shoulder - Elbow;
                form.Invalidate();
            }
        }

        private static bool IsKeyEqualsQAWS(KeyEventArgs key)
        => (key.KeyCode == Keys.Q || key.KeyCode == Keys.A
            || key.KeyCode == Keys.W || key.KeyCode == Keys.S);


        public static void MouseMove(Form form, MouseEventArgs e)
        {
            // TODO: Измените X и Y пересчитав координаты (e.X, e.Y) в логические.

            var newMousePoint = ConvertWindowToMath(
                windowPoint: new PointF(e.X, e.Y),
                shoulderPos: GetFormCenterPointF(form));
            X = newMousePoint.X;
            Y = newMousePoint.Y;
            UpdateManipulator();
            form.Invalidate();
        }

        public static PointF GetFormCenterPointF(Form form)
        => new PointF(form.ClientRectangle.Left + (form.ClientRectangle.Width / 2f),
            form.ClientRectangle.Top + (form.ClientRectangle.Height / 2f));


        public static void MouseWheel(Form form, MouseEventArgs e)
        {
            // TODO: Измените Alpha, используя e.Delta — размер прокрутки колеса мыши
            Alpha += e.Delta;
            UpdateManipulator();
            form.Invalidate();
        }

        public static void UpdateManipulator()
        {
            // Вызовите ManipulatorTask.MoveManipulatorTo и обновите значения полей Shoulder, Elbow и Wrist, 
            // если они не NaN. Это понадобится для последней задачи.
            //shoulder, elbow, wrist
            var angles = ManipulatorTask.MoveManipulatorTo(X, Y, Alpha);
            Shoulder = angles[0];
            Elbow = angles[1];
            Wrist = angles[2];
        }

        public static void DrawManipulator(Graphics graphics, PointF shoulderPos)
        {
            var joints = AnglesToCoordinatesTask.GetJointPositions(Shoulder, Elbow, Wrist);

            graphics.DrawString(
                $"X={X:0}, Y={Y:0}, Alpha={Alpha:0.00}",
                new Font(SystemFonts.DefaultFont.FontFamily, 12),
                Brushes.DarkRed,
                10,
                10);
            DrawReachableZone(graphics, ReachableAreaBrush, 
                UnreachableAreaBrush, shoulderPos, joints);
            var graphicalPoints = ConvertMathToWindow(
                mathPoint: new PointF((float)X, (float)Y),
                shoulderPos: new PointF(0, 0));
            var jointCoordinates = AnglesToCoordinatesTask
                .GetJointPositions(Shoulder, Elbow, Wrist); // elbow, wrist, palmEnd
            DrawManipulatorByCoordinates(graphics, jointCoordinates);
        }

        public static void DrawManipulatorByCoordinates(Graphics graphics,
            PointF[] jointCoordinates)
        {
            var elbowXY = jointCoordinates[0];
            var wristXY = jointCoordinates[1];
            var palmEndXY = jointCoordinates[2];
            graphics.DrawLine(ManipulatorPen, new Point(0, 0), elbowXY);
            graphics.DrawLine(ManipulatorPen, elbowXY, wristXY);
            graphics.DrawLine(ManipulatorPen, wristXY, palmEndXY);
            var deltaEllipse = 1;
            graphics.FillEllipse(JointBrush,
                new RectangleF(x: elbowXY.X - deltaEllipse, y: elbowXY.Y - deltaEllipse,
                width: 2 * deltaEllipse, height: 2 * deltaEllipse));
            graphics.FillEllipse(JointBrush,
                new RectangleF(x: wristXY.X - deltaEllipse, y: wristXY.Y - deltaEllipse,
                width: 2 * deltaEllipse, height: 2 * deltaEllipse));
        }

        private static void DrawReachableZone(
            Graphics graphics,
            Brush reachableBrush,
            Brush unreachableBrush,
            PointF shoulderPos,
            PointF[] joints)
        {
            var rmin = Math.Abs(Manipulator.UpperArm - Manipulator.Forearm);
            var rmax = Manipulator.UpperArm + Manipulator.Forearm;
            var mathCenter = new PointF(joints[2].X - joints[1].X, joints[2].Y - joints[1].Y);
            var windowCenter = ConvertMathToWindow(mathCenter, shoulderPos);
            graphics.FillEllipse(reachableBrush, windowCenter.X - rmax, windowCenter.Y - rmax, 2 * rmax, 2 * rmax);
            graphics.FillEllipse(unreachableBrush, windowCenter.X - rmin, windowCenter.Y - rmin, 2 * rmin, 2 * rmin);
        }

        public static PointF GetShoulderPos(Form form)
        {
            return new PointF(form.ClientSize.Width / 2f, form.ClientSize.Height / 2f);
        }

        public static PointF ConvertMathToWindow(PointF mathPoint, PointF shoulderPos)
        {
            return new PointF(mathPoint.X + shoulderPos.X, shoulderPos.Y - mathPoint.Y);
        }

        public static PointF ConvertWindowToMath(PointF windowPoint, PointF shoulderPos)
        {
            return new PointF(windowPoint.X - shoulderPos.X, shoulderPos.Y - windowPoint.Y);
        }
    }
}