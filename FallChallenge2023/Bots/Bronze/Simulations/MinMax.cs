using DevLib.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FallChallenge2023.Bots.Bronze.Simulations
{
    public class MinMax
    {
        protected IGameReferee Referee { get; }

        public Stopwatch StopWatch { get; set; }
        public long TimeOutTime { get; set; }
        public long TimeOutDelta { get; set; }

        public int Depth { get; set; }

        public Dictionary<IGameState, MinMaxDetail> StateBuffer { get; } = new Dictionary<IGameState, MinMaxDetail>();

        protected virtual int Evaluation(IGameState sourceState) => 0;
        protected virtual IGameAction[] GetActions(IGameState sourceState) => Referee.GetActions(sourceState);

        public MinMax(IGameReferee referee)
        {
            Referee = referee;
        }

        public void FindBestAction(IGameState state)
        {
            try
            {
                Simulation(state, Depth);
            }
            catch (TimeoutException) { }
        }

        public MinMaxDetail GetStateDetails(IGameState state)
        {
            if (!StateBuffer.TryGetValue(state, out var detail))
                StateBuffer.Add(state, detail = new MinMaxDetail());
            return detail;
        }

        protected void CheckTimeOut()
        {
            if (TimeOutTime > 0 && StopWatch.ElapsedMilliseconds > TimeOutTime - TimeOutDelta)
                throw new TimeoutException();
        }

        private int Simulation(IGameState state, int depth, int alpha = -int.MaxValue, int beta = int.MaxValue)
        {
            // Time breaker
            CheckTimeOut();

            // Check buffer
            var details = GetStateDetails(state);
            if (details.Depth > 0 && details.Depth >= depth) return details.Evaluation;
            details.Depth = depth;

            // Time to Evaluation
            if (Referee.IsOver(state) || depth < 1) return Evaluation(state);

            // Get edges
            if (details.Variants == null)
                details.Variants = GetVariants(state);

            // MinMax
            details.Evaluation = alpha;

            for (int i = 0; i < details.Variants.Length; i++)
            {
                // Initialize New State
                if (details.Variants[i].State == null)
                    details.Variants[i].State = Referee.GetNext(state, details.Variants[i].Action);

                // Evaluation
                int evaluation = -Simulation(details.Variants[i].State, depth - 1, -beta, -details.Evaluation);
                if (evaluation > details.Evaluation)
                {
                    details.Evaluation = evaluation;
                    details.BestVariant = details.Variants[i];
                    if (details.Evaluation >= beta) break;
                }
            }

            return details.Evaluation;
        }

        private MinMaxVariant[] GetVariants(IGameState state)
        {
            var actions = GetActions(state);
            var variants = new MinMaxVariant[actions.Length];

            for (int i = 0; i < actions.Length; i++)
                variants[i] = new MinMaxVariant(actions[i]);

            return variants;
        }
    }
}
