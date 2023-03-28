using System.Drawing;

namespace MouseMovementsDLL
{
    public class MousePathTransformer
    {
        public static Point RotatePoint(Point point, Point pivot, double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            double translatedX = point.X - pivot.X;
            double translatedY = point.Y - pivot.Y;

            double rotatedX = translatedX * cos - translatedY * sin;
            double rotatedY = translatedX * sin + translatedY * cos;

            return new Point((int)rotatedX + (int)pivot.X, (int)rotatedY + (int)pivot.Y);
        }
    }
}
