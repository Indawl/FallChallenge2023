namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameActionWait : GameAction
    {
        public GameActionWait(int droneId, bool light = false) : base(GameActionType.WAIT, droneId, light)
        {
        }

        public override string ToString() => string.Format("WAIT {0}", Light ? 1 : 0);
    }
}
