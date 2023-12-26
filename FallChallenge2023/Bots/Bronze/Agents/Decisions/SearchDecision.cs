namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SearchDecision : Decision
    {
        public SearchDecision(DroneAgent agent, GameState state) : base(agent, state)
        {
            Decisions.Add(new DiveSearchDecision(agent, state));
            Decisions.Add(new AllSearchDecision(agent, state));
        }
    }
}
