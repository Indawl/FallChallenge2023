using DevLib.Game;
using DevLib.GameMath;

namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameAction : IGameAction
    {
        public GameActionType Type { get; }
        public int DroneId { get; }
        public Vector Position { get; }
        public bool Light { get; }

        public GameAction(GameActionType type, int droneId = 0, bool light = false, Vector position = null)
        {
            Type = type;
            DroneId = droneId;
            Position = position;
            Light = light;
        }
    }
}
