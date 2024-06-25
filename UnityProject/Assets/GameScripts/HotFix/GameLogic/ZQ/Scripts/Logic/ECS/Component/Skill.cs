using System;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.Framework;
using System.Text;


namespace Lockstep.Game
{
    public enum ESkillState
    {
        Idle,
        Firing,
    }

    public interface ISkillEventHandler
    {
        void OnSkillStart(Skill skill);
        void OnSkillDone(Skill skill);
        void OnSkillPartStart(Skill skill);
    }

    [Serializable]
    public class Skill : IComponent
    {
        private static readonly HashSet<Entity> _tempEntities = new HashSet<Entity>();

        public ISkillEventHandler eventHandler;
        public Entity entity { get; private set; }
        public SkillInfo SkillInfo;

        public LFloat CdTimer;
        public ESkillState State;
        public LFloat skillTimer;
        public int[] partCounter = new int[0];
        private int _curPartIdx;

        public SkillPart CurPart => _curPartIdx == -1 ? null : Parts[_curPartIdx];
#if DEBUG_SKILL
        private float _showTimer;
#endif

        public LFloat CD => SkillInfo.CD;
        public LFloat DoneDelay => SkillInfo.doneDelay;
        public List<SkillPart> Parts => SkillInfo.parts;
        public int TargetLayer => SkillInfo.targetLayer;
        public LFloat MaxPartTime => SkillInfo.maxPartTime;
        public string AnimName => SkillInfo.animName;

        public void ForceStop() { }

        public void BindEntity(Entity entity, SkillInfo info, ISkillEventHandler eventHandler)
        {
            this.entity = entity;
            this.SkillInfo = info;
            this.eventHandler = eventHandler;
        }

        public void DoStart()
        {
            skillTimer = MaxPartTime;
            State = ESkillState.Idle;
            _curPartIdx = -1;
            partCounter = new int[Parts.Count];
        }


        public bool Fire()
        {
            if (CdTimer <= 0 && State == ESkillState.Idle)
            {
                CdTimer = CD;
                skillTimer = LFloat.zero;
                for (int i = 0; i < partCounter.Length; i++)
                {
                    partCounter[i] = 0;
                }

                State = ESkillState.Firing;
                entity.animator?.Play(AnimName);
                ((Player)entity).mover.needMove = false;
                OnFire();
                return true;
            }

            return false;
        }

        public void OnFire()
        {
            eventHandler.OnSkillStart(this);
        }

        public void Done()
        {
            eventHandler.OnSkillDone(this);
            State = ESkillState.Idle;
            entity.animator?.Play(AnimDefine.Idle);
        }

        public void DoUpdate(LFloat deltaTime)
        {
            CdTimer -= deltaTime;
            skillTimer += deltaTime;
            if (skillTimer < MaxPartTime)
            {
                for (int i = 0; i < Parts.Count; i++)
                {
                    var part = Parts[i];
                    CheckSkillPart(part, i);
                }

                if (CurPart != null && CurPart.moveSpd != 0)
                {
                    entity.transform.pos += CurPart.moveSpd * deltaTime * entity.transform.forward;
                }
            }
            else
            {
                _curPartIdx = -1;
                if (State == ESkillState.Firing)
                {
                    Done();
                }
            }
        }

        void CheckSkillPart(SkillPart part, int idx)
        {
            if (partCounter[idx] > part.otherCount) return;
            if (skillTimer > part.NextTriggerTimer(partCounter[idx]))
            {
                TriggerPart(part, idx);
                partCounter[idx]++;
            }
        }

        void TriggerPart(SkillPart part, int idx)
        {
            eventHandler.OnSkillPartStart(this);
            _curPartIdx = idx;
#if DEBUG_SKILL
            _showTimer = Time.realtimeSinceStartup + 0.1f;
#endif

            var col = part.collider;
#if NO_DEBUG_NO_COLLISION
            if (col.radius > 0) 
            {
                //circle
                PhysicSystem.QueryRegion(TargetLayer, entity.transform.TransformPoint(col.pos), col.radius, _OnTriggerEnter);
            }
            else 
            {
                //aabb
                PhysicSystem.QueryRegion(TargetLayer, entity.transform.TransformPoint(col.pos), col.size, entity.transform.forward, _OnTriggerEnter);
            }

#else
            //TODO Ignore CollisionSystem
            if (col.radius > 0)
            {
                var colPos = entity.transform.TransformPoint(col.pos);
                foreach (var e in World.Instance.GetEnemies())
                {
                    var targetCenter = e.transform.pos;
                    if ((targetCenter - colPos).sqrMagnitude < col.radius * col.radius)
                    {
                        _tempEntities.Add(e);
                    }
                }
            }
#endif
            foreach (var other in _tempEntities)
            {
                other.TakeDamage(entity, CurPart.damage, other.transform.pos.ToLVector3());
            }

            //add force
            if (part.needForce)
            {
                var force = part.impulseForce;
                var forward = entity.transform.forward;
                var right = forward.RightVec();
                var z = forward * force.z + right * force.x;
                force.x = z.x;
                force.z = z.y;
                foreach (var other in _tempEntities)
                {
                    other.rigidbody.AddImpulse(force);
                }
            }

            if (part.isResetForce)
            {
                foreach (var other in _tempEntities)
                {
                    other.rigidbody.ResetSpeed(new LFloat(3));
                }
            }

            _tempEntities.Clear();
        }


        //private static readonly HashSet<Entity> _tempEntities = new HashSet<Entity>();
        private void _OnTriggerEnter(ColliderProxy other)
        {
            if (CurPart.collider.IsCircle && CurPart.collider.deg > 0)
            {
                var deg = (other.Transform2D.pos - entity.transform.pos).ToDeg();
                var degDiff = entity.transform.deg.Abs() - deg;
                if (LMath.Abs(degDiff) <= CurPart.collider.deg)
                {
                    _tempEntities.Add((Entity)other.EntityObject);
                }
            }
            else
            {
                _tempEntities.Add((Entity)other.EntityObject);
            }
        }

        public void OnDrawGizmos()
        {
#if UNITY_EDITOR && DEBUG_SKILL
            float tintVal = 0.3f;
            Gizmos.color = new Color(0, 1.0f - tintVal, tintVal, 0.25f);
            if (Application.isPlaying) 
            {
                if (entity == null) return;
                if (CurPart == null) return;
                if (_showTimer < Time.realtimeSinceStartup) 
                {
                    return;
                }

                ShowPartGizmons(CurPart);
            }
            else 
            {
                foreach (var part in Parts) 
                {
                    if (part._DebugShow) 
                    {
                        ShowPartGizmons(part);
                    }
                }
            }

            Gizmos.color = Color.white;
#endif
        }

        private void ShowPartGizmons(SkillPart part)
        {
            //var col = part.collider;
            //if (col.radius > 0)
            //{
            //    //circle
            //    var pos = entity?.transform.TransformPoint(col.pos) ?? col.pos;
            //    Gizmos.DrawSphere(new UnityEngine.Vector3(pos.ToVector3XZ(LFloat.one)), col.radius.ToFloat());
            //}
            //else
            //{
            //    //aabb
            //    var pos = entity?.transform.TransformPoint(col.pos) ?? col.pos;
            //    Gizmos.DrawCube(pos.ToVector3XZ(LFloat.one), col.size.ToVector3XZ(LFloat.one));
            //    DebugDraw.DebugLocalCube(Matrix4x4.TRS(
            //            pos.ToVector3XZ(LFloat.one),
            //            Quaternion.Euler(0, entity.transform.deg.ToFloat(), 0),
            //            Vector3.one),
            //        col.size.ToVector3XZ(LFloat.one), Gizmos.color);
            //}
        }

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(CdTimer);
            writer.Write(_curPartIdx);
            writer.Write(skillTimer);
            writer.Write((int)(State));
            writer.Write(partCounter);
        }

        public override void ReadBackup(Deserializer reader)
        {
            CdTimer = reader.ReadLFloat();
            _curPartIdx = reader.ReadInt32();
            skillTimer = reader.ReadLFloat();
            State = (ESkillState)reader.ReadInt32();
            partCounter = reader.ReadArray(this.partCounter);
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += CdTimer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _curPartIdx.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += skillTimer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += ((int)State) * PrimerLUT.GetPrimer(idx++);
            if (partCounter != null) foreach (var item in partCounter) { if (item != default(System.Int32)) hash += item.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++); }
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "CdTimer" + ":" + CdTimer.ToString());
            sb.AppendLine(prefix + "_curPartIdx" + ":" + _curPartIdx.ToString());
            sb.AppendLine(prefix + "skillTimer" + ":" + skillTimer.ToString());
            sb.AppendLine(prefix + "State" + ":" + State.ToString());
            BackUpUtil.DumpList("partCounter", partCounter, sb, prefix);
        }
    }

}