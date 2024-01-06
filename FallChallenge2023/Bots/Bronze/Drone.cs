using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class Drone : ICloneable
    {
        public int Id { get; }
        public int PlayerId { get; }

        public Vector Position { get; set; }
        public Vector Speed { get; set; }
        public bool Emergency { get; set; }
        public int Battery { get; set; } = GameProperties.MAX_BATTERY;
        public int LightRadius { get; set; }
        public bool Lighting { get; set; }
        public bool MotorOn { get; set; }

        public HashSet<int> Scans { get; set; } = new HashSet<int>();
        public HashSet<int> NewScans { get; set; } = new HashSet<int>();
        public List<RadarBlip> RadarBlips { get; set; } = new List<RadarBlip>();

        public Drone(int id = 0, int playerId = 0)
        {
            Id = id;
            PlayerId = playerId;
        }

        public override string ToString()
            => string.Format("[{0}] {1} Drone {2} V {3} B {4:D2} S {5:D2}{6}", Id, PlayerId == 0 ? "My" : "Enemy", Position.ToIntString(), (int)Speed.Length(), Battery, Scans.Count, Emergency ? " Broken" : string.Empty);

        public object Clone()
        {
            var drone = (Drone)MemberwiseClone();
            drone.Scans = new HashSet<int>(Scans);
            drone.NewScans = new HashSet<int>(NewScans);
            drone.RadarBlips = new List<RadarBlip>(); // no need to copy radar blips
            return drone;
        }
    }
}
