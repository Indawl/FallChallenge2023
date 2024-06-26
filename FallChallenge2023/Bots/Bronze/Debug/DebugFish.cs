﻿using DebugUtils.Objects;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugFish : DebugObject
    {
        public static Dictionary<FishColor, Dictionary<FishType, Rectangle>> MODEL_POSITION = new Dictionary<FishColor, Dictionary<FishType, Rectangle>>()
        {
            { FishColor.UGLY, new Dictionary<FishType, Rectangle>() {
                { FishType.ANGLER, new Rectangle(422, 5, 86, 71) } } },
            { FishColor.PINK, new Dictionary<FishType, Rectangle>() {
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
        public Vector Coord { get; set; }
        public Vector Speed { get; set; }
        public Rectangle Location { get; set; }

        public DebugFish(Fish fish, DebugObject parent) : base(fish.ToString(), parent)
        {
            Fish = fish;

            var size = MODEL_POSITION[Fish.Color][Fish.Type].Size;

            Coord = new Vector(parent.Position.Width * Fish.Position.X / GameProperties.MAP_SIZE, parent.Position.Height * Fish.Position.Y / GameProperties.MAP_SIZE);
            if (fish.Speed == null) Speed = new Vector();
            else Speed = new Vector(parent.Position.Width * Fish.Speed.X / GameProperties.MAP_SIZE, parent.Position.Height * Fish.Speed.Y / GameProperties.MAP_SIZE);

            Position = new Rectangle((int)Coord.X - size.Width / 2, (int)Coord.Y - size.Height / 2, size.Width, size.Height);
            Location = new Rectangle((int)(parent.Position.Width * Fish.Location.From.X / GameProperties.MAP_SIZE),
                                     (int)(parent.Position.Height * Fish.Location.From.Y / GameProperties.MAP_SIZE),
                                     (int)(parent.Position.Width * Fish.Location.To.X / GameProperties.MAP_SIZE),
                                     (int)(parent.Position.Height * Fish.Location.To.Y / GameProperties.MAP_SIZE));
            Location = new Rectangle(Location.X, Location.Y, Location.Width - Location.X, Location.Height - Location.Y);
            Visible = true;

            Properties.Add("Id", Fish.Id);
            Properties.Add("Color", Fish.Color);
            Properties.Add("Type", Fish.Type);
            Properties.Add("Position", Fish.Position?.ToIntString() );
            if (Fish.Speed != null) Properties.Add("Speed", string.Format("{0} ({1})", Fish.Speed.ToIntString(), (int)Fish.Speed.Length()));
        }

        public override Bitmap GetFigure()
        {
            var modelPosition = MODEL_POSITION[Fish.Color][Fish.Type];

            var fish = new Bitmap(Position.Width, Position.Height);
            var g = Graphics.FromImage(fish);

            g.FillRectangle(new HatchBrush(HatchStyle.Cross, Color.FromArgb(50, 50, 50, 255), Color.FromArgb(50, 200, 200, 200)), Location);

            g.DrawImage(DebugRes.Models.Clone(modelPosition, DebugRes.Models.PixelFormat),
                Position.Width / 2 - modelPosition.Width / 2, Position.Height / 2 - modelPosition.Height / 2, modelPosition.Width, modelPosition.Height);

            g.FillEllipse(new SolidBrush(Color.Black), Position.Width / 2 - 5, Position.Height / 2 - 5, 10, 10);
            g.DrawLine(new Pen(Color.Black, 2.0f), Position.Width / 2, Position.Height / 2, Position.Width / 2 + (int)Speed.X, Position.Height / 2 + (int)Speed.Y);

            return fish;
        }

        public override Rectangle? GetSelectedArea() => new Rectangle(0, 0, Position.Width, Position.Height);

        public override string ToString() => Name;
    }
}
