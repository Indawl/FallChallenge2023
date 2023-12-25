using FallChallenge2023.Bots.Bronze.Actions;
using FallChallenge2023.Bots.Bronze.Agents.Conditions;
using FallChallenge2023.Bots.Bronze.Agents.Decisions;
using FallChallenge2023.Bots.Bronze.GameMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FallChallenge2023.Bots.Bronze.Agents
{
    public class DroneAgent
    {
        public Drone Drone { get; }
        public GameAction Action { get; private set; } 

        public bool Lighting { get; set; } = true;

        private Dictionary<int, bool> CheckedConditions { get; } = new Dictionary<int, bool>();

        public DroneAgent(Drone drone)
        {
            Drone = drone;
        }

        public void FindAction(GameState state)
        {
            Action = new GameActionWait() { Text = "WhatsssAPP???" };
            CheckedConditions.Clear();

            // Decided action
            Action = GetActionFromDecision(new List<Decision>()
            {
                new EmergencyDecision(this, state), // Need repair
                //new LightDecision(this, state), // Need lighting or not
                //new MotorDecision(this, state), // Need motor or not
                //new PreSaveDecision(this, state), // Need save, when not all scanning
                new DiveDecision(this, state), // Dive from start
                new SearchDecision(this, state), // Search for scan
                new SaveDecision(this, state) // Save if all scans
                //new ScareDecision(this, state), // Try to lost scaning fish
            });
        }

        private GameAction GetActionFromDecision(List<Decision> decisions)
        {
            GameAction action = null;

            foreach (var decision in decisions)
                if (decision.CheckConditions())
                {
                    if (decision.Decisions == null) action = decision.GetDecision();
                    else action = GetActionFromDecision(decision.Decisions);
                    if (action != null) break;
                }

            return action;
        }

        public bool CheckCondition(Condition condition)
        {
            if (!CheckedConditions.TryGetValue(condition.Id, out var result))
                CheckedConditions.Add(condition.Id, result = condition.Check());

            return result;
        }

        public Vector GetAroundMonster(GameState state, Vector from, Vector to)
        {
            var move = to - from;

            var found = true;
            var checks = new HashSet<int>();

            while (found)
            {
                found = false;
                foreach (var fish in state.Fishes.Where(_ => !checks.Contains(_.Id) && _.Color == FishColor.UGLY && _.Status == FishStatus.VISIBLE))
                    if (CheckColision(fish.Position, fish.Speed, from, to))
                    {
                        var fp = Drone.Position - fish.Position;
                        var fs = fish.Speed - move;
                        var tanget = Math.Asin((Fish.MONSTER_ATTACK_RADIUS + 1) / fp.Length());
                        var fv = Math.Acos(Vector.Dot(fp, fs) / (fp.Length() * fs.Length()));//atan2(y,x)

                        var sign = Math.Sign(fp.X * fs.Y - fp.Y * fs.X);
                        move = move.Rotate(-tanget - sign * fv);

                        found = true;
                        checks.Add(fish.Id);
                    }
            }

            return from + move;
        }

        public bool CheckColision(Vector position, Vector speed, Vector from, Vector to)
        {
            if (speed.IsZero() && to.Equals(from)) return false;

            var pos = position - from;
            var vd = to - from;
            if (vd.Equals(speed)) return false;

            var vf = speed - vd;
            var a = vf.LengthSqr();
            var b = 2.0 * Vector.Dot(pos, vf);
            var c = pos.LengthSqr() - Fish.MONSTER_ATTACK_RADIUS_SQR;
            var delta = b * b - 4.0 * a * c;
            if (delta < 0.0) return false;

            double t = (-b - Math.Sqrt(delta)) / (2.0 * a);

            if (t <= 0.0 || t > 1.0) return false;

            return true;
        }
    }
}
