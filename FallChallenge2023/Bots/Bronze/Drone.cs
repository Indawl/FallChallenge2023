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
        public int LightRadius { get; set; }
        public bool Lighting { get; set; }
        public bool MotorOn { get; set; }

        public HashSet<int> Scans { get; set; } = new HashSet<int>();
        public HashSet<int> NewScans { get; set; } = new HashSet<int>();
        public List<RadarBlip> RadarBlips { get; set; } = new List<RadarBlip>();

        public HashSet<int> FishesDark { get; set; } = new HashSet<int>();
        public HashSet<int> FishesLight { get; set; } = new HashSet<int>();
        public HashSet<int> FishesMotor { get; set; } = new HashSet<int>();

        [JsonIgnore]
        public bool Scanning => NewScans.Any();

        public Drone(int id, int playerId)
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
            drone.FishesDark = new HashSet<int>(FishesDark);
            drone.FishesLight = new HashSet<int>(FishesLight);
            drone.FishesMotor = new HashSet<int>(FishesMotor);
            return drone;
        }

        public void SetFishes(IEnumerable<Fish> fishes)
        {
            FishesDark = new HashSet<int>();
            FishesLight = new HashSet<int>();
            FishesMotor = new HashSet<int>();

            foreach (var fish in fishes)
            {
                if (fish.Position.InRange(Position, GameProperties.DARK_SCAN_RADIUS_SQR))
                    FishesDark.Add(fish.Id);
                else if (Lighting && fish.Position.InRange(Position, GameProperties.LIGHT_SCAN_RADIUS_SQR))
                    FishesLight.Add(fish.Id);
                if (fish.Type != FishType.ANGLER && fish.Position.InRange(Position, GameProperties.MOTOR_RANGE_SQR))
                    FishesMotor.Add(fish.Id);
            }
        }
    }
}
