using DevLib.Game;

namespace FallChallenge2023.Bots.Bronze.Simulations
{
    public class MinMaxVariant
    {
        public IGameAction Action { get; }
        public IGameState State { get; set; }

        public MinMaxVariant(IGameAction action)
        {
            Action = action;
        }
    }
}
