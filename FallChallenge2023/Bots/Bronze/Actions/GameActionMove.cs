using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameActionMove : GameAction
    {
        public GameActionMove(Vector position, bool light = false) : base(GameActionType.MOVE, light, position)
        {
        }

        public override string ToString() => string.Format("MOVE {0} {1} {2} {3}", (int)Position.X, (int)Position.Y, Light ? 1 : 0, Text);
    }
}
