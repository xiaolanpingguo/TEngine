using System;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.Framework;
using System.Text;
using static Lockstep.Framework.LMathUtils;
using dnlib.DotNet.Pdb;


namespace Lockstep.Game
{
    public enum ESkillState
    {
        Idle,
        Firing,
    }

    [Serializable]
    public class CSkill : IComponent
    {
        private static readonly HashSet<Entity> _tempEntities = new HashSet<Entity>();

        public Action<CSkill> OnSkillStartHandler;
        public Action<CSkill> OnSkillPartStartHandler;
        public Action<CSkill> OnSkillDoneHandler;

        private bool _deubgCollistion = false;

        private LFloat _cdTimer;
        private int _curPartIdx;
        private LFloat _skillTimer;
        private ESkillState _state;
        private int[] _partCounter = new int[0];

        private SkillPart _curPart => _curPartIdx == -1 ? null : _parts[_curPartIdx];

        private float _showTimer;

        // config
        private LFloat _cd;
        private LFloat _doneDelay;
        private List<SkillPart> _parts;
        private int _targetLayer;
        private LFloat _maxPartTime;


        public CSkill(Entity entity, SkillConfig config) : base(entity)
        {
            _cd = config.CD;
            _doneDelay = config.doneDelay;
            _parts = config.parts;
            _targetLayer = config.targetLayer;
            _maxPartTime = config.maxPartTime;

            _skillTimer = _maxPartTime;
            _state = ESkillState.Idle;
            _curPartIdx = -1;
            _partCounter = new int[_parts.Count];
        }

        public override void Start()
        {
        }

        public override void Update(LFloat deltaTime)
        {
            _cdTimer -= deltaTime;
            _skillTimer += deltaTime;
            if (_skillTimer < _maxPartTime)
            {
                for (int i = 0; i < _parts.Count; i++)
                {
                    var part = _parts[i];
                    CheckSkillPart(part, i);
                }

                if (_curPart != null && _curPart.moveSpd != 0)
                {
                    Entity.LTrans2D.pos += _curPart.moveSpd * deltaTime * Entity.LTrans2D.forward;
                }
            }
            else
            {
                _curPartIdx = -1;
                if (_state == ESkillState.Firing)
                {
                    Done();
                }
            }
        }

        public bool Fire()
        {
            if (_cdTimer <= 0 && _state == ESkillState.Idle)
            {
                _cdTimer = _cd;
                _skillTimer = LFloat.zero;
                for (int i = 0; i < _partCounter.Length; i++)
                {
                    _partCounter[i] = 0;
                }

                _state = ESkillState.Firing;
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
            _state = ESkillState.Idle;
        }

        public void ForceStop()
        { 
        }

        void CheckSkillPart(SkillPart part, int idx)
        {
            if (_partCounter[idx] > part.otherCount)
            {
                return;
            }

            if (_skillTimer > part.NextTriggerTimer(_partCounter[idx]))
            {
                TriggerPart(part, idx);
                _partCounter[idx]++;
            }
        }

        void TriggerPart(SkillPart part, int idx)
        {
            OnSkillPartStartHandler?.Invoke(this);
            _curPartIdx = idx;
            _showTimer = Time.realtimeSinceStartup + 0.1f;

            var col = part.collider;

            if (_deubgCollistion)
            {
                if (col.radius > 0)
                {
                    //circle
                    PhysicSystem.QueryRegion(_targetLayer, Entity.LTrans2D.TransformPoint(col.pos), col.radius, _OnTriggerEnter);
                }
                else
                {
                    //aabb
                    PhysicSystem.QueryRegion(_targetLayer, Entity.LTrans2D.TransformPoint(col.pos), col.size, Entity.LTrans2D.forward, _OnTriggerEnter);
                }
            }
            else
            {
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
            }

            foreach (var other in _tempEntities)
            {
                CHealth health = other.GetComponent<CHealth>();
                if (health != null)
                {
                    health.TakeDamage(Entity, _curPart.damage, other.LTrans2D.pos.ToLVector3());
                }
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
                    other.Rigidbody.AddImpulse(force);
                }
            }

            if (part.isResetForce)
            {
                foreach (var other in _tempEntities)
                {
                    other.Rigidbody.ResetSpeed(new LFloat(3));
                }
            }

            _tempEntities.Clear();
        }

        private void _OnTriggerEnter(ColliderProxy other)
        {
            if (_curPart.collider.IsCircle && _curPart.collider.deg > 0)
            {
                var deg = (other.Transform2D.pos - Entity.LTrans2D.pos).ToDeg();
                var degDiff = Entity.LTrans2D.deg.Abs() - deg;
                if (LMath.Abs(degDiff) <= _curPart.collider.deg)
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
            float tintVal = 0.3f;
            Gizmos.color = new Color(0, 1.0f - tintVal, tintVal, 0.25f);
            if (Application.isPlaying) 
            {
                if (_curPart == null)
                {
                    return;
                }

                if (_showTimer < Time.realtimeSinceStartup) 
                {
                    return;
                }

                ShowPartGizmons(_curPart);
            }
            else 
            {
                foreach (var part in _parts) 
                {
                    if (part._DebugShow) 
                    {
                        ShowPartGizmons(part);
                    }
                }
            }

            Gizmos.color = Color.white;
        }

        private void ShowPartGizmons(SkillPart part)
        {
            var col = part.collider;
            if (col.radius > 0)
            {
                //circle
                var pos = Entity.LTrans2D.TransformPoint(col.pos);
                Gizmos.DrawSphere(pos.ToVector3XZ(LFloat.one), col.radius.ToFloat());
            }
            else
            {
                //aabb
                LVector2 pos = Entity.LTrans2D.TransformPoint(col.pos);
                Gizmos.DrawCube(pos.ToVector3XZ(LFloat.one), col.size.ToVector3XZ(LFloat.one));
                DebugDraw.DebugLocalCube(Matrix4x4.TRS(
                        pos.ToVector3XZ(LFloat.one),
                        Quaternion.Euler(0, Entity.LTrans2D.deg.ToFloat(), 0),
                        Vector3.one),
                    col.size.ToVector3XZ(LFloat.one), Gizmos.color);
            }
        }

        public override void Serialize(Serializer writer)
        {
            writer.Write(_cdTimer);
            writer.Write(_curPartIdx);
            writer.Write(_skillTimer);
            writer.Write((int)(_state));
            writer.Write(_partCounter);
        }

        public override void Deserialize(Deserializer reader)
        {
            _cdTimer = reader.ReadLFloat();
            _curPartIdx = reader.ReadInt32();
            _skillTimer = reader.ReadLFloat();
            _state = (ESkillState)reader.ReadInt32();
            _partCounter = reader.ReadArray(this._partCounter);
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += _cdTimer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _curPartIdx.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _skillTimer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += ((int)_state) * PrimerLUT.GetPrimer(idx++);
            if (_partCounter != null) foreach (var item in _partCounter) { if (item != default(System.Int32)) hash += item.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++); }
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "CdTimer" + ":" + _cdTimer.ToString());
            sb.AppendLine(prefix + "_curPartIdx" + ":" + _curPartIdx.ToString());
            sb.AppendLine(prefix + "skillTimer" + ":" + _skillTimer.ToString());
            sb.AppendLine(prefix + "State" + ":" + _state.ToString());
            SerializeUtil.DumpList("partCounter", _partCounter, sb, prefix);
        }
    }

}