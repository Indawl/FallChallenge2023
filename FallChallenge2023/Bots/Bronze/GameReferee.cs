using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze
{
    public class GameReferee
    {
        public GameState State { get; }

        public GameReferee(GameState state)
        {
            State = state;
        }

        public void UpdateFishs(bool losted = false)
        {
            UpdateFishs(State.SwimmingFishes, losted);
        }

        public void UpdateFishs(Func<Fish, bool> predicate, bool losted = false)
        {
            UpdateFishs(State.SwimmingFishes.Where(predicate), losted);
        }

        private void UpdateFishs(IEnumerable<Fish> fishes, bool losted = false)
        {
            // New Position
            UpdatePositions(fishes, losted);

            // New Speed
            UpdateSpeeds(fishes);
        }

        private void UpdatePositions(IEnumerable<Fish> fishes, bool losted = false)
        {
            foreach (var fish in fishes.Where(_ => _.Speed != null).ToList())
            {
                fish.Position = fish.Position + fish.Speed;

                if (losted && fish.Type != FishType.ANGLER && (fish.Position.X < 0 || fish.Position.X > GameProperties.MAP_SIZE - 1))
                    State.FishLosted(fish);
                else fish.Position = GameUtils.SnapToFishZone(fish.Type, fish.Position);
            }
        }

        private void UpdateSpeeds(IEnumerable<Fish> fishes)
        {
            foreach (var fish in fishes.Where(_ => _.Speed != null))
                fish.Speed = GetFishSpeed(fish);
        }

        private Vector GetFishSpeed(Fish fish) => fish.Type == FishType.ANGLER ?
            GetUglySpeed(fish.Id, fish.Position, fish.Speed) :
            GetFishSpeed(fish.Id, fish.Type, fish.Position, fish.Speed);

        private Vector GetFishSpeed(int fishId, FishType type, Vector position, Vector speed)
        {
            // Near drone
            var dronePositions = position.GetClosest(State.Drones
                .Where(drone => !drone.Emergency && drone.Position.InRange(position, GameProperties.MOTOR_RANGE))
                .Select(drone => drone.Position)
                .ToList());
            if (dronePositions.Any())
            {
                var pos = dronePositions.Aggregate((a, b) => a + b) / dronePositions.Count;
                speed = ((position - pos).Normalize() * GameProperties.FISH_FRIGHTENED_SPEED).Round();
            }
            else
            {
                // Near fish
                var fishesPositions = position.GetClosest(State.Fishes
                    .Where(fish => fish.Speed != null && fish.Id != fishId && fish.Position.InRange(position, GameProperties.MIN_DISTANCE_BT_FISH))
                    .Select(fish => fish.Position)
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
            var dronePositions = position.GetClosest(State.Drones
                .Where(drone => !drone.Emergency && drone.Position.InRange(position, drone.LightRadius))
                .Select(drone => drone.Position)
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
                var fishesPositions = position.GetClosest(State.Monsters
                    .Where(fish => fish.Speed != null && fish.Id != fishId && fish.Position.InRange(position, GameProperties.MIN_DISTANCE_BT_MONSTER))
                    .Select(fish => fish.Position)
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
