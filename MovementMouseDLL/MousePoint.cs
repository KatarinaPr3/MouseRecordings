using System.Drawing;

namespace MouseMovementsDLL
{
    public enum MovementType
    {
        RandomMovement,
        ButtonClick
    }
    public struct MousePoint
    {
        public Point coordinate;
        public long elapsedMS;
    }

}
