using DevLib.Game;
using System;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze.Actions
{
    public class GameActionList : GameAction
    {
        public List<IGameAction> Actions { get; } = new List<IGameAction>();

        public GameActionList() : base(GameActionType.LIST)
        {
        }

        public GameActionList(IGameAction action) : base(GameActionType.LIST)
        {
            Actions.Add(action);
        }

        public override string ToString() => string.Join(Environment.NewLine, Actions);
    }
}
