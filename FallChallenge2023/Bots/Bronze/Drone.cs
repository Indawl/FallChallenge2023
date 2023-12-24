using FallChallenge2023.Bots.Bronze.GameMath;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class Drone : ICloneable
    {
        public const int MAX_SPEED = 600;
        public const int SINK_SPEED = 300;
        public const int EMERGENCY_SPEED = 300;
        public const int SCAN_RADIUS = 800;
        public const int LIGHT_SCAN_RADIUS = 2000;
        public const int MONSTER_DETECTED_RADIUS_ADD = 300;
        public const int BATTERY_DRAIN = 5;
        public const int BATTERY_RECHARGE = 1;
        public const int MAX_BATTERY = 30;
        public const int SURFACE = 500;
        public const int MOTOR_RANGE = 1400;

        public int Id { get; }
        public int PlayerId { get; }

        public Vector Position { get; set; }
        public Vector Speed { get; set; }
        public bool Emergency { get; set; }
        public int Battery { get; set; } = MAX_BATTERY;
        public bool Lighting { get; set; }

        public List<int> Scans { get; set; } = new List<int>();
        public List<int> NewScans { get; set; } = new List<int>();                
        public List<RadarBlip> RadarBlips { get; set; } = new List<RadarBlip>();

        [JsonIgnore]
        public bool Scanning => NewScans.Any();
        [JsonIgnore]
        public int LightRadius => Lighting ? LIGHT_SCAN_RADIUS : SCAN_RADIUS;

        public Drone(int id, int playerId)
        {
            Id = id;
            PlayerId = playerId;
        }

        public override string ToString() => string.Format("[{0}] {1} Drone {2} V {3} B {4:D2} S {5:D2}{6}", Id, PlayerId == 0 ? "My" : "Enemy", Position.ToIntString(), (int)Speed.Length(), Battery, Scans.Count, Emergency ? " Broken" : string.Empty);

        public object Clone()
        {
            var drone = (Drone)MemberwiseClone();
            drone.NewScans = new List<int>(NewScans);
            drone.Scans = new List<int>(Scans);
            drone.RadarBlips = new List<RadarBlip>(RadarBlips);
            return drone;
        }
    }
}
