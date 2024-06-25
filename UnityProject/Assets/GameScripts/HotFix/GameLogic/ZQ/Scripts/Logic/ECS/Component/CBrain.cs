using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class CBrain : IComponent
    {
        public Entity target { get; private set; }
        public int targetId;
        public LFloat stopDistSqr = 1 * 1;
        public LFloat atkInterval = 1;
        private LFloat _atkTimer;

        public override void BindEntity(Entity e)
        {
            base.BindEntity(e);
            target = World.Instance.GetEntity(targetId) as Entity;
        }

        public override void Update(LFloat deltaTime)
        {
            if (!Entity.rigidbody.isOnFloor)
            {
                return;
            }

            //find target
            var allPlayer = World.Instance.GetPlayers();
            var minDist = LFloat.MaxValue;
            Entity minTarget = null;
            foreach (var player in allPlayer)
            {
                if (player.isDead)
                {
                    continue;
                }

                var dist = (player.LTrans2D.pos - transform.pos).sqrMagnitude;
                if (dist < minDist)
                {
                    minTarget = player;
                    minDist = dist;
                }
            }

            target = minTarget;
            targetId = target?.EntityId ?? -1;

            if (minTarget == null)
            {
                return;
            }

            if (minDist > stopDistSqr)
            {
                // turn to target
                var targetPos = minTarget.LTrans2D.pos;
                var currentPos = transform.pos;
                var turnVal = Entity.turnSpd * deltaTime;
                var targetDeg = CTransform2D.TurnToward(targetPos, currentPos, transform.deg, turnVal, out var isFinishedTurn);
                transform.deg = targetDeg;
                //move to target
                var distToTarget = (targetPos - currentPos).magnitude;
                var movingStep = Entity.moveSpd * deltaTime;
                if (movingStep > distToTarget)
                {
                    movingStep = distToTarget;
                }

                var toTarget = (targetPos - currentPos).normalized;
                transform.pos = transform.pos + toTarget * movingStep;
            }
            else
            {
                //atk target
                _atkTimer -= deltaTime;
                if (_atkTimer <= 0)
                {
                    _atkTimer = atkInterval;
                    target.TakeDamage(Entity, Entity.damage, target.LTrans2D.Pos3);
                }
            }
        }

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(_atkTimer);
            writer.Write(atkInterval);
            writer.Write(stopDistSqr);
            writer.Write(targetId);
        }

        public override void ReadBackup(Deserializer reader)
        {
            _atkTimer = reader.ReadLFloat();
            atkInterval = reader.ReadLFloat();
            stopDistSqr = reader.ReadLFloat();
            targetId = reader.ReadInt32();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += _atkTimer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += atkInterval.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += stopDistSqr.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += targetId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "_atkTimer" + ":" + _atkTimer.ToString());
            sb.AppendLine(prefix + "atkInterval" + ":" + atkInterval.ToString());
            sb.AppendLine(prefix + "stopDistSqr" + ":" + stopDistSqr.ToString());
            sb.AppendLine(prefix + "targetId" + ":" + targetId.ToString());
        }
    }
}