﻿using FallChallenge2023.Bots.Bronze.GameMath;

namespace FallChallenge2023.Bots.Bronze
{
    public class RadarBlip
    {
        public int FishId { get; }
        public BlipType Type { get; }
        public BlipType LastType { get; set; }

        public RadarBlip(int fishId, BlipType type)
        {
            FishId = fishId;
            Type = type;
        }

        public static int GetDirectionX(BlipType type) => (type == BlipType.BR || type == BlipType.TR) ? 1 : -1;
        public static int GetDirectionY(BlipType type) => (type == BlipType.BL || type == BlipType.BR) ? 1 : -1;
        public static Vector GetDirection(BlipType type) => new Vector(GetDirectionX(type), GetDirectionY(type));

        public override string ToString() => string.Format("{0} {1}{2}", FishId, Type, (Type == LastType) ? string.Empty : string.Format("({0})", LastType));

        public RectangleRange GetRange(Vector position)
        {
            double x = 0, y = 0, toX = GameState.MAP_SIZE, toY = GameState.MAP_SIZE;

            if (Type == BlipType.BL || Type == BlipType.TL)
            {
                if (LastType == BlipType.BR || LastType == BlipType.TR) x = position.X;
                toX = position.X;
            }
            else
            {
                if (LastType == BlipType.BL || LastType == BlipType.TL) toX = position.X;
                x = position.X;
            }

            if (Type == BlipType.TL || Type == BlipType.TR)
            {
                if (LastType == BlipType.BL || LastType == BlipType.BR) y = position.Y;
                toY = position.Y;
            }
            else
            {
                if (LastType == BlipType.TL || LastType == BlipType.TR) toY = position.Y;
                y = position.Y;
            }

            return new RectangleRange(x, y, toX, toY);
        }
    }
}
