﻿using FallChallenge2023.Bots.Bronze.GameMath;
using System.Collections.Generic;

namespace FallChallenge2023.Bots.Bronze
{
    public static class GameProperties
    {
        public const int SERIAL_FROM_TURN = 0;
        public const int LAST_TURN = 200;

        public const int MAP_SIZE = 10000;
        public const int SURFACE = 500;
        public static Vector CENTER = new Vector((MAP_SIZE - 1) / 2.0, (MAP_SIZE - 1) / 2.0);

        public const int FISH_SPEED = 200;
        public const int FISH_SPEED_SQR_DELTA = (FISH_SPEED + 10) * (FISH_SPEED + 10);
        public const int FISH_FRIGHTENED_SPEED = 400;

        public const int MONSTER_SPEED = 270;
        public const int MONSTER_ATTACK_SPEED = 540;
        public const int MONSTER_ATTACK_RADIUS = 500;
        public const int MONSTER_ATTACK_RADIUS_SQR = MONSTER_ATTACK_RADIUS * MONSTER_ATTACK_RADIUS;

        public const int DRONE_MAX_SPEED = 600;
        public const int DRONE_SINK_SPEED = 300;
        public const int DRONE_EMERGENCY_SPEED = 300;

        public const int DARK_SCAN_RADIUS = 800;
        public const int LIGHT_SCAN_RADIUS = 2000;
        public const int MONSTER_DETECTED_RADIUS_ADD = 300;
        public const int MOTOR_RANGE = 1400;

        public const int BATTERY_DRAIN = 5;
        public const int BATTERY_RECHARGE = 1;
        public const int MAX_BATTERY = 30;
        
        public const int MIN_DISTANCE_BT_FISH = 600;
        public const int MIN_DISTANCE_BT_MONSTER = 600;        
        public const int MONSTER_MIN_START_Y = 5000;

        public const double MONSTER_TRAVERSAL_ANGLE = 0.1;
        public const double MONSTER_TRAVERSAL_ANGLE_FAST = 20.0;
        public const int MONSTER_TRAVERSAL_TURNS = 5;

        public static Dictionary<FishType, int[]> HABITAT = new Dictionary<FishType, int[]>()
        {
            { FishType.ANGLER, new int[] { 2500, 9999 } },
            { FishType.JELLY, new int[] { 2500, 4999 } },
            { FishType.FISH, new int[] { 5000, 7499 } },
            { FishType.CRAB, new int[] { 7500, 9999 } }
        };

        public static Dictionary<FishType, int> REWARDS = new Dictionary<FishType, int>()
        {
            { FishType.JELLY, 1 },
            { FishType.FISH, 2 },
            { FishType.CRAB, 3 },
            { FishType.ONE_COLOR, 3 },
            { FishType.ONE_TYPE, 4 }
        };

        public static List<FishType> TYPES = new List<FishType>()
        {
            FishType.JELLY,
            FishType.FISH,
            FishType.CRAB
        };

        public static List<FishColor> COLORS = new List<FishColor>()
        {
            FishColor.PINK,
            FishColor.YELLOW,
            FishColor.GREEN,
            FishColor.BLUE
        };

        public const int EmergencyCondition = 1;
        public const int DiveCondition = 2;
        public const int EarlySaveCondition = 3;
        public const int SearchCrabCondition = 4;        
        public const int SearchCondition = 5;
    }
}
