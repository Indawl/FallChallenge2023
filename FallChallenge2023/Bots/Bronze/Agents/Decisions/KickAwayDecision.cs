using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class KickAwayDecision : Decision
    {
        public override int Id => GameProperties.KickAwayDecision;

        private List<Fish> Fishes { get; set; }

        public KickAwayDecision(DroneAgent agent) : base(agent) { }
        
        public override bool Check()
        {
            Fishes = Agent.State.SwimmingFishes
                .Where(_ => _.Position.X * Agent.LessX > Agent.Drone.Position.X * Agent.LessX
                         && Agent.State.UnscannedFishes[1 - Agent.Drone.PlayerId].Contains(_)
                         && _.Position.InRange(Agent.Drone.Position, GameProperties.LIGHT_SCAN_RADIUS + GameProperties.DRONE_MAX_SPEED)).ToList();

            return Fishes.Any();
        }

        public override GameAction GetAction()
        {
            var fish = Fishes.OrderBy(_ => (_.Position - Agent.Drone.Position).LengthSqr()).First();

            //добавить скорость рыбы в дельто с проверкой на нулл и если рыба внизу скорелировать х для макс вектора ухода
            var x = Agent.Drone.Position.X;
            var delta = Agent.LessX > 0 ? GameProperties.MAP_SIZE - 1 - GameProperties.MOTOR_RANGE : GameProperties.MOTOR_RANGE;

            if (Agent.LessX > 0 && delta > x) x = delta;
            else if (Agent.LessX < 0 && delta < x) x = delta;

            var newPosition = GameUtils.GetAroundMonsterTo(Agent.State, Agent.Drone.Position, new Vector(x, fish.Position.Y), Agent.Drone.Id);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Kick away" };
        }
    }
}
