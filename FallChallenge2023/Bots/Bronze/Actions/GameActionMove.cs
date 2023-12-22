namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameActionMove : GameAction
    {
        public GameActionMove(int x, int y, bool light = false) : base(GameActionType.MOVE, light, x, y)
        {
        }

        public override string ToString() => string.Format("MOVE {0} {1} {2}", X, Y, Light ? 1 : 0);
    }
}
