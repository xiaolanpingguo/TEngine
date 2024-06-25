using System;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.Framework;


namespace Lockstep.Game
{

    [Serializable]
    public partial class CAnimator : IComponent
    {
        public int configId;

        [HideInInspector]public AnimatorConfig config;
        [HideInInspector]public IAnimatorView view;
        [HideInInspector]public AnimBindInfo curAnimBindInfo;

        private LFloat _animLen;
        private LFloat _timer;
        private string _curAnimName = "";
        private int _curAnimIdx = -1;

        private List<string> _animNames = new List<string>();
        private LVector3 _intiPos;

        private List<AnimInfo> _animInfos => config.anims;
        public AnimInfo curAnimInfo => _curAnimIdx == -1 ? null : _animInfos[_curAnimIdx];

        public override void BindEntity(Entity baseEntity)
        {
            base.BindEntity(baseEntity);
            config = GameConfigSingleton.Instance.GetAnimatorConfig(configId);
            if (config == null) return;
            UpdateBindInfo();
            _animNames.Clear();
            foreach (var info in _animInfos)
            {
                _animNames.Add(info.name);
            }
        }

        void UpdateBindInfo()
        {
            curAnimBindInfo = config.events.Find((a) => a.name == _curAnimName);
            if (curAnimBindInfo == null) curAnimBindInfo = AnimBindInfo.Empty;
        }

        public override void Start()
        {
            Play(AnimDefine.Idle);
        }

        public override void Update(LFloat deltaTime)
        {
            if (config == null) return;
            _animLen = curAnimInfo.length;
            _timer += deltaTime;
            if (_timer > _animLen)
            {
                ResetAnim();
            }

            view?.Sample(_timer);

            var idx = GetTimeIdx(_timer);
            if (curAnimBindInfo.isMoveByAnim)
            {
                var animOffset = curAnimInfo[idx].pos;
                var pos = transform.TransformDirection(animOffset.ToLVector2XZ());
                transform.Pos3 = (_intiPos + pos.ToLVector3XZ(animOffset.y));
            }
        }

        public void SetTrigger(string name, bool isCrossfade = false)
        {
            Play(name, isCrossfade); //TODO
        }

        public void Play(string name, bool isCrossfade = false)
        {
            if (config == null) return;
            if (_curAnimName == name)
                return;

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

            view?.Play(_curAnimName, isCrossfade);
        }

        public void SetTime(LFloat timer)
        {
            if (config == null) return;
            var idx = GetTimeIdx(timer);
            _intiPos = transform.Pos3 - curAnimInfo[idx].pos;
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
    }
}