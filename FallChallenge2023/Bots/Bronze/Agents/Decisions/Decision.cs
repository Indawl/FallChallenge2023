﻿using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze.Agents.Decisions
{
    public class Decision
    {
        protected DroneAgent Agent { get; }
        protected GameState State { get; }

        public List<Condition> Conditions { get; } = new List<Condition>();
        public List<Decision> Decisions { get; }

        public Decision(DroneAgent agent, GameState state)
        {
            Agent = agent;
            State = state;
        }
        public virtual GameAction GetDecision() => null;
    }
}
