using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameReferee
    {
        public GameState State { get; set; }
        public Stopwatch StopWatch { get; set; }
        public long TimeOutTime { get; set; }

        public GameReferee(GameState state = null)
        {
            State = state;
        }

        public void UpdateFishs()
        {
            UpdateFishs(State.SwimmingFishes);
        }

        public void UpdateFishs(Func<Fish, bool> predicate)
        {
            UpdateFishs(State.SwimmingFishes.Where(predicate));
        }

        public Vector GetFishSpeed(Fish fish) => fish.Type == FishType.ANGLER ?
            GetUglySpeed(fish.Id, fish.Position, fish.Speed) :
            GetFishSpeed(fish.Id, fish.Type, fish.Position, fish.Speed);

        public void UpdatePositions()
        {
            UpdatePositions(State.SwimmingFishes);
        }

        public void UpdatePositions(Func<Fish, bool> predicate)
        {
            UpdatePositions(State.SwimmingFishes.Where(predicate));
        }

        private void UpdateFishs(IEnumerable<Fish> fishes)
        {
            // New Position
            UpdatePositions(fishes);

            // New Speed
            UpdateSpeeds(fishes);
        }

        private void UpdatePositions(IEnumerable<Fish> fishes)
        {
            foreach (var fish in fishes.Where(_ => _.Speed != null))
                fish.Position = GameUtils.SnapToFishZone(fish.Type, fish.Position + fish.Speed);
        }

        public void RemoveLostedFish()
        {
            foreach (var fish in State.Fishes.Where(_ => _.Speed != null).ToList())
                if ((fish.Position.X + fish.Speed.X < 0 || fish.Position.X + fish.Speed.X > GameProperties.MAP_SIZE - 1))
                {
                    State.FishLosted(fish);
                    State.NewEvent = true;
                }
        }

        public void UpdateSpeeds()
        {
            UpdateSpeeds(State.SwimmingFishes);
        }

        public void UpdateSpeeds(Func<Fish, bool> predicate)
        {
            UpdateSpeeds(State.SwimmingFishes.Where(predicate));
        }

        private void UpdateSpeeds(IEnumerable<Fish> fishes)
        {
            foreach (var fish in fishes.Where(_ => _.Speed != null))
                fish.Speed = GetFishSpeed(fish);
        }

        private Vector GetFishSpeed(int fishId, FishType type, Vector position, Vector speed)
        {
            // Near drone
            var dronePositions = position.GetClosest(State.Drones
                .Where(drone => !drone.Emergency && drone.Position.InRange(position, GameProperties.MOTOR_RANGE))
                .Select(drone => drone.Position)
                .ToList());
            if (dronePositions.Any())
            {
                var pos = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                speed = ((position - pos).Normalize() * GameProperties.FISH_FRIGHTENED_SPEED).Round();
            }
            else
            {
                // Near fish
                var fishesPositions = position.GetClosest(State.Fishes
                    .Where(fish => fish.Speed != null && fish.Id != fishId && fish.Position.InRange(position, GameProperties.MIN_DISTANCE_BT_FISH))
                    .Select(fish => fish.Position)
                    .ToList());
                if (fishesPositions.Any())
                {
                    var pos = fishesPositions.Aggregate((a, b) => a + b) / fishesPositions.Count;
                    speed = (position - pos).Normalize() * GameProperties.FISH_SPEED;
                }
                else speed = speed.Normalize() * GameProperties.FISH_SPEED;

                // Border
                var nextPosition = position + speed;

                var habitat = GameProperties.HABITAT[type];
                if (nextPosition.X < 0 || nextPosition.X > GameProperties.MAP_SIZE - 1) speed = speed.HSymmetric();
                if (nextPosition.Y < habitat[0] || nextPosition.Y > habitat[1]) speed = speed.VSymmetric();

                speed = speed.EpsilonRound().Round();
            }

            return speed;
        }

        private Vector GetUglySpeed(int fishId, Vector position, Vector speed)
        {
            // Near drone
            var dronePositions = position.GetClosest(State.Drones
                .Where(drone => !drone.Emergency && drone.Position.InRange(position, drone.LightRadius))
                .Select(drone => drone.Position)
                .ToList());
            if (dronePositions.Any())
            {
                var pos = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                speed = ((pos - position).Normalize() * GameProperties.MONSTER_ATTACK_SPEED).Round();
            }
            else
            {
                if (speed.Length() > GameProperties.MONSTER_SPEED)
                    speed = (speed.Normalize() * GameProperties.MONSTER_SPEED).Round();

                // Near other ugly
                var fishesPositions = position.GetClosest(State.Monsters
                    .Where(fish => fish.Speed != null && fish.Id != fishId && fish.Position.InRange(position, GameProperties.MIN_DISTANCE_BT_MONSTER))
                    .Select(fish => fish.Position)
                    .ToList());
                if (fishesPositions.Any())
                {
                    var pos = fishesPositions.Aggregate((a, b) => a + b) / fishesPositions.Count;
                    speed = ((position - pos).Normalize() * GameProperties.FISH_SPEED).Round(); // Fish.MONSTER_SPEED; // repeat error from referee
                }

                // Border
                var nextPosition = position + speed;

                var habitat = GameProperties.HABITAT[FishType.ANGLER];
                if (nextPosition.X < 0 || nextPosition.X > GameProperties.MAP_SIZE - 1) speed = speed.HSymmetric();
                if (nextPosition.Y < habitat[0] || nextPosition.Y > habitat[1]) speed = speed.VSymmetric();
            }

            return speed;
        }

        public bool IsOver(GameState state) => state.Turn > GameProperties.LAST_TURN;

        public static int GetDronesScore(GameState state, int playerId)
        {
            var score = 0;

            var bonus = new HashSet<int>();
            var allScans = state.GetScans();
            var scans = state.GetDrones(playerId).SelectMany(drone => drone.Scans).Distinct().Except(state.GetScans(playerId)).ToHashSet();

            foreach (var fish in scans.Select(fishId => state.GetAnyFish(fishId)))
            {
                score += GameProperties.REWARDS[fish.Type];

                if (!allScans.Contains(fish.Id))
                {
                    var drones = state.GetDrones(playerId).Where(drone => drone.Scans.Contains(fish.Id));
                    var myMinDistance = drones.Any() ? drones.Min(drone => GameUtils.GetDistance(state, drone.Position)) : 0;
                    drones = state.GetDrones(1 - playerId).Where(drone => drone.Scans.Contains(fish.Id));
                    var enemyMinDistance = drones.Any()? drones.Min(drone => GameUtils.GetDistance(state, drone.Position)) : 0;
                    if (myMinDistance < enemyMinDistance)
                    {
                        score += GameProperties.REWARDS[fish.Type];
                        bonus.Add(fish.Id);
                    }
                }
            }

            score += GetComboBonus(state, scans, bonus);

            return score;
        }

        public static int GetSwimmingScore(GameState state, int playerId, bool withBonus)
        {
            var score = 0;
            var unscanned = state.GetUnscannedFish(playerId);

            foreach (var fish in unscanned.Select(fishId => state.GetAnyFish(fishId)))
                score += GameProperties.REWARDS[fish.Type];

            score += GetComboBonus(state, unscanned, withBonus ? unscanned : null);

            return score;
        }

        public static int GetComboBonus(GameState state, IEnumerable<int> scans, IEnumerable<int> bonus = null)
        {
            var score = 0;

            var allScans = state.GetScans().Union(scans).Select(fishId => state.GetAnyFish(fishId));

            foreach (var color in scans.Select(fishId => state.GetAnyFish(fishId).Color).Distinct())
            {
                var fishes = allScans.Where(fish => fish.Color == color).ToList();
                if (GameProperties.TYPES.All(type => fishes.Exists(fish => fish.Type == type)))
                {
                    score += GameProperties.REWARDS_COLOR;
                    if (bonus != null && bonus.Any(fishId => state.GetAnyFish(fishId).Color == color))
                        score += GameProperties.REWARDS_COLOR;
                }
            }

            foreach (var type in scans.Select(fishId => state.GetAnyFish(fishId).Type).Distinct())
            {
                var fishes = allScans.Where(fish => fish.Type == type).ToList();
                if (GameProperties.COLORS.All(color => fishes.Exists(fish => fish.Color == color)))
                {
                    score += GameProperties.REWARDS_TYPE;
                    if (bonus != null && bonus.Any(fishId => state.GetAnyFish(fishId).Type == type))
                        score += GameProperties.REWARDS_TYPE;
                }
            }

            return score;
        }

        public GameState InitNext(GameState state, Decision[] decisions)
        {
            State = (GameState)state.Clone();
            State.DefferedDecisions = decisions.ToList();
            return State;
        }

        public GameState GetNext(GameState state, Decision[] decisions)
        {
            State = (GameState)state.Clone();
            State.NewEvent = false;

            var processDecisions = State.DefferedDecisions.Union(decisions).ToList();
            State.DefferedDecisions = new List<Decision>();

            foreach (var decision in processDecisions.Where(d => d is SaveDecision))
                State.SavedDroneId.Add(decision.DroneId);

            while (!State.NewEvent && !IsOver(State))
            {
                // Check, maybe all done
                foreach (var decision in processDecisions)
                    if (decision.Finished(State)) return State;

                // Do now
                foreach (var decision in processDecisions)
                    UpdateDrone(decision.DroneId, decision.GetAction(State));

                // Check Timeout
                if (TimeOutTime > 0 && StopWatch.ElapsedMilliseconds > TimeOutTime)
                    throw new TimeoutException();

                // Update state                
                RemoveLostedFish();
                UpdatePositions();
                DoScans();
                DoReports();
                UpdateSpeeds();

                State.RefreshBuffer();
                State.Turn++;
            }

            return State;
        }

        public void UpdateDrone(int droneId, GameAction action)
        {
            var drone = State.GetDrone(droneId);
            drone.NewScans.Clear();

            if (drone.Emergency)
            {
                drone.Position -= new Vector(0, GameProperties.DRONE_EMERGENCY_SPEED);
                drone.Position = GameUtils.SnapToDroneZone(drone.Position);
                if (drone.Position.Y == 0) drone.Emergency = false;
            }
            else
            {
                drone.Speed = drone.Position;
                if (action is GameActionMove actionMove)
                {
                    drone.Position = actionMove.Position;
                    drone.Lighting = actionMove.Light;
                    drone.MotorOn = true;
                }
                else if (action is GameActionWait actionWait)
                {
                    drone.Position += new Vector(0, GameProperties.DRONE_SINK_SPEED);
                    drone.Lighting = actionWait.Light;
                    drone.MotorOn = false;
                }
                drone.Position = GameUtils.SnapToDroneZone(drone.Position);
                drone.Speed = drone.Position - drone.Speed;

                if (drone.Battery < GameProperties.BATTERY_DRAIN) drone.Lighting = false;
                drone.Battery += drone.Lighting ? -GameProperties.BATTERY_DRAIN : GameProperties.BATTERY_RECHARGE;
                drone.Battery = Math.Min(drone.Battery, GameProperties.MAX_BATTERY);

                foreach (var fish in State.Monsters.Where(_ => _.Speed != null && _.Position.InRange(drone.Position, 1700)))
                    if (GameUtils.CheckCollision(fish.Position, fish.Speed, drone.Position - drone.Speed, drone.Position))
                    {
                        drone.Emergency = true;
                        drone.MotorOn = false;
                        drone.Lighting = false;
                        drone.Scans.Clear();
                        break;
                    }
            }
        }

        public void DoScans()
        {
            foreach (var drone in State.Drones.Where(_ => !_.Emergency))
                foreach (var fish in State.Fishes.Where(_ => !drone.Scans.Contains(_.Id) && !State.GetScans(drone.PlayerId).Contains(_.Id)
                                                          && _.Position.InRange(drone.Position, drone.LightRadius)))
                {
                    drone.Scans.Add(fish.Id);
                    drone.NewScans.Add(fish.Id);
                    State.NewEvent = true;
                }
        }

        private void DoReports()
        {
            var myDrones = State.MyDrones.Where(drone => !drone.Emergency && (int)drone.Position.Y <= GameProperties.SURFACE);
            var myScans = myDrones
                .SelectMany(drone => drone.Scans)
                .Distinct()
                .Except(State.MyScans)
                .ToHashSet();
            var enemyDrones = State.EnemyDrones.Where(drone => !drone.Emergency && (int)drone.Position.Y <= GameProperties.SURFACE);
            var enemyScans = enemyDrones
                .SelectMany(drone => drone.Scans)
                .Distinct()
                .Except(State.EnemyScans)
                .ToHashSet();

            foreach (var fish in myScans.Select(fishId => State.GetAnyFish(fishId)))
            {
                State.AddScore(0, GameProperties.REWARDS[fish.Type]);
                State.NewEvent = true;

                if (!enemyScans.Contains(fish.Id))
                    State.AddScore(0, GameProperties.REWARDS[fish.Type]);
            }
            foreach (var fish in enemyScans.Select(fishId => State.GetAnyFish(fishId)))
            {
                State.AddScore(1, GameProperties.REWARDS[fish.Type]);
                State.NewEvent = true;

                if (!myScans.Contains(fish.Id))
                    State.AddScore(1, GameProperties.REWARDS[fish.Type]);
            }

            State.AddScore(0, GetComboBonus(State, myScans, myScans.Except(enemyScans)));
            State.AddScore(1, GetComboBonus(State, enemyScans, enemyScans.Except(myScans)));

            foreach (var fishId in myScans)
                State.MyScans.Add(fishId);
            foreach (var fishId in enemyScans)
                State.EnemyScans.Add(fishId);

            foreach (var drone in myDrones.Union(enemyDrones))
            {
                drone.Scans.Clear();
                drone.NewScans.Clear();
            }
        }
    }
}
