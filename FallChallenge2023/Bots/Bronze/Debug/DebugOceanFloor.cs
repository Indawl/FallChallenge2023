using DebugUtils.Objects;
using DebugUtils.Objects.Maps;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugOceanFloor : DebugMapXY
    {
        public DebugOceanFloor(DebugObject parent) : base("Ocean Floor", GameState.MAP_SIZE, GameState.MAP_SIZE, parent)
        {
            Position = new Rectangle(460, 40, 1000, 1000);
            Visible = true;
        }

        public override Bitmap GetFigure()
        {
            var area = new Bitmap(Position.Width, Position.Height);
            var g = Graphics.FromImage(area);

            g.DrawRectangle(new Pen(Color.White, 4.0f), 0, 0, Position.Width, Position.Height);
            g.FillRectangle(new SolidBrush(Color.FromArgb(128, 128, 128, 128)), 0, 0, Position.Width, Position.Height);

            g.DrawLine(new Pen(Color.White, 4.0f) { DashPattern = new float[] { 15.0f, 5.0f } }, 0,  50, Position.Width,  50);
            g.DrawLine(new Pen(Color.Green, 4.0f), 0, 250, Position.Width, 250);
            g.DrawLine(new Pen(Color.Yellow, 4.0f), 0, 500, Position.Width, 500);
            g.DrawLine(new Pen(Color.Red, 4.0f), 0, 750, Position.Width, 750);

            return area;
        }
    }
}
