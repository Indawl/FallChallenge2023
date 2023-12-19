using DevLib.Game;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Wood
{
    public class GameState : IGameState
    {
        public int[] Score { get; } = new int[2];
        public List<int>[] Scans { get; } = new List<int>[2];
        public Dictionary<int, Drone> Drones { get; } = new Dictionary<int, Drone>();
        public Dictionary<int, Fish> Fishes { get; } = new Dictionary<int, Fish>();

        public GameState()
        {
            for (int k = 0; k < 2; k++)
                Scans[k] = new List<int>();
        }
    }
}
