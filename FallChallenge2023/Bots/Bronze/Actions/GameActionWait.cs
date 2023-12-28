namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameActionWait : GameAction
    {
        public bool Light { get; }

        public GameActionWait(bool light = false) : base(GameActionType.WAIT)
        {
            Light = light;
        }

        public override string ToString() => string.Format("WAIT {0} {1}", Light ? 1 : 0, Text);
    }
}
