using DebugUtils.Objects;
using System.Collections.Generic;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugFish : DebugObject
    {
        public static Dictionary<FishColor, Dictionary<FishType, Rectangle>> MODEL_POSITION = new Dictionary<FishColor, Dictionary<FishType, Rectangle>>()
        {
            { FishColor.UGLY, new Dictionary<FishType, Rectangle>() {
                { FishType.ANGLER, new Rectangle(422, 5, 86, 71) } } },
            { FishColor.PURPLE, new Dictionary<FishType, Rectangle>() {
                { FishType.JELLY, new Rectangle(281, 312, 49, 45) },
                { FishType.FISH, new Rectangle(141, 203, 50, 42) },
                { FishType.CRAB, new Rectangle(140, 0, 56, 43) } } },
            { FishColor.YELLOW, new Dictionary<FishType, Rectangle>() {
                { FishType.JELLY, new Rectangle(281, 358, 43, 61) },
                { FishType.FISH, new Rectangle(140, 91, 56, 44) },
                { FishType.CRAB, new Rectangle(0, 47, 54, 46) } } },
            { FishColor.GREEN, new Dictionary<FishType, Rectangle>() {
                { FishType.JELLY, new Rectangle(0, 0, 50, 46) },
                { FishType.FISH, new Rectangle(141, 246, 37, 47) },
                { FishType.CRAB, new Rectangle(337, 0, 72, 36) } } },
            { FishColor.BLUE, new Dictionary<FishType, Rectangle>() {
                { FishType.JELLY, new Rectangle(140, 44, 52, 46) },
                { FishType.FISH, new Rectangle(140, 136, 52, 36) },
                { FishType.CRAB, new Rectangle(275, 108, 60, 54) } } }
        };

        public Fish Fish { get; set; }
        public Point Speed { get; set; }

        public DebugFish(string name, Fish fish, DebugObject parent = null) : base(name, parent)
        {
            Fish = fish;

            var size = MODEL_POSITION[Fish.Color][Fish.Type].Size;
            var x = (int)(parent.Position.Width * Fish.Position.X / GameState.MAP_SIZE);
            var y = (int)(parent.Position.Height * Fish.Position.Y / GameState.MAP_SIZE);

            var vX = (int)(parent.Position.Width * Fish.Speed.X / GameState.MAP_SIZE);
            var vY = (int)(parent.Position.Height * Fish.Speed.Y / GameState.MAP_SIZE);
            Speed = new Point(vX, vY);

            Position = new Rectangle(x - size.Width / 2, y - size.Height / 2, size.Width, size.Height);
            Visible = true;

            Properties.Add("Id", Fish.Id);
            Properties.Add("Color", Fish.Color);
            Properties.Add("Type", Fish.Type);
            Properties.Add("Position", Fish.Position.ToIntString());
            Properties.Add("Speed", Fish.Speed.ToIntString());
            Properties.Add("Lost", Fish.Lost);
        }

        public override Bitmap GetFigure()
        {
            var modelPosition = MODEL_POSITION[Fish.Color][Fish.Type];

            var fish = new Bitmap(Position.Width, Position.Height);
            var g = Graphics.FromImage(fish);

            g.DrawImage(DebugRes.Models.Clone(modelPosition, DebugRes.Models.PixelFormat),
                Position.Width / 2 - modelPosition.Width / 2, Position.Height / 2 - modelPosition.Height / 2, modelPosition.Width, modelPosition.Height);

            g.FillEllipse(new SolidBrush(Color.Black), Position.Width / 2 - 5, Position.Height / 2 - 5, 10, 10);
            g.DrawLine(new Pen(Color.Black, 2.0f), Position.Width / 2, Position.Height / 2, Position.Width / 2 + Speed.X, Position.Height / 2 + Speed.Y);

            return fish;
        }

        public override Bitmap GetSelectedArea() => GetFigure();

        public override string ToString() => Fish.ToString();
    }
}
