using DevLib.GameMath;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class Drone
    {
        public const int MAX_SPEED = 600;
        public const int SINK_SPEED = 300;
        public const int SCAN_RADIUS = 800;
        public const int LIGHT_SCAN_RADIUS = 2000;
        public const int BATTERY_DRAIN = 5;
        public const int BATTERY_RECHARGE = 1;
        public const int MAX_BATTERY = 30;
        public const int SURFACE = 500;
        public const int EMERGENCY_SPEED = 300;
        public const int MONSTER_DETECTED_RADIUS_ADD = 300;

        public int Id { get; }
        public int PlayerId { get; }

        public Vector Position { get; set; }
        public bool Emergency { get; set; }
        public int Battery { get; set; } = MAX_BATTERY;

        public List<int> Scans { get; set; }
        public List<RadarBlip> RadarBlips { get; set; }

        public bool Lighting { get; set; }

        public Drone(int id, int playerId)
        {
            Id = id;
            PlayerId = playerId;
        }

        public override string ToString() => string.Format("{0} {1} {2} B{3:D2} S{4:D2}{5}", PlayerId == 0 ? "My" : "Enemy", Id, Position.ToIntString(), Battery, Scans.Count, Emergency ? " Broken" : string.Empty);
    }
}
