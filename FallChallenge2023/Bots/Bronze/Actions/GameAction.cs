using DevLib.Game;

namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameAction : IGameAction
    {
        public GameActionType Type { get; }        

        public string Text { get; set; }

        public GameAction(GameActionType type)
        {
            Type = type;
        }
    }
}
