using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class SearchDecision : Decision
    {
        public override int Id => GameProperties.SearchDecision;

        public SearchDecision(DroneAgent agent) : base(agent) { }

        public override bool Check() => Agent.UnscannedFishes.Any();

        public override GameAction GetAction()
        {
            var fish = Agent.UnscannedFishes.OrderBy(_ =>
                (_.Position - Agent.Drone.Position -
                new Vector(_.Position.X * Agent.LessX <= Agent.Drone.Position.X * Agent.LessX ? GameProperties.MAP_SIZE * Agent.LessX : 0, 0))
                .LengthSqr()).First();
            var newPosition = GameUtils.GetAroundMonsterTo(Agent.State, Agent.Drone.Position, fish.Position, Agent.Drone.Id);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Searching..." };
        }
    }
}
