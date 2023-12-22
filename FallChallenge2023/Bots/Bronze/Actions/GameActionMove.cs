using DevLib.GameMath;

namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameActionMove : GameAction
    {
        public GameActionMove(int droneId, Vector position, bool light = false) : base(GameActionType.MOVE, droneId, light, position)
        {
        }

        public override string ToString() => string.Format("MOVE {0} {1} {2}", (int)Position.X, (int)Position.Y, Light ? 1 : 0);
    }
}
