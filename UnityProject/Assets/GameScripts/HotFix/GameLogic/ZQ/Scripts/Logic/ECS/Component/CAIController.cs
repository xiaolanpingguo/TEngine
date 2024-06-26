using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class CAIController : IComponent
    {
        private LFloat _atkTimer;

        private int _targetId;
        private LFloat _stopDistSqr = 1 * 1;
        private LFloat _atkInterval = 1;

        public int Damage = 10;
        public LFloat MoveSpd = 2;
        public LFloat TurnSpd = 150;

        public CAIController(Entity entity) : base(entity)
        {
        }

        public override void Start()
        {
        }

        public override void Update(LFloat deltaTime)
        {
            if (!Entity.Rigidbody.isOnFloor)
            {
                return;
            }

            //find target
            var allPlayer = World.Instance.GetPlayers();
            var minDist = LFloat.MaxValue;
            Entity minTarget = null;
            foreach (var player in allPlayer)
            {
                var dist = (player.LTrans2D.pos - Entity.LTrans2D.pos).sqrMagnitude;
                if (dist < minDist)
                {
                    minTarget = player;
                    minDist = dist;
                }
            }

            if (minTarget == null)
            {
                return;
            }

            _targetId = minTarget.EntityId;
            if (minDist > _stopDistSqr)
            {
                // turn to target
                var targetPos = minTarget.LTrans2D.pos;
                var currentPos = Entity.LTrans2D.pos;
                var turnVal = TurnSpd * deltaTime;
                var targetDeg = CTransform2D.TurnToward(targetPos, currentPos, Entity.LTrans2D.deg, turnVal, out var isFinishedTurn);
                Entity.LTrans2D.deg = targetDeg;

                // move to target
                var distToTarget = (targetPos - currentPos).magnitude;
                var movingStep = MoveSpd * deltaTime;
                if (movingStep > distToTarget)
                {
                    movingStep = distToTarget;
                }

                var toTarget = (targetPos - currentPos).normalized;
                Entity.LTrans2D.pos = Entity.LTrans2D.pos + toTarget * movingStep;
            }
            else
            {
                // take damage
                _atkTimer -= deltaTime;
                if (_atkTimer <= 0)
                {
                    _atkTimer = _atkInterval;
                    CHealth health = minTarget.GetComponent<CHealth>();
                    if (health != null)
                    {
                        health.TakeDamage(Entity, Damage, minTarget.LTrans2D.Pos3);
                    }
                }
            }
        }

        public override void Serialize(Serializer writer)
        {
            writer.Write(_atkTimer);
            writer.Write(_atkInterval);
            writer.Write(_stopDistSqr);
            writer.Write(_targetId);
        }

        public override void Deserialize(Deserializer reader)
        {
            _atkTimer = reader.ReadLFloat();
            _atkInterval = reader.ReadLFloat();
            _stopDistSqr = reader.ReadLFloat();
            _targetId = reader.ReadInt32();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += _atkTimer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _atkInterval.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _stopDistSqr.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _targetId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "_atkTimer" + ":" + _atkTimer.ToString());
            sb.AppendLine(prefix + "atkInterval" + ":" + _atkInterval.ToString());
            sb.AppendLine(prefix + "stopDistSqr" + ":" + _stopDistSqr.ToString());
            sb.AppendLine(prefix + "targetId" + ":" + _targetId.ToString());
        }
    }
}