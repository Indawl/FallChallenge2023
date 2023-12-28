using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameActionMove : GameAction
    {
        public Vector Position { get; }
        public bool Light { get; }

        public GameActionMove(Vector position, bool light = false) : base(GameActionType.MOVE)
        {
            Position = position;
            Light = light;
        }

        public override string ToString() => string.Format("MOVE {0} {1} {2} {3}", (int)Position.X, (int)Position.Y, Light ? 1 : 0, Text);
    }
}
