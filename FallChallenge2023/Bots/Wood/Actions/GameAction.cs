﻿using DevLib.Game;
using DevLib.GameMath;

namespace FallChallenge2023.Bots.Wood.Actions
{
    public class GameAction : IGameAction
    {
        public GameActionType Type { get; }
        public Vector Position { get; }
        public bool Light { get; }

        public GameAction(GameActionType type, bool light = false, Vector position = null)
        {
            Type = type;
            Position = position;
            Light = light;
        }
    }
}
