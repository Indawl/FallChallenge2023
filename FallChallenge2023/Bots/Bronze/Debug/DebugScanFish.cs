using DebugUtils.Objects;
using System.Collections.Generic;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugScanFish : DebugObject
    {
        public const int MY_OFFSET = 88;
        public const int ENEMY_OFFSET_Y = 4;

        public static Dictionary<FishColor, Dictionary<FishType, Rectangle>> MODEL_POSITION = new Dictionary<FishColor, Dictionary<FishType, Rectangle>>()
        {
            { FishColor.PURPLE, new Dictionary<FishType, Rectangle>() {
                { FishType.JELLY, new Rectangle(7, 7, 70, 70) },
                { FishType.FISH, new Rectangle(90, 7, 70, 70) },
                { FishType.CRAB, new Rectangle(170, 7, 70, 70) } } },
            { FishColor.YELLOW, new Dictionary<FishType, Rectangle>() {
                { FishType.JELLY, new Rectangle(7, 85, 70, 70) },
                { FishType.FISH, new Rectangle(90, 85, 70, 70) },
                { FishType.CRAB, new Rectangle(170, 85, 70, 70) } } },
            { FishColor.GREEN, new Dictionary<FishType, Rectangle>() {
                { FishType.JELLY, new Rectangle(7, 162, 70, 70) },
                { FishType.FISH, new Rectangle(90, 162, 70, 70) },
                { FishType.CRAB, new Rectangle(170, 162, 70, 70) } } },
            { FishColor.BLUE, new Dictionary<FishType, Rectangle>() {
                { FishType.JELLY, new Rectangle(7, 238, 70, 70) },
                { FishType.FISH, new Rectangle(90, 238, 70, 70) },
                { FishType.CRAB, new Rectangle(170, 238, 70, 70) },
                { FishType.ONE_COLOR, new Rectangle(5, 238, 70, 70) } } }
        };

        public Fish Fish { get; set; }
        public bool Saved { get; set; }
        public bool NoFish { get; set; }

        public DebugScanFish(int playerId, Fish fish, bool saved, bool noFish, DebugObject parent) :
            base(string.Format("{0}{1}{2}", saved ? "SAVED " : string.Empty, fish.Status == FishStatus.LOSTED ? "LOST " : string.Empty, fish), parent)
        {
            Fish = fish;
            Saved = saved;
            NoFish = noFish;

            var pos = MODEL_POSITION[Fish.Color][Fish.Type];

            Position = new Rectangle(pos.X + (playerId == 0 ? MY_OFFSET : 0), pos.Y + (playerId == 0 ? 0 : ENEMY_OFFSET_Y), pos.Width, pos.Height);
            Visible = true;
        }
        public override Bitmap GetFigure()
        {
            var modelPosition = DebugFish.MODEL_POSITION[Fish.Color][Fish.Type];

            var fish = new Bitmap(Position.Width, Position.Height);
            var g = Graphics.FromImage(fish);

            if (!NoFish) g.DrawImage(DebugRes.Models.Clone(modelPosition, DebugRes.Models.PixelFormat), 0, 0, Position.Width, Position.Height);
            if (!Saved) g.FillRectangle(new SolidBrush(Color.FromArgb(180, 150, 150, 150)), 0, 0, Position.Width, Position.Height);
            if (Fish.Status == FishStatus.LOSTED) g.DrawImage(DebugRes.Models.Clone(new Rectangle(197, 108, 77, 77), DebugRes.Models.PixelFormat), 0, 0, Position.Width, Position.Height);

            return fish;
        }

        public override string ToString() => Name;
    }
}
