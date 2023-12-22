using DebugUtils.Objects;
using FallChallenge2023.Bots.Bronze.Actions;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugAction : DebugObject
    {
        public GameAction Action { get; set; }
        public Point Move { get; set; }

        public DebugAction(GameAction action, DebugObject parent = null) : base(action.ToString(), parent)
        {
            Action = action;

            if (Action.Type == GameActionType.MOVE)
            {
                var toX = (int)(Parent.Parent.Position.Width * Action.Position.X / GameState.MAP_SIZE);
                var toY = (int)(Parent.Parent.Position.Height * Action.Position.Y / GameState.MAP_SIZE);
                Move = new Point(toX - Parent.Position.X, toY - Parent.Position.Y);
            }

            Position = new Rectangle(0, 0, Parent.Position.Width, Parent.Position.Height);
            Visible = true;

            Properties.Add("DroneId", Action.DroneId);
            Properties.Add("Action", Action.ToString());
        }

        public override Bitmap GetFigure()
        {
            var action = new Bitmap(Position.Width, Position.Height);
            var g = Graphics.FromImage(action);

            var debugDrone = Parent as DebugDrone;

            if (Action.Type == GameActionType.MOVE) g.DrawLine(new Pen(Color.Black, 2.0f), Position.Width / 2, Position.Height / 2, Move.X, Move.Y);
            if (Action.Light) g.DrawEllipse(new Pen(Color.White, 4.0f), 
                Position.Width / 2 - debugDrone.MaxLightRadius, Position.Height / 2 - debugDrone.MaxLightRadius,
                2 * debugDrone.MaxLightRadius, 2 * debugDrone.MaxLightRadius);

            return action;
        }
    }
}
