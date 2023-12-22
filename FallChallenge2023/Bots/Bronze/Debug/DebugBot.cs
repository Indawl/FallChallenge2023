using DebugUtils;
using DebugUtils.Objects;
using DevLib.Game;
using FallChallenge2023.Bots.Bronze.Actions;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Debug
{
    public class DebugBot : Bot, IDebugBot
    {
        public void GetDebugAction(IGameState state, DebugObject debugObject)
        {
            var gameState = (GameState)(state as GameState).Clone();

            var actions = (GameAction)GetAction(gameState);
            if (actions.Type != GameActionType.LIST) actions = new GameActionList(actions);

            // Ocean
            var oceanFloor = debugObject.Childs.First(_ => _.Name == "Ocean Floor");

            foreach (GameAction action in (actions as GameActionList).Actions)
            {
                var drone = gameState.Drones[action.DroneId];
                var debugDrone = oceanFloor.Childs.First(_ => _.Name == drone.ToString());

                debugDrone.Childs.Add(new DebugAction(action, debugDrone));
            }
        }
    }
}
