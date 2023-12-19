using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class Drone
    {
        public int Id { get; }
        public int PlayerId { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Emergency { get; set; }
        public int Battery { get; set; }

        public List<int> Scans { get; } = new List<int>();
        public List<RadarBlip> RadarBlips { get; } = new List<RadarBlip>();

        public Drone(int id, int playerId, int x, int y, bool emergency, int battery)
        {
            Id = id;
            PlayerId = playerId;
            X = x;
            Y = y;
            Emergency = emergency;
            Battery = battery;
        }
    }
}
