using DebugUtils.Objects;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugFish : DebugObject
    {
        public DebugFish(string name, Fish fish, DebugObject parent = null) : base(name, parent)
        {
            Position = new Rectangle((int)fish.Position.X - 300, (int)fish.Position.Y - 300, 600, 600);
        }
    }
}
