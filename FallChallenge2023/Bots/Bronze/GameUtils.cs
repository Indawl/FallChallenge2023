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

        public static Vector GetAroundMonsterTo(GameState state, Vector from, Vector to, int forMoves = GameProperties.MONSTER_TRAVERSAL_TURNS, double epsilon = GameProperties.MONSTER_TRAVERSAL_ANGLE)
        {
            CheckCollisionWithMonsters(state, from, to, out var speed, forMoves, epsilon);
            return from + speed;
        }

        public static Vector GetAroundMonster(GameState state, Vector from, Vector speed, int forMoves = GameProperties.MONSTER_TRAVERSAL_TURNS, double epsilon = GameProperties.MONSTER_TRAVERSAL_ANGLE)
            =>  GetAroundMonsterTo(state, from, from + speed, forMoves, epsilon);

        public static bool CheckCollision(Vector fishPosition, Vector fishSpeed, Vector droneFrom, Vector droneTo, bool anytime = false)
        {
            var droneSpeed = droneTo - droneFrom;
            if (droneSpeed.Equals(fishSpeed)) return false;

            var relativeFishPosition = fishPosition - droneFrom;
            var relativeFishSpeed = fishSpeed - droneSpeed;

            var a = -relativeFishSpeed.LengthSqr();
            var b = Vector.Dot(relativeFishPosition, relativeFishSpeed);
            var c = relativeFishPosition.LengthSqr() - GameProperties.MONSTER_ATTACK_RADIUS * GameProperties.MONSTER_ATTACK_RADIUS;
            var delta = b * b + a * c;
            if (delta < 0.0) return false;

            double t = (b + Math.Sqrt(delta)) / a;

            if (t <= 0.0 || !anytime && t > 1.0) return false;

            return true;
        }

        private static bool CheckCollisionWithMonsters(GameState state, Vector from, Vector moveTo, out Vector speed, int forMoves = 0, double epsilon = 0.1)
        {
            epsilon *= Math.PI / 180;
            speed = ((moveTo - from).Normalize() * GameProperties.DRONE_MAX_SPEED).Round();

            var newSpeed = from.Equals(moveTo) ? ((GameProperties.CENTER - from).Normalize() * GameProperties.DRONE_MAX_SPEED).Round() : moveTo - from;
            var newTo = from + speed;

            var alpha = 0.0;
            var wise = true;
            var collision = true;

            while (collision)
            {
                while (collision)
                {
                    collision = false;
                    
                    foreach (var fish in state.Monsters.Where(_ => _.Speed != null))
                        while (CheckCollision(fish.Position, fish.Speed, from, newTo))
                        {
                            alpha = (wise ? epsilon : 0.0) - alpha;
                            if (alpha > Math.PI) return true;

                            wise = !wise;                            
                            collision = true;

                            speed = (newSpeed.Rotate(alpha).Round().Normalize() * GameProperties.DRONE_MAX_SPEED).Round();
                            newTo = SnapToDroneZone(from + speed);
                            speed = newTo - from;
                        }
                }

                // Check next turn
                if (forMoves > 0)
                {
                    var referee = new GameReferee(new GameState());
                    GameState.CloneFishes(state.Monsters, referee.State.Monsters);
                    referee.State.MyDrones.Add(new Drone() { Position = newTo });
                    referee.UpdateFishs();

                    if (CheckCollisionWithMonsters(referee.State, newTo, moveTo, out var nextSpeed, forMoves - 1, GameProperties.MONSTER_TRAVERSAL_ANGLE_FAST))
                    {
                        alpha = (wise ? epsilon : 0.0) - alpha;
                        if (alpha > Math.PI) return true;

                        wise = !wise;
                        collision = true;

                        nextSpeed = (newSpeed.Rotate(alpha).Round().Normalize() * GameProperties.DRONE_MAX_SPEED).Round();
                        newTo = SnapToDroneZone(from + nextSpeed);
                    }
                }

                // No way
                if (alpha > Math.PI) return true;
            }

            return false;
        }

        public static int GetDistance(GameState state, Vector from, Vector to)
        {
            var drone = new Drone() { Position = from };
            var referee = new GameReferee(new GameState());
            GameState.CloneFishes(state.Monsters, referee.State.Monsters);
            referee.State.MyDrones.Add(drone);

            int distance;
            for (distance = 0; !to.Equals(drone.Position) && distance < 30; distance++)
            {
                drone.Position = GetAroundMonsterTo(state, from, to, 0);
                referee.UpdateFishs();
            }

            return distance;
        }

        public static int GetDistance(GameState state, Vector from, int top = GameProperties.SURFACE)
        {
            var to = new Vector(0, top);
            var drone = new Drone() { Position = from };
            var referee = new GameReferee(new GameState());
            GameState.CloneFishes(state.Monsters, referee.State.Monsters);
            referee.State.MyDrones.Add(drone);

            int distance;
            for (distance = 0; drone.Position.Y > top && distance < 30; distance++)
            {
                drone.Position = GetAroundMonsterTo(state, from, to, 0);
                referee.UpdateFishs();
            }

            return distance;
        }
    }
}
