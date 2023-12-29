using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Linq;

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

        public static Vector GetAroundMonsterTo(GameState state, Vector from, Vector to, int? droneId = null, double epsilon = GameProperties.MONSTER_TRAVERSAL_ANLE)
        {
            var speed = to - from;
            if (speed.Length() > GameProperties.DRONE_MAX_SPEED) speed = (speed.Normalize() * GameProperties.DRONE_MAX_SPEED).Round();
            return GetAroundMonster(state, from, speed, droneId, epsilon);
        }

        public static Vector GetAroundMonster(GameState state, Vector from, Vector speed, int? droneId = null, double epsilon = GameProperties.MONSTER_TRAVERSAL_ANLE)
        {
            if (CheckCollisionWithMonsters(state, from, ref speed, droneId, epsilon, GameProperties.MONSTER_TRAVERSAL_TURNS)) return from;
            else return from + speed;
        }

        public static bool CheckCollision(Vector fishPostion, Vector fishSpeed, Vector droneFrom, Vector droneTo)
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

        private static bool CheckCollisionWithMonsters(GameState state, Vector from, ref Vector speed, int? droneId = null, double epsilon = 0.1, int forMoves = 0)
        {
            epsilon *= Math.PI / 180;

            var newTo = from + speed;
            var newSpeed = speed.IsZero() ? new Vector(GameProperties.DRONE_MAX_SPEED, 0) : speed;

            var alpha = 0.0;
            var wise = true;
            var collision = true;

            while (collision)
            {
                if (alpha > Math.PI) return true;

                while (collision)
                {
                    collision = false;

                    foreach (var fish in state.Fishes.Where(_ => _.Color == FishColor.UGLY && _.Speed != null))
                        while (CheckCollision(fish.Position, fish.Speed, from, newTo))
                        {
                            alpha = (wise ? epsilon : 0.0) - alpha;
                            wise = !wise;
                            if (alpha > Math.PI) return true;

                            var rSpeed = (newSpeed.Rotate(alpha).Round().Normalize() * GameProperties.DRONE_MAX_SPEED).Round();
                            newTo = SnapToDroneZone(from + rSpeed);
                            collision = true;
                        }
                }

                // Check next turn
                if (forMoves > 0)
                {
                    var referee = new GameReferee((GameState)state.Clone());
                    if (droneId != null) referee.State.Drones.First(_ => _.Id == droneId.Value).Position = newTo;
                    referee.UpdateFishPositions(_ => _.Color == FishColor.UGLY);

                    var nextSpeed = newTo - from;
                    if (CheckCollisionWithMonsters(referee.State, newTo, ref nextSpeed, droneId, GameProperties.MONSTER_TRAVERSAL_ANLE_FAST, forMoves - 1))
                    {
                        alpha = (wise ? epsilon : 0.0) - alpha;
                        wise = !wise;
                        newTo = SnapToDroneZone(from + newSpeed.Rotate(alpha).Round());
                        collision = true;
                    }
                }
            }

            speed = newTo - from;
            return false;
        }
    }
}
