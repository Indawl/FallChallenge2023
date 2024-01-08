using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class KickAwayDecision : Decision
    {
        public int FishId { get; }

        public KickAwayDecision(int droneId, int fishId) : base(droneId)
        {
            FishId = fishId;
        }

        public override bool Finished(GameState state) => !state.GetUnscannedFish(1 - state.GetDrone(DroneId).PlayerId).Contains(FishId);

        protected override GameAction CalculateAction(GameState state)
        {
            var drone = state.GetDrone(DroneId);
            var fish = state.GetFish(FishId);
            
            var fishPosition = fish.Position;
            fishPosition += fish.Speed;

            var fromDirectionY = (drone.Position.Y < fishPosition.Y - GameProperties.MOTOR_RANGE) ? - 1 
                               : (drone.Position.Y > fishPosition.Y + GameProperties.MOTOR_RANGE) ? 1 : 0;

            var fromDirectionX = Math.Sign((int)(drone.Position.X - fishPosition.X));
            if (fishPosition.X < GameProperties.CENTER.X - GameProperties.MOTOR_RANGE / 2) fromDirectionX = 1;
            else if (fishPosition.X > GameProperties.CENTER.X + GameProperties.MOTOR_RANGE / 2) fromDirectionX = -1;
            else if (fromDirectionX == 0) fromDirectionX = GameProperties.CENTER.X > fishPosition.X ? 1 : -1;

            if (fromDirectionX * drone.Position.X > fromDirectionX * fishPosition.X + GameProperties.MOTOR_RANGE) fromDirectionY = 0;

            if (fromDirectionY == 0)
                fishPosition += new Vector(fromDirectionX * GameProperties.MOTOR_RANGE, 0);
            else
            {
                fishPosition += new Vector(fromDirectionX * GameProperties.MOTOR_RANGE / 2, fromDirectionY * (GameProperties.MOTOR_RANGE + 1));

                var dir = fishPosition - drone.Position;
                var distSqr = dir.LengthSqr();
                //if (fromDirectionY < 0 && drone.Position.X > fishPosition.X - fromDirectionX * GameProperties.MOTOR_RANGE / 2 
                //                       && drone.Position.X < fishPosition.X
                //                       && distSqr < GameProperties.DRONE_SINK_SPEED * GameProperties.DRONE_SINK_SPEED / 9)
                //{
                //    if (!GameUtils.CheckCollisionWithMonsters(state, drone.Position, new Vector(drone.Position.X, drone.Position.Y + GameProperties.DRONE_SINK_SPEED), out var speed))
                //        return new GameActionWait(NeedLight(state));
                //}
                if (distSqr < GameProperties.DRONE_MAX_SPEED * GameProperties.DRONE_MAX_SPEED)
                {
                    var x = fromDirectionX * Math.Sqrt(GameProperties.DRONE_MAX_SPEED * GameProperties.DRONE_MAX_SPEED - dir.Y * dir.Y);
                    var y = dir.Y;
                    if (fromDirectionX * drone.Position.X + x > fromDirectionX * fishPosition.X + GameProperties.MOTOR_RANGE / 2)
                    {
                        x = fishPosition.X + fromDirectionX * GameProperties.MOTOR_RANGE / 2;
                        y = -fromDirectionY * Math.Sqrt(GameProperties.DRONE_MAX_SPEED * GameProperties.DRONE_MAX_SPEED - x * x);
                    }
                    fishPosition = new Vector(x, y);
                    fishPosition.Round();
                }
            }

            if (fishPosition.Equals(drone.Position)) fishPosition -= new Vector(fromDirectionX, 0);
            var newPosition = GameUtils.GetAroundMonsterTo(state, drone.Position, fishPosition);
            return new GameActionMove(newPosition, NeedLight(state, drone, newPosition));
        }
    }
}
