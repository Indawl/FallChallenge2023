using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class EarlySaveDecision : Decision
    {
        public override DecisionType Type => DecisionType.EarlySaveDecision;
        public EarlySaveDecision(DroneAgent agent) : base(agent) { }

        public override bool Check()
        {
            if (!Agent.Drone.Scans.Any()) return false;

            var myDrones = Agent.State.GetDrones(Agent.Drone.PlayerId);
            var enemyDrones = Agent.State.GetDrones(1 - Agent.Drone.PlayerId);

            var minScore = GetMinScore(1 - Agent.Drone.PlayerId, enemyDrones, myDrones) 
                         - GetMinScore(Agent.Drone.PlayerId, myDrones, enemyDrones);
            var bonusesFishes = GetBonusesFishes();
            var bonuses = GetBonuses(bonusesFishes);
            
            var bothDroneSave = GetDroneBonuses(bonusesFishes, myDrones, enemyDrones);
            var thisDroneSave = GetDroneBonuses(bonusesFishes, myDrones.Where(_=>_.Id == Agent.Drone.Id).ToList(), enemyDrones);
            var anotherDroneSave = GetDroneBonuses(bonusesFishes, myDrones.Where(_=>_.Id != Agent.Drone.Id).ToList(), enemyDrones);

            var limit = (minScore + bonuses) / 2;
            if (anotherDroneSave >= limit) return false;//i tot i tot??
            if (bothDroneSave >= limit || thisDroneSave >= limit) return true;
            return false;
        }

        private List<Fish> GetBonusesFishes() 
            => Agent.State.SwimmingFishes.Where(_ => !Agent.State.MyScans.Contains(_.Id) && !Agent.State.EnemyScans.Contains(_.Id)).ToList();

        private int GetComboBonuses(List<Fish> addedFishes = null, List<Fish> bonusesFishes = null)
        {
            var score = 0;
            var scanFishes = Agent.State.MyScans.Intersect(Agent.State.EnemyScans).Select(_ => Agent.State.GetFish(_)).ToList();
            if (addedFishes != null)
            {
                score -= GetComboBonuses();
                scanFishes = scanFishes.Union(addedFishes).ToList();
            }

            foreach (var color in GameProperties.COLORS)
            {
                var fishes = scanFishes.Where(c => c.Color == color).ToList();
                if (GameProperties.TYPES.All(_ => fishes.Exists(f => f.Type == _)))
                {
                    score += GameProperties.REWARDS[FishType.ONE_COLOR];
                    if (bonusesFishes != null && bonusesFishes.Any(_ => _.Color == color))
                        score += GameProperties.REWARDS[FishType.ONE_COLOR];
                }
            }

            foreach (var type in GameProperties.TYPES)
            {
                var fishes = scanFishes.Where(c => c.Type == type).ToList();
                if (GameProperties.COLORS.All(_ => fishes.Exists(f => f.Color == _)))
                {
                    score += GameProperties.REWARDS[FishType.ONE_TYPE];
                    if (bonusesFishes != null && bonusesFishes.Any(_ => _.Type == type))
                        score += GameProperties.REWARDS[FishType.ONE_TYPE];
                }
            }

            return score;
        }

        private int GetBonuses(List<Fish> fishes)
        {
            var bonuses = GetComboBonuses(fishes);

            foreach (var fish in fishes)
                bonuses += GameProperties.REWARDS[fish.Type];

            return bonuses;
        }

        private int GetDroneBonuses(List<Fish> fishes, List<Drone> myDrones, List<Drone> enemyDrones)
        {
            var score = 0;
            var myComboFishes = new List<Fish>();
            var myBonusesComboFishes = new List<Fish>();
            var enemyComboFishes = new List<Fish>();
            var enemyBonusesComboFishes = new List<Fish>();

            foreach (var fish in fishes)
            {
                var mD = myDrones.Where(_ => _.Scans.Contains(fish.Id)).ToList();
                var eD = enemyDrones.Where(_ => _.Scans.Contains(fish.Id)).ToList();

                if (mD.Any()) myComboFishes.Add(fish);
                if (eD.Any()) enemyComboFishes.Add(fish);

                if (mD.Any(_ => eD.All(d => Math.Floor(d.Position.Y / GameProperties.DRONE_MAX_SPEED) > Math.Floor(_.Position.Y / GameProperties.DRONE_MAX_SPEED))))
                {
                    score += GameProperties.REWARDS[fish.Type];
                    myBonusesComboFishes.Add(fish);
                }
                if (eD.Any(_ => mD.All(d => Math.Floor(d.Position.Y / GameProperties.DRONE_MAX_SPEED) > Math.Floor(_.Position.Y / GameProperties.DRONE_MAX_SPEED))))
                {
                    score -= GameProperties.REWARDS[fish.Type];
                    enemyBonusesComboFishes.Add(fish);
                }
            }

            score += GetComboBonuses(myComboFishes, myBonusesComboFishes) - GetComboBonuses(enemyComboFishes, enemyBonusesComboFishes);

            return score;
        }

        private int GetMinScore(int playerId, List<Drone> myDrones, List<Drone> enemyDrones)
        {
            var score = Agent.State.GetScore(playerId);
            //var comboFishes = Agent.State.GetScans(playerId).Select(_ => Agent.State.GetFish(_)).ToList();
            //var bonusesComboFishes = new List<Fish>(comboFishes);

            //foreach (var fish in Agent.State.Fishes
            //    .Where(_ => myDrones.Any(s => s.Scans.Contains(_.Id))
            //             || (!Agent.State.GetScans(playerId).Contains(_.Id) && _.Status != FishStatus.LOSTED && _.Color != FishColor.UGLY)))
            //{
            //    score += GameProperties.REWARDS[fish.Type];
            //    comboFishes.Add(fish);
            //    if (fish.Status == FishStatus.LOSTED && !enemyDrones.Any(_ => _.Scans.Contains(fish.Id)))
            //    {
            //        score += GameProperties.REWARDS[fish.Type];
            //        bonusesComboFishes.Add(fish);
            //    }
            //}

            //score += GetComboBonuses(comboFishes, bonusesComboFishes);

            return score;
        }

        public override bool CheckGoal(DroneAgentGoal goal) => !Agent.Drone.Scans.Any();

        public override GameAction GetAction()
        {
            Agent.Goal = new DroneAgentGoal(Type);
            var newPosition = GameUtils.GetAroundMonsterTo(Agent.State, Agent.Drone.Position, new Vector(Agent.Drone.Position.X, GameProperties.SURFACE), Agent.Drone.Id);
            return new GameActionMove(newPosition, Agent.NeedLighting(newPosition)) { Text = "Faster Faster" };
        }
    }
}
