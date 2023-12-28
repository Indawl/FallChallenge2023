using FallChallenge2023.Bots.Bronze.Agents;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public static class GameUtils
    {
        public static Vector SnapToDroneZone(Vector position)
        {
            var newPosition = position;

            if (position.X < 0) newPosition = new Vector(0, position.Y);
            if (position.X > GameProperties.MAP_SIZE - 1) newPosition = new Vector(GameProperties.MAP_SIZE - 1, position.Y);
            if (position.Y < 0) newPosition = new Vector(position.X, 0);
            if (position.Y > GameProperties.MAP_SIZE - 1) newPosition = new Vector(position.X, GameProperties.MAP_SIZE - 1);

            return newPosition;
        }

        public static Vector SnapToFishZone(FishType type, Vector position)
        {
            var newPosition = position;

            var habitat = GameProperties.HABITAT[type];
            if (position.X < 0) newPosition = new Vector(0, position.Y);
            if (position.X > GameProperties.MAP_SIZE - 1) newPosition = new Vector(GameProperties.MAP_SIZE - 1, position.Y);
            if (position.Y < habitat[0]) newPosition = new Vector(position.X, habitat[0]);
            if (position.Y > habitat[1]) newPosition = new Vector(position.X, habitat[1]);

            return newPosition;
        }

        private static bool CheckCollision(Vector fishPostion, Vector fishSpeed, Vector droneFrom, Vector droneTo)
        {
            if (fishSpeed.IsZero() && droneTo.Equals(droneFrom)) return false;

            var pos = fishPostion - droneFrom;
            var vd = droneTo - droneFrom;
            if (vd.Equals(fishSpeed)) return false;

            var vf = fishSpeed - vd;
            var a = vf.LengthSqr();
            var b = 2.0 * Vector.Dot(pos, vf);
            var c = pos.LengthSqr() - GameProperties.MONSTER_ATTACK_RADIUS_SQR;
            var delta = b * b - 4.0 * a * c;
            if (delta < 0.0) return false;

            double t = (-b - Math.Sqrt(delta)) / (2.0 * a);

            if (t <= 0.0 || t > 1.0) return false;

            return true;
        }

        public static int Simulation(GameState state, List<DroneAgent> agents)
        {
            var newState = state;
            var bot = new SimulationBot(agents);

            while (!newState.IsGameOver())
            {
                newState = (GameState)state.Clone();
                newState.Turn++;
                bot.UpdateDronePosition(newState);
                newState.UpdateFishPositions(Fishes, Drones);
                newState.CheckDrones();
                newState.DoScans();
                newState.DoSave();
            }

            // In the end of game, scan all fishes
            foreach (var drone in newState.Drones)
                drone.Position = new Vector();
            newState.DoSave();

            return newState.Score;
        }
    }
}
