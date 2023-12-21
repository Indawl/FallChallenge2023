﻿using DevLib.GameMath;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public class Fish
    {
        public const int SPEED = 200;
        public const int FRIGHTENED_SPEED = 400;
        public const int MIN_DISTANCE_BT_FISH = 600;        
        public const int MONSTER_SPEED = 270;
        public const int MONSTER_ATTACK_SPEED = 540;
        public const int MONSTER_ATTACK_RADIUS = 500;
        public const int MIN_DISTANCE_BT_MONSTER = 600;
        public const int MONSTER_MIN_START_Y = 5000;

        public static Dictionary<FishType, Vector> HABITAT = new Dictionary<FishType, Vector>()
        {
            { FishType.ANGLER, new Vector(2500, 10000) },
            { FishType.JELLY, new Vector(2500, 5000) },
            { FishType.FISH, new Vector(5000, 7500) },
            { FishType.CRAB, new Vector(7500, 10000) }
        };

        public int Id { get; }
        public FishColor Color { get; }
        public FishType Type { get; }

        public Vector Position { get; set; }
        public Vector Speed { get; set; }

        public bool Lost { get; set; }

        public Fish(int id, FishColor color, FishType type)
        {
            Id = id;
            Color = color;
            Type = type;
        }

        public override string ToString() => string.Format("[{0}] {1} {2} P{3} S{4}", Id, Color, Type, Position?.ToIntString(), Speed?.ToIntString());

        public void CalculateLocation()
        {
            Position += Speed;

            // Left Border
            if (Position.X < 0)
            {
                Position = Position.HSymmetric();
                Speed.X = -Speed.X;
            }
            // Right Border
            if (Position.X > GameState.MAP_SIZE)
            {
                Position = Position.HSymmetric(GameState.MAP_SIZE);
                Speed.X = -Speed.X;
            }

            var h = HABITAT[Type];
            // Top Border
            if (Position.Y < h.X)
            {
                Position = Position.VSymmetric(h.X);
                Speed.Y = -Speed.Y;
            }
            // Bottom Border
            if (Position.Y > h.Y)
            {
                Position = Position.VSymmetric(h.Y);
                Speed.Y = -Speed.Y;
            }
        }
    }
}
