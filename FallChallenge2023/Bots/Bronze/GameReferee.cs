﻿namespace FallChallenge2023.Bots.Bronze
{
    public class GameReferee
    {
        public void UpdateFishPositions()
        {
            UpdateFishPositions(Fishes);
        }

        public void UpdateFishPositions(Func<Fish, bool> predicate)
        {
            UpdateFishPositions(Fishes.Where(predicate));
        }


        private void UpdateFishPositions(IEnumerable<Fish> fishes)
        {
            // New Position
            foreach (var fish in fishes.Where(_ => _.Position != null && _.Speed != null))
                fish.Position = GetNextPosition(fish.Type, fish.Position, fish.Speed);

            // New Speed
            foreach (var fish in fishes.Where(_ => _.Position != null && _.Speed != null))
                fish.Speed = GetFishSpeed(fish);
        }

        private Vector GetNextPosition(FishType type, Vector position, Vector speed) => GameUtils.SnapToFishZone(type, position + speed);

        private Vector GetFishSpeed(Fish fish) => fish.Color == FishColor.UGLY ?
            GetUglySpeed(fish.Id, fish.Position, fish.Speed) :
            GetFishSpeed(fish.Id, fish.Type, fish.Position, fish.Speed);

        private Vector GetFishSpeed(int fishId, FishType type, Vector position, Vector speed)
        {
            // Near drone
            var dronePositions = position.GetClosest(Drones
                .Where(_ => !_.Emergency && _.Position.InRange(position, GameProperties.MOTOR_RANGE))
                .Select(_ => _.Position)
                .ToList());
            if (dronePositions.Any())
            {
                var pos = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                speed = ((position - pos).Normalize() * GameProperties.FISH_FRIGHTENED_SPEED).Round();
            }
            else
            {
                // Near fish
                var fishesPositions = position.GetClosest(Fishes
                    .Where(_ => _.Position != null && _.Speed != null && _.Id != fishId && _.Color != FishColor.UGLY && _.Position.InRange(position, GameProperties.MIN_DISTANCE_BT_FISH))
                    .Select(_ => _.Position)
                    .ToList());
                if (fishesPositions.Any())
                {
                    var pos = fishesPositions.Aggregate((a, b) => a + b) / fishesPositions.Count;
                    speed = (position - pos).Normalize() * GameProperties.FISH_SPEED;
                }
                else speed = speed.Normalize() * GameProperties.FISH_SPEED;

                // Border
                var nextPosition = position + speed;

                var habitat = GameProperties.HABITAT[type];
                if (nextPosition.X < 0 || nextPosition.X > GameProperties.MAP_SIZE - 1) speed = speed.HSymmetric();
                if (nextPosition.Y < habitat[0] || nextPosition.Y > habitat[1]) speed = speed.VSymmetric();

                speed = speed.EpsilonRound().Round();
            }

            return speed;
        }

        private Vector GetUglySpeed(int fishId, Vector position, Vector speed)
        {
            // Near drone
            var dronePositions = position.GetClosest(Drones
                .Where(_ => !_.Emergency && _.Position.InRange(position, _.LightRadius))
                .Select(_ => _.Position)
                .ToList());
            if (dronePositions.Any())
            {
                var pos = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                speed = ((pos - position).Normalize() * GameProperties.MONSTER_ATTACK_SPEED).Round();
            }
            else
            {
                if (speed.Length() > GameProperties.MONSTER_SPEED)
                    speed = (speed.Normalize() * GameProperties.MONSTER_SPEED).Round();

                // Near other ugly
                var fishesPositions = position.GetClosest(Fishes
                    .Where(_ => _.Position != null && _.Speed != null && _.Id != fishId && _.Color == FishColor.UGLY && _.Position.InRange(position, GameProperties.MIN_DISTANCE_BT_MONSTER))
                    .Select(_ => _.Position)
                    .ToList());
                if (fishesPositions.Any())
                {
                    var pos = fishesPositions.Aggregate((a, b) => a + b) / fishesPositions.Count;
                    speed = ((position - pos).Normalize() * GameProperties.FISH_SPEED).Round(); // Fish.MONSTER_SPEED; // repeat error from referee
                }

                // Border
                var nextPosition = position + speed;

                var habitat = GameProperties.HABITAT[FishType.ANGLER];
                if (nextPosition.X < 0 || nextPosition.X > GameProperties.MAP_SIZE - 1) speed = speed.HSymmetric();
                if (nextPosition.Y < habitat[0] || nextPosition.Y > habitat[1]) speed = speed.VSymmetric();
            }

            return speed;
        }
    }
}