using DebugUtils.Objects.Maps;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugOceanFloor : DebugMap, IDebugMouseMove
    {
        public Point MousePosition { get; set; }

        public DebugOceanFloor() : base("Ocean Floor")
        {
            Position = new Rectangle(0, 0, (int)GameState.MAP_SIZE.X, (int)GameState.MAP_SIZE.Y);
            Visible = true;
        }

        public override string ToString() => string.Format("({0}, {1})", MousePosition.X, MousePosition.Y);
    }
}
