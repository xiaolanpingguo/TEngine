using System;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.Framework;
using System.Text;


namespace Lockstep.Game
{
    public enum EAnimDefine
    {
        Walk,
        Run,
        RunFast,
        Evade,
        Hit,
        Idle,
        Jump,
        Died,
        AtkNormal01,
        AtkNormal02,
        AtkNormal03,
        AtkSkill01,
        AtkSkill02,
        AtkSkill03,
    }

    public class AnimDefine
    {
        public static string Walk = EAnimDefine.Walk.ToString();
        public static string Run = EAnimDefine.Run.ToString();
        public static string RunFast = EAnimDefine.RunFast.ToString();
        public static string Evade = EAnimDefine.Evade.ToString();
        public static string Hit = EAnimDefine.Hit.ToString();
        public static string Idle = EAnimDefine.Idle.ToString();
        public static string Jump = EAnimDefine.Jump.ToString();
        public static string Died = EAnimDefine.Died.ToString();
        public static string AtkNormal01 = EAnimDefine.AtkNormal01.ToString();
        public static string AtkNormal02 = EAnimDefine.AtkNormal02.ToString();
        public static string AtkNormal03 = EAnimDefine.AtkNormal03.ToString();
        public static string AtkSkill01 = EAnimDefine.AtkSkill01.ToString();
        public static string AtkSkill02 = EAnimDefine.AtkSkill02.ToString();
        public static string AtkSkill03 = EAnimDefine.AtkSkill03.ToString();
    }

    [Serializable]
    public class CAnimator : IComponent
    {
        private int _configId = 1;

        private AnimatorConfig _config;
        private AnimatorView _view;

        private LFloat _animLen;
        private LFloat _timer;
        private string _curAnimName = "";
        private int _curAnimIdx = -1;

        private List<string> _animNames = new List<string>();
        private LVector3 _intiPos;

        private List<AnimInfo> _animInfos => _config.anims;

        public AnimBindInfo CurAnimBindInfo;
        public AnimInfo curAnimInfo => _curAnimIdx == -1 ? null : _animInfos[_curAnimIdx];

        public CAnimator(Entity entity) : base(entity)
        {

        }

        public override void Start()
        {
            _config = GameConfigSingleton.Instance.GetAnimatorConfig(_configId);
            UpdateBindInfo();
            _animNames.Clear();
            foreach (var info in _animInfos)
            {
                _animNames.Add(info.name);
            }

            Play(AnimDefine.Idle);
        }

        void UpdateBindInfo()
        {
            CurAnimBindInfo = _config.events.Find((a) => a.name == _curAnimName);
            if (CurAnimBindInfo == null)
            {
                CurAnimBindInfo = AnimBindInfo.Empty;
            }
        }

        public override void Update(LFloat deltaTime)
        {
            _animLen = curAnimInfo.length;
            _timer += deltaTime;
            if (_timer > _animLen)
            {
                ResetAnim();
            }

            _view?.Sample(_timer);

            var idx = GetTimeIdx(_timer);
            if (CurAnimBindInfo.isMoveByAnim)
            {
                var animOffset = curAnimInfo[idx].pos;
                var pos = Entity.LTrans2D.TransformDirection(animOffset.ToLVector2XZ());
                Entity.LTrans2D.Pos3 = (_intiPos + pos.ToLVector3XZ(animOffset.y));
            }
        }

        public void SetTrigger(string name, bool isCrossfade = false)
        {
            Play(name, isCrossfade); //TODO
        }

        public void Play(string name, bool isCrossfade = false)
        {
            if (_curAnimName == name)
            {
                return;
            }

            var idx = _animNames.IndexOf(name);
            if (idx == -1)
            {
                UnityEngine.Debug.LogError("miss animation " + name);
                return;
            }

            var hasChangedAnim = _curAnimName != name;
            _curAnimName = name;
            _curAnimIdx = idx;
            UpdateBindInfo();

            if (hasChangedAnim)
            {
                //owner.TakeDamage(0, owner.transform2D.Pos3);
                ResetAnim();
            }

            _view?.Play(_curAnimName, isCrossfade);
        }

        public void SetTime(LFloat timer)
        {
            var idx = GetTimeIdx(timer);
            _intiPos = Entity.LTrans2D.Pos3 - curAnimInfo[idx].pos;
            this._timer = timer;
        }

        private void ResetAnim()
        {
            _timer = LFloat.zero;
            SetTime(LFloat.zero);
        }

        private int GetTimeIdx(LFloat timer)
        {
            var idx = (int)(timer / AnimatorConfig.FrameInterval);
            idx = System.Math.Min(curAnimInfo.OffsetCount - 1, idx);
            return idx;
        }

        public override void Serialize(Serializer writer)
        {
            writer.Write(_animLen);
            writer.Write(_curAnimIdx);
            writer.Write(_curAnimName);
            writer.Write(_timer);
            writer.Write(_configId);
        }

        public override void Deserialize(Deserializer reader)
        {
            _animLen = reader.ReadLFloat();
            _curAnimIdx = reader.ReadInt32();
            _curAnimName = reader.ReadString();
            _timer = reader.ReadLFloat();
            _configId = reader.ReadInt32();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += _animLen.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _curAnimIdx.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _curAnimName.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _timer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _configId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "_animLen" + ":" + _animLen.ToString());
            sb.AppendLine(prefix + "_curAnimIdx" + ":" + _curAnimIdx.ToString());
            sb.AppendLine(prefix + "_curAnimName" + ":" + _curAnimName.ToString());
            sb.AppendLine(prefix + "_timer" + ":" + _timer.ToString());
            sb.AppendLine(prefix + "configId" + ":" + _configId.ToString());
        }
    }
}