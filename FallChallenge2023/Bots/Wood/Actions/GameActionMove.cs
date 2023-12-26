using DevLib.GameMath;

namespace FallChallenge2023.Bots.Wood.Actions
{
    public class GameActionMove : GameAction
    {
        public GameActionMove(VectorD position, bool light = false) : base(GameActionType.MOVE, light, position)
        {
        }

        public override string ToString() => string.Format("MOVE {0} {1} {2}", (int)Position.X, (int)Position.Y, Light ? 1 : 0);
    }
}
