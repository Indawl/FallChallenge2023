﻿using DebugUtils.Objects;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
        public Vector Coord { get; set; }
        public int LightRadius { get; set; }
        public int MaxLightRadius { get; set; }
        public int MonsterRadius { get; set; }
        public int MotorRadius { get; set; }

        public DebugDrone(Drone drone, DebugObject parent) : base(drone.ToString(), parent)
        {
            Drone = drone;
            Type = Drone.Emergency ? DroneType.DAMAGED : (Drone.NewScans.Any() ? DroneType.SCAN : DroneType.NORMAL);
            LightRadius = Drone.Lighting ? GameProperties.LIGHT_SCAN_RADIUS : GameProperties.DARK_SCAN_RADIUS;
            MonsterRadius = LightRadius + GameProperties.MONSTER_DETECTED_RADIUS_ADD;

            LightRadius = parent.Position.Width * LightRadius / GameProperties.MAP_SIZE;
            MaxLightRadius = parent.Position.Width * GameProperties.LIGHT_SCAN_RADIUS / GameProperties.MAP_SIZE;
            MonsterRadius = parent.Position.Width * MonsterRadius / GameProperties.MAP_SIZE;
            MotorRadius = parent.Position.Width * GameProperties.MOTOR_RANGE / GameProperties.MAP_SIZE;

            Coord = new Vector(parent.Position.Width * Drone.Position.X / GameProperties.MAP_SIZE, parent.Position.Height * Drone.Position.Y / GameProperties.MAP_SIZE);

            Position = new Rectangle(0, 0, parent.Position.Width, parent.Position.Height);
            Visible = true;

            Properties.Add("Id", Drone.Id);
            Properties.Add("Player", Drone.PlayerId == 0 ? "Me" : "Enemy");
            Properties.Add("Position", Drone.Position.ToIntString() );
            Properties.Add("Speed", string.Format("{0} {1}", (int)Drone.Speed.Length(), Drone.Speed.ToIntString()));
            Properties.Add("Emergency", Drone.Emergency);
            Properties.Add("Battery", Drone.Battery);
            Properties.Add("Lighting", Drone.Lighting);
            Properties.Add("MotorOn", Drone.MotorOn);
            Properties.Add("Scanning", Drone.NewScans.Any());
            Properties.Add("ScansCount", Drone.Scans.Count);
            Properties.Add("Scans", Drone.Scans.ToArray());
            Properties.Add("NewScans", Drone.NewScans.ToArray());
            Properties.Add("RadarBlips", Drone.RadarBlips.ToArray());
            
        }

        public override Bitmap GetFigure()
        {
            var modelPosition = MODEL_POSITION[Drone.PlayerId][Type];

            var drone = new Bitmap(Position.Width, Position.Height);
            var g = Graphics.FromImage(drone);

            g.FillEllipse(new SolidBrush(Color.FromArgb(50, 255, 255, 255)), (int)Coord.X - LightRadius, (int)Coord.Y - LightRadius, 2 * LightRadius, 2 * LightRadius);
            g.DrawEllipse(new Pen(Color.Red, 2.0f), (int)Coord.X - MonsterRadius, (int)Coord.Y - MonsterRadius, 2 * MonsterRadius, 2 * MonsterRadius);
            g.DrawEllipse(new Pen(Color.Blue, 2.0f), (int)Coord.X - MotorRadius, (int)Coord.Y - MotorRadius, 2 * MotorRadius, 2 * MotorRadius);
            g.DrawEllipse(new Pen(Color.White, 2.0f) { DashPattern = new float[] { 15.0f, 15.0f } }, (int)Coord.X - MaxLightRadius, (int)Coord.Y - MaxLightRadius, 2 * MaxLightRadius, 2 * MaxLightRadius);

            g.DrawImage(DebugRes.Models.Clone(modelPosition, DebugRes.Models.PixelFormat),
                (int)Coord.X - modelPosition.Width / 2, (int)Coord.Y - modelPosition.Height / 3, modelPosition.Width, modelPosition.Height);

            var radiusAttack = Parent.Position.Width * GameProperties.MONSTER_ATTACK_RADIUS / GameProperties.MAP_SIZE; 
            g.DrawEllipse(new Pen(Color.Yellow, 2.0f), (int)Coord.X - radiusAttack, (int)Coord.Y - radiusAttack, 2 * radiusAttack, 2 * radiusAttack);

            return drone;
        }

        public override Rectangle? GetSelectedArea()
        {
            var modelPosition = MODEL_POSITION[Drone.PlayerId][Type];
            return new Rectangle((int)Coord.X - modelPosition.Width / 3, (int)Coord.Y - modelPosition.Height / 3, 2 * modelPosition.Width / 3, 2 * modelPosition.Height / 3);
        }

        public override string ToString() => Drone.ToString();
    }
}
