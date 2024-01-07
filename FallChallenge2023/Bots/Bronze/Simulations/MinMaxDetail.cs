namespace FallChallenge2023.Bots.Bronze.Simulations
{
    public class MinMaxDetail
    {
        public int Depth { get; set; }
        public int Evaluation { get; set; }        
        public MinMaxVariant[] Variants { get; set; }

        public MinMaxVariant BestVariant { get; set; }

        public override string ToString() => string.Format("Evaluation({0}): {1}", Depth, Evaluation);
    }
}
