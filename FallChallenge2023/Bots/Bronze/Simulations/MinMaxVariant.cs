using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Simulations
{
    public class MinMaxVariant
    {
        public Decision[] Decisions { get; }
        public GameState State { get; set; }
        public GameActionList Action => new GameActionList(Decisions.Select(_ => _.Action).ToList());

        public MinMaxVariant(Decision[] decisions)
        {
            Decisions = decisions;
        }
    }
}
