using DebugUtils.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugDrone : DebugObject
    {
        public enum DroneType { NORMAL, DAMAGED, SCAN }

        public static Dictionary<DroneType, Rectangle>[] MODEL_POSITION = new Dictionary<DroneType, Rectangle>[]
        { new Dictionary<DroneType, Rectangle>() {
            { DroneType.NORMAL, new Rectangle(141, 312, 139, 108) },
            { DroneType.DAMAGED, new Rectangle(0, 312, 140, 108) },
            { DroneType.SCAN, new Rectangle(197, 186, 139, 107) } },
          new Dictionary<DroneType, Rectangle>() {
            { DroneType.NORMAL, new Rectangle(0, 94, 139, 108) },
            { DroneType.DAMAGED, new Rectangle(0, 203, 140, 108) },
            { DroneType.SCAN, new Rectangle(197, 0, 139, 107) } }
        };

        public Drone Drone { get; set; }
        public DroneType Type { get; set; }
        public int LightRadius { get; set; }
        public int MonsterRadius { get; set; }
        public int MotorRadius { get; set; }

        public DebugDrone(string name, Drone drone, DebugObject parent = null) : base(name, parent)
        {
            Drone = drone;
            Type = Drone.Emergency ? DroneType.DAMAGED : (Drone.Scanning ? DroneType.SCAN : DroneType.NORMAL);
            LightRadius = Drone.Lighting ? Drone.LIGHT_SCAN_RADIUS : Drone.SCAN_RADIUS;
            MonsterRadius = LightRadius + Drone.MONSTER_DETECTED_RADIUS_ADD;

            LightRadius = parent.Position.Width * LightRadius / GameState.MAP_SIZE;            
            MonsterRadius = parent.Position.Width * MonsterRadius / GameState.MAP_SIZE;
            MotorRadius = parent.Position.Width * Drone.MOTOR_RANGE / GameState.MAP_SIZE;

            var x = (int)(parent.Position.Width * Drone.Position.X / GameState.MAP_SIZE);
            var y = (int)(parent.Position.Height * Drone.Position.Y / GameState.MAP_SIZE);

            var radius = Math.Max(MonsterRadius, MotorRadius);

            Position = new Rectangle(x - radius, y - radius, 2 * radius, 2 * radius);
            Visible = true;

            Properties.Add("Id", Drone.Id);
            Properties.Add("Player", Drone.PlayerId == 0 ? "Me" : "Enemy");
            Properties.Add("Position", Drone.Position.ToIntString());
            Properties.Add("Emergency", Drone.Emergency);
            Properties.Add("Battery", Drone.Battery);
            Properties.Add("ScansCount", Drone.Scans.Count);
            Properties.Add("Scans", Drone.Scans.ToArray());
            Properties.Add("RadarBlips", Drone.RadarBlips.ToArray());
            Properties.Add("Lighting", Drone.Lighting);
            Properties.Add("Scanning", Drone.Scanning);
        }

        public override Bitmap GetFigure()
        {
            var modelPosition = MODEL_POSITION[Drone.PlayerId][Type];

            var drone = new Bitmap(Position.Width, Position.Height);
            var g = Graphics.FromImage(drone);

            g.FillEllipse(new SolidBrush(Color.FromArgb(50, 255, 255, 255)), Position.Width / 2 - LightRadius, Position.Height / 2 - LightRadius, 2 * LightRadius, 2 * LightRadius);
            g.DrawEllipse(new Pen(Color.Red, 2.0f), Position.Width / 2 - MonsterRadius, Position.Height / 2 - MonsterRadius, 2 * MonsterRadius, 2 * MonsterRadius);
            g.DrawEllipse(new Pen(Color.Blue, 2.0f), Position.Width / 2 - MotorRadius, Position.Height / 2 - MotorRadius, 2 * MotorRadius, 2 * MotorRadius);

            g.DrawImage(DebugRes.Models.Clone(modelPosition, DebugRes.Models.PixelFormat),
                Position.Width / 2 - modelPosition.Width / 2, Position.Height / 2 - modelPosition.Height / 3, modelPosition.Width, modelPosition.Height);

            g.FillEllipse(new SolidBrush(Color.Black), Position.Width / 2 - 5, Position.Height / 2 - 5, 10, 10);

            return drone;
        }

        public override Bitmap GetSelectedArea()
        {
            var modelPosition = MODEL_POSITION[Drone.PlayerId][Type];

            var drone = new Bitmap(Position.Width, Position.Height);
            var g = Graphics.FromImage(drone);

            g.DrawImage(DebugRes.Models.Clone(modelPosition, DebugRes.Models.PixelFormat),
                Position.Width / 2 - modelPosition.Width / 2, Position.Height / 2 - modelPosition.Height / 3, modelPosition.Width, modelPosition.Height);

            return drone;
        }

        public override string ToString() => Drone.ToString();
    }
}
