﻿using DebugUtils;
using DebugUtils.Objects;
using DevLib.Game;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugBot : Bot, IDebugBot
    {
        public GameManager Manager { get; }

        public DebugBot(GameManager manager)
        {
            Manager = manager;
        }

        public void GetDebugAction(IGameState state, ref DebugObject debugObject)
        {
            var gameState = (GameState)(state as GameState).Clone();

            // Get actions
            GetAction(gameState);

            // State
            debugObject = Manager.GetObjectFromState(gameState);

            // Simulation
            var simulationObject = new DebugObject("Simulation", debugObject);
            debugObject.Childs.Add(simulationObject);

            simulationObject.Properties.Add(string.Format("Depth: {0}", Simultation.Depth), new SimulationDebugState(Simultation));

            // Ocean
            var oceanFloor = debugObject.Childs.First(_ => _.Name == "Ocean Floor");

            // Actions
            foreach (var decision in Simultation.GetStateDetails(Simultation.Referee.State).BestVariant.Decisions)
            {
                var debugDrone = oceanFloor.Childs.Where(_ => _ is DebugDrone).First(_ => (_ as DebugDrone).Drone.Id == decision.DroneId);
                debugDrone.Childs.Add(new DebugAction(decision.Action, debugDrone));
            }
        }
    }
}
