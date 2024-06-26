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


    [Serializable]
    public class Skill : IComponent
    {
        private static readonly HashSet<Entity> _tempEntities = new HashSet<Entity>();

        public Action<Skill> OnSkillStartHandler;
        public Action<Skill> OnSkillPartStartHandler;
        public Action<Skill> OnSkillDoneHandler;

        public SkillInfo SkillInfo;

        public LFloat CdTimer;
        public ESkillState State;
        public LFloat skillTimer;
        public int[] partCounter = new int[0];
        private int _curPartIdx;

        public SkillPart CurPart => _curPartIdx == -1 ? null : Parts[_curPartIdx];

        private float _showTimer;

        public LFloat CD => SkillInfo.CD;
        public LFloat DoneDelay => SkillInfo.doneDelay;
        public List<SkillPart> Parts => SkillInfo.parts;
        public int TargetLayer => SkillInfo.targetLayer;
        public LFloat MaxPartTime => SkillInfo.maxPartTime;
        public string AnimName => SkillInfo.animName;


        public Skill(Entity entity, SkillInfo info) : base(entity)
        {
            this.SkillInfo = info;
        }

        public override void Awake()
        {
            skillTimer = MaxPartTime;
            State = ESkillState.Idle;
            _curPartIdx = -1;
            partCounter = new int[Parts.Count];
        }

        public override void Start()
        {
        }

        public override void Update(LFloat deltaTime)
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
                    Entity.LTrans2D.pos += CurPart.moveSpd * deltaTime * Entity.LTrans2D.forward;
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
                ((Player)Entity)._mover.needMove = false;
                OnFire();
                return true;
            }

            return false;
        }

        public void OnFire()
        {
            OnSkillStartHandler?.Invoke(this);
        }

        public void Done()
        {
            OnSkillDoneHandler?.Invoke(this);
            State = ESkillState.Idle;
        }

        public void ForceStop()
        { 
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
            OnSkillPartStartHandler?.Invoke(this);
            _curPartIdx = idx;
            _showTimer = Time.realtimeSinceStartup + 0.1f;

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
                var colPos = Entity.LTrans2D.TransformPoint(col.pos);
                foreach (var e in World.Instance.GetEnemies())
                {
                    var targetCenter = e.LTrans2D.pos;
                    if ((targetCenter - colPos).sqrMagnitude < col.radius * col.radius)
                    {
                        _tempEntities.Add(e);
                    }
                }
            }
#endif
            foreach (var other in _tempEntities)
            {
                other.TakeDamage(Entity, CurPart.damage, other.LTrans2D.pos.ToLVector3());
            }

            //add force
            if (part.needForce)
            {
                var force = part.impulseForce;
                var forward = Entity.LTrans2D.forward;
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
                var deg = (other.Transform2D.pos - Entity.LTrans2D.pos).ToDeg();
                var degDiff = Entity.LTrans2D.deg.Abs() - deg;
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