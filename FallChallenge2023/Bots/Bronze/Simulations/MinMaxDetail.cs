namespace FallChallenge2023.Bots.Bronze.Simulations
{
    public class MinMaxDetail
    {
        public int Depth { get; set; }
        public int Evaluation { get; set; }        
        public MinMaxVariant[] Variants { get; set; }

        public MinMaxVariant BestVariant { get; set; }
    }
}
