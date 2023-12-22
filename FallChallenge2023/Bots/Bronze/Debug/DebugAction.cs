using DebugUtils.Objects;
using FallChallenge2023.Bots.Bronze.Actions;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugAction : DebugObject
    {
        public GameAction Action { get; set; }
        public Point Move { get; set; }

        public DebugAction(GameAction action, DebugObject parent) : base(action.ToString(), parent)
        {
            Action = action;

            if (Action.Type == GameActionType.MOVE)
                Move = new Point(Parent.Position.Width * Action.X / GameState.MAP_SIZE, Parent.Position.Height * Action.Y / GameState.MAP_SIZE);

            Position = Parent.Position;
            Visible = true;

            Properties.Add("Action", Action.ToString());
        }

        public override Bitmap GetFigure()
        {
            var action = new Bitmap(Position.Width, Position.Height);
            var g = Graphics.FromImage(action);

            var debugDrone = Parent as DebugDrone;

            if (Action.Type == GameActionType.MOVE) g.DrawLine(new Pen(Color.Black, 2.0f), debugDrone.Coord.X, debugDrone.Coord.Y, Move.X, Move.Y);
            if (Action.Light) g.DrawEllipse(new Pen(Color.White, 4.0f),
                debugDrone.Coord.X - debugDrone.MaxLightRadius, debugDrone.Coord.X - debugDrone.MaxLightRadius,
                2 * debugDrone.MaxLightRadius, 2 * debugDrone.MaxLightRadius);

            return action;
        }
    }
}
