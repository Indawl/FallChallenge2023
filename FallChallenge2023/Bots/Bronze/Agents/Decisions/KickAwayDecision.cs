using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class KickAwayDecision : Decision
    {
        public override DecisionType Type => DecisionType.KickAwayDecision; 

        private List<Fish> Fishes { get; set; }

        public KickAwayDecision(DroneAgent agent) : base(agent) { }

        public override bool Check()
        {
            //Fishes = Agent.State.SwimmingFishes
            //    .Where(_ => _.Position.X * Agent.LessX >= Agent.Drone.Position.X * Agent.LessX
            //             && Agent.State.UnscannedFishes[1 - Agent.Drone.PlayerId].Contains(_)).ToList();

            return Fishes.Any();
        }

        public override GameAction GetAction()
        {
            return null;
            //var fishes = Fishes.Where(_ => Priority(_) == 2);
            //var fish = fishes.OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).FirstOrDefault();
            //if (fish == null)
            //{
            //    fishes = Fishes.Where(_ => Priority(_) == 1);
            //    fish = fishes.OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).FirstOrDefault();
            //}
            //if (fish == null)
            //    fish = Fishes.OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).FirstOrDefault();

            //var speed = fish.Speed == null ? 0 : fish.Speed.X;
            //var x = fish.Position.X - Agent.LessX * (GameProperties.MOTOR_RANGE - speed - 1);
            //var delta = Agent.LessX > 0 ? GameProperties.MAP_SIZE - GameProperties.MOTOR_RANGE - speed : GameProperties.MOTOR_RANGE + speed - 1;

            //if (delta * Agent.LessX > x * Agent.LessX) x = delta;

            //var newPosition = GameUtils.GetAroundMonsterTo(Agent.State, Agent.Drone.Position, new Vector(x, fish.Position.Y), Agent.Drone.Id);
            //return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Hunting..." };
        }

        private int Priority(Fish fish)
        {
            var priority = 0;

            //if (Agent.State.Fishes.Count(_ => _.Status != FishStatus.LOSTED && _.Type == fish.Type) == 4) priority++;
            //if (Agent.State.Fishes.Count(_ => _.Status != FishStatus.LOSTED && _.Color == fish.Color) == 3) priority++;

            return priority;
        }
    }
}
