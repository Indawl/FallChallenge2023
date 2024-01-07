using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public abstract class Decision
    {
        public int DroneId { get; }
        public GameAction Action { get; private set; }

        public Decision(int droneId)
        {
            DroneId = droneId;
        }
        public GameAction GetAction(GameState state) => Action == null ? Action = CalculateAction(state) : CalculateAction(state);
        public abstract bool Finished(GameState state);

        protected abstract GameAction CalculateAction(GameState state);

        protected bool NeedLight(GameState state, Drone drone, Vector dronePosition)
        {
            // Unscanned fishes close
            if (state.GetUnscannedFish(drone.PlayerId).Select(fishId => state.GetFish(fishId))
                                                      .Any(fish => dronePosition.InRange(fish.Position + fish.Speed, GameProperties.DARK_SCAN_RADIUS, GameProperties.LIGHT_SCAN_RADIUS)))
                return true;

            var enemyDrones = state.GetDrones(1 - drone.PlayerId);

            // Enemy with light close and have new scans
            if (!drone.Lighting && enemyDrones
                .Any(_ => _.Lighting && _.Position.InRange(drone.Position, GameProperties.DARK_SCAN_RADIUS)
                                     && _.NewScans.Any(s => state.GetUnscannedFish(drone.PlayerId).Contains(s))))
                return true;

            // Maybe he go away from monster
            if (!drone.Lighting && enemyDrones
                .Any(_ => !_.Position.InRange(drone.Position, GameProperties.DARK_SCAN_RADIUS)
                       && _.Position.InRange(dronePosition, GameProperties.DARK_SCAN_RADIUS)))
                return true;

            // Incite monster to enemy
            if (!drone.Lighting)
                foreach (var fish in state.Monsters.Where(_ => !_.Speed.IsZero() && dronePosition.InRange(_.Position + _.Speed, GameProperties.DARK_SCAN_RADIUS, GameProperties.LIGHT_SCAN_RADIUS)))
                    if (enemyDrones.Any(_ => GameUtils.CheckCollision(fish.Position,
                        ((dronePosition - fish.Position - fish.Speed).Normalize() * GameProperties.MONSTER_ATTACK_SPEED).Round(),
                        _.Position, _.Position + _.Speed, true)))
                        return true;

            return false;
        }
    }
}
