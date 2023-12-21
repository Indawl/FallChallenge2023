using DebugUtils.Objects.Maps;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugOcean : DebugMap
    {
        public DebugOcean() : base("Ocean")
        {
            Position = new Rectangle(0, 0, (int)GameState.MAP_SIZE.X, (int)GameState.MAP_SIZE.Y);
            Visible = true;
        }

        public override Bitmap GetFigure()
        {
            return base.GetFigure();
        }
    }
}
