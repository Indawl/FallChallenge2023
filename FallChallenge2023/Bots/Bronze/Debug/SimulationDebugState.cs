using DebugUtils.Objects;
using DebugUtils.Controls.Properties;
using FallChallenge2023.Bots.Bronze.Simulations;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class SimulationDebugState : DebugState, IPropertiesDetailsAction
    {
        private MinMax Simultation { get; }

        public SimulationDebugState(MinMax simultation) : 
            base(simultation.GetStateDetails(simultation.Referee.State).BestVariant.ToString(), simultation.Referee.State)
        {
            Simultation = simultation;

            AddVariant(this);
        }

        private void AddVariant(DebugState debugState)
        {
            var best = Simultation.GetStateDetails((GameState)debugState.State);

            var root = new DebugState(best.BestVariant.ToString(), best.BestVariant.State, debugState);
            debugState.NextMoves.Add(root);
            AddVariant(root);

            foreach (var variant in best.Variants)
                if (variant != best.BestVariant)
                {
                    var alt = new DebugState(variant.ToString(), variant.State, debugState);
                    debugState.NextMoves.Add(alt);
                    AddVariant(alt);
                }
        }

        public void GetDetails()
        {
            var manager = new GameManager(this);
            manager.Show();
        }

        public override string ToString() => Simultation.GetStateDetails(Simultation.Referee.State).BestVariant.ToString();
    }
}
