using System;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameActionList : GameAction
    {
        public List<GameAction> Actions { get; } = new List<GameAction>();

        public GameActionList() : base(GameActionType.LIST)
        {
        }

        public GameActionList(GameAction action) : base(GameActionType.LIST)
        {
            Actions.Add(action);
        }

        public GameActionList(List<GameAction> actions) : base(GameActionType.LIST)
        {
            Actions = actions;
        }

        public override string ToString() => string.Join(Environment.NewLine, Actions);
    }
}
