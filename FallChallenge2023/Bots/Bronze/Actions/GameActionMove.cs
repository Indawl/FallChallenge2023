using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameActionMove : GameAction
    {
        public GameActionMove(Vector position, bool light = false) : base(GameActionType.MOVE, light, position)
        {
        }

        public override string ToString() => string.Format("MOVE {0} {1} {2}", Position.X, Position.Y, Light ? 1 : 0);
    }
}
