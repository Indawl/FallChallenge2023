using DevLib.Game;

namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameAction : IGameAction
    {
        public GameActionType Type { get; }
        public int X { get; }
        public int Y { get; }
        public bool Light { get; }

        public GameAction(GameActionType type, bool light = false, int x = 0, int y = 0)
        {
            Type = type;
            X = x;
            Y = y;
            Light = light;
        }
    }
}
