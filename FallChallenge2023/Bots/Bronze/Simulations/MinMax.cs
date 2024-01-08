using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Simulations
{
    public class MinMax
    {
        public GameReferee Referee { get; set; }

        public Stopwatch StopWatch { get; set; }
        public long TimeOutTime { get; set; }

        public int Depth { get; set; }

        public Dictionary<GameState, MinMaxDetail> StateBuffer { get; } = new Dictionary<GameState, MinMaxDetail>();

        public void FindBestAction(GameState state)
        {
            // Create Referee
            Referee = new GameReferee()
            {
                StopWatch = StopWatch,
                TimeOutTime = TimeOutTime
            };

            // For process unvisible fishes, set speed = 0
            Referee.State = state = (GameState)state.Clone();
            foreach (var fish in Referee.State.SwimmingFishes.Where(_ => _.Speed == null))
                fish.Speed = new Vector();

            // Simulate
            try
            {
                Simulation(Referee.State, Depth);
            }
            catch (TimeoutException) { }

            Referee.State = state;
        }

        public MinMaxDetail GetStateDetails(GameState state)
        {
            if (!StateBuffer.TryGetValue(state, out var detail))
                StateBuffer.Add(state, detail = new MinMaxDetail());
            return detail;
        }

        protected void CheckTimeOut()
        {
            if (TimeOutTime > 0 && StopWatch.ElapsedMilliseconds > TimeOutTime)
                throw new TimeoutException();
        }

        private int Simulation(GameState state, int depth, int alpha = -int.MaxValue, int beta = int.MaxValue, int playerId = 0)
        {
            // Time breaker
            CheckTimeOut();

            // Check buffer
            var details = GetStateDetails(state);
            details.Depth = depth;

            // Time to Evaluation
            if (Referee.IsOver(state) || depth < 1) return Evaluation(state);

            // Get edges
            if (details.Variants == null)
                details.Variants = GetVariants(state, playerId);

            // MinMax
            if (playerId == 0)
            {
                details.Evaluation = int.MinValue;
                for (int i = 0; i < details.Variants.Length; i++)
                {
                    // Initialize New State
                    if (details.Variants[i].State == null)
                        details.Variants[i].State = Referee.InitNext(state, details.Variants[i].Decisions);

                    // Evaluation
                    int evaluation = Simulation(details.Variants[i].State, depth - 1, alpha, beta, 1 - playerId);
                    if (evaluation > details.Evaluation)
                    {
                        details.Evaluation = evaluation;
                        details.BestVariant = details.Variants[i];
                    }

                    alpha = Math.Max(alpha, details.Evaluation);
                    if (beta <= alpha) break;
                }
            }
            else
            {
                details.Evaluation = int.MaxValue;
                for (int i = 0; i < details.Variants.Length; i++)
                {
                    // Initialize New State
                    if (details.Variants[i].State == null)
                        details.Variants[i].State = Referee.GetNext(state, details.Variants[i].Decisions);

                    // Evaluation
                    int evaluation = Simulation(details.Variants[i].State, depth - 1, alpha, beta, 1 - playerId);
                    if (evaluation < details.Evaluation)
                    {
                        details.Evaluation = evaluation;
                        details.BestVariant = details.Variants[i];
                    }

                    beta = Math.Min(beta, details.Evaluation);
                    if (beta <= alpha) break;
                }
            }

            return details.Evaluation;
        }

        private MinMaxVariant[] GetVariants(GameState state, int playerId)
        {
            var decision = GetDecisions(state, playerId);
            var variants = new MinMaxVariant[decision.Length];

            for (int i = 0; i < decision.Length; i++)
                variants[i] = new MinMaxVariant(decision[i]);

            return variants;
        }

        private int Evaluation(GameState state)
            => state.Score + GameReferee.GetDronesScore(state, 0) - GameReferee.GetDronesScore(state, 1) 
                           + GameReferee.GetSwimmingScore(state, 0, false) - GameReferee.GetSwimmingScore(state, 1, true);

        private Decision[][] GetDecisions(GameState state, int playerId)
        {
            var decisionsList = new List<Decision[]>();

            var decisions = new Decision[2];

            // Check saved decisions
            var drones = state.GetDrones(playerId);
            for (int i = 0; i < 2; i++)
                if (state.SavedDroneId.Contains(drones[i].Id))
                {
                    var decision = new SaveDecision(drones[i].Id);
                    if (decision.Finished(state)) state.SavedDroneId.Remove(decision.DroneId);
                    else decisions[i] = decision;
                }
            if (decisions[0] != null && decisions[1] != null)
            {
                decisionsList.Add(decisions);
                return decisionsList.ToArray();
            }

            // Find interesting fishes
            var pairs = state.GetUnscannedFish(playerId)
                             .Union(state.GetUnscannedFish(1 - playerId))
                             .Select(fishId => state.GetFish(fishId))
                             .Select(fish => new
                             {
                                 Fish = fish,
                                 Prio = GetReward(state, fish),
                                 Dist = Math.Min(drones[0].Position.DistanceSqr(fish.Position),
                                                 drones[1].Position.DistanceSqr(fish.Position))
                             })
                             .ToList();
            pairs.Sort((a, b) => b.Prio == a.Prio ? a.Dist.CompareTo(b.Dist) : b.Prio.CompareTo(a.Prio));

            // Distribute tasks
            var fishes = new List<Fish>[] { new List<Fish>(), new List<Fish>() };
            var positions = new Vector[] { drones[0].Position, drones[1].Position };
            var distance = new double[2];

            foreach (var pair in pairs)
            {
                var dist0 = distance[0] + positions[0].DistanceSqr(pair.Fish.Position);
                var dist1 = distance[1] + positions[1].DistanceSqr(pair.Fish.Position);
                if (decisions[0] == null && dist0 < dist1 || decisions[1] != null)
                {
                    fishes[0].Add(pair.Fish);
                    distance[0] = dist0;
                    positions[0] = pair.Fish.Position;
                }
                else
                {
                    fishes[1].Add(pair.Fish);
                    distance[1] = dist1;
                    positions[1] = pair.Fish.Position;
                }
            }

            // Get decisions
            var enemyDrones = state.GetDrones(1 - playerId);

            for (int i = 0; i < 2; i++)
            {
                foreach (var fish in fishes[i])
                {
                    if (decisions[i] == null && state.GetUnscannedFish(1 - playerId).Contains(fish.Id))
                    {
                        var dec = new List<Decision>()
                        {
                            new SearchDecision(enemyDrones[0].Id, fish.Id),
                            new SearchDecision(enemyDrones[1].Id, fish.Id),
                            new KickAwayDecision(drones[i].Id, fish.Id)
                        };
                        GameUtils.GetDistance(state, dec, out var dIds);
                        if (dIds.Count == 1 && dIds[0] == drones[i].Id)
                        {
                            decisions[i] = new KickAwayDecision(drones[i].Id, fish.Id);
                            break;
                        }
                    }
                    if (state.GetUnscannedFish(playerId).Contains(fish.Id))
                    {
                        if (decisions[i] == null)
                        {
                            var dec = new List<Decision>()
                            {
                                new KickAwayDecision(enemyDrones[0].Id, fish.Id),
                                new KickAwayDecision(enemyDrones[1].Id, fish.Id),
                                new SearchDecision(drones[i].Id, fish.Id)
                            };
                            GameUtils.GetDistance(state, dec, out var dIds);
                            if (dIds.Contains(drones[i].Id))
                                decisions[i] = new SearchDecision(drones[i].Id, fish.Id);
                        }
                        else
                        {
                            (decisions[i] as SearchDecision).NextFishId = fish.Id;
                            break;
                        }
                    }
                }
                if (decisions[i] == null)
                    foreach (var fish in fishes[i])
                        if (state.GetUnscannedFish(playerId).Contains(fish.Id))
                            decisions[i] = new SearchDecision(drones[i].Id, fish.Id);
                if (decisions[i] == null) decisions[i] = new SaveDecision(drones[i].Id);
            }

            // Get variants
            decisionsList.Add(decisions);
            if (drones[0].Scans.Any() && drones[1].Scans.Any())
                decisionsList.Add(new Decision[] { new SaveDecision(drones[0].Id), new SaveDecision(drones[1].Id) });
            if (!(decisions[0] is SaveDecision || decisions[1] is SaveDecision))
            {
                if (drones[0].Scans.Any()) decisionsList.Add(new Decision[] { new SaveDecision(drones[0].Id), decisions[1] });
                if (drones[1].Scans.Any()) decisionsList.Add(new Decision[] { decisions[0], new SaveDecision(drones[1].Id) });
            }

            return decisionsList.ToArray();
        }

        private int GetReward(GameState state, Fish fish)
        {
            var score = 0;

            for (int playerId = 0; playerId < 2; playerId++)
                if (state.GetUnscannedFish(playerId).Contains(fish.Id))
                {
                    score += GameProperties.REWARDS[fish.Type];

                    var fishes = state.LostedFishes.Where(f => !state.GetScannedFish(playerId).Contains(f.Id));
                    if (!fishes.Any(f => f.Type == fish.Type)) score += GameProperties.REWARDS_TYPE;
                    if (!fishes.Any(f => f.Color == fish.Color)) score += GameProperties.REWARDS_COLOR;
                }

            return score;
        }
    }
}
