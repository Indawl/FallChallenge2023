using DebugUtils.Objects;
using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System.Drawing;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugAction : DebugObject
    {
        public GameAction Action { get; set; }
        public Vector Move { get; set; }

        public DebugAction(GameAction action, DebugObject parent) : base(action.ToString(), parent)
        {
            Action = action;

            if (Action.Type == GameActionType.MOVE)
                Move = new Vector(Parent.Position.Width * (Action as GameActionMove).Position.X / GameProperties.MAP_SIZE, 
                                  Parent.Position.Height * (Action as GameActionMove).Position.Y / GameProperties.MAP_SIZE);

            Position = Parent.Position;
            Visible = true;

            Properties.Add("Action", Action);
        }

        public override Bitmap GetFigure()
        {
            var action = new Bitmap(Position.Width, Position.Height);
            var g = Graphics.FromImage(action);

            var debugDrone = Parent as DebugDrone;

            if (Action.Type == GameActionType.MOVE) g.DrawLine(new Pen(Color.Black, 2.0f), (int)debugDrone.Coord.X, (int)debugDrone.Coord.Y, (int)Move.X, (int)Move.Y);
            if (Action is GameActionMove && (Action as GameActionMove).Light || Action is GameActionWait && (Action as GameActionWait).Light)
                g.DrawEllipse(new Pen(Color.White, 4.0f),
                    (int)debugDrone.Coord.X - debugDrone.MaxLightRadius, (int)debugDrone.Coord.Y - debugDrone.MaxLightRadius,
                    2 * debugDrone.MaxLightRadius, 2 * debugDrone.MaxLightRadius);

            return action;
        }
    }
}
