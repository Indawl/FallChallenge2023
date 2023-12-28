using FallChallenge2023.Bots.Bronze.GameMath;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public bool Lighting { get; set; }

        public List<int> Scans { get; set; } = new List<int>();
        public List<int> NewScans { get; set; } = new List<int>();                
        public List<RadarBlip> RadarBlips { get; set; } = new List<RadarBlip>();

        [JsonIgnore]
        public bool Scanning => NewScans.Any();
        [JsonIgnore]
        public int LightRadius => Lighting ? GameProperties.LIGHT_SCAN_RADIUS : GameProperties.DARK_SCAN_RADIUS;

        public Drone(int id, int playerId)
        {
            Id = id;
            PlayerId = playerId;
        }

        public override string ToString() => string.Format("[{0}] {1} Drone {2} V {3} B {4:D2} S {5:D2}{6}", Id, PlayerId == 0 ? "My" : "Enemy", Position.ToIntString(), (int)Speed.Length(), Battery, Scans.Count, Emergency ? " Broken" : string.Empty);

        public object Clone()
        {
            var drone = (Drone)MemberwiseClone();
            drone.Scans = new List<int>(Scans);
            drone.NewScans = new List<int>(NewScans);            
            drone.RadarBlips = new List<RadarBlip>(RadarBlips);
            return drone;
        }
    }
}
