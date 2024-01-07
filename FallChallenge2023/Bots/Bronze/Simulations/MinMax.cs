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
            Referee.RemoveLostedFish();

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
            var decisions = new List<Decision[]>();

            decisions.Add(new Decision[] { new SaveDecision(), new SaveDecision() });

            return decisions.ToArray();
        }
    }
}
