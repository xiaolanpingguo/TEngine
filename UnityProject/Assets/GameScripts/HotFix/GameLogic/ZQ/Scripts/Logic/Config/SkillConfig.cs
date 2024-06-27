using System;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class SkillColliderInfo
    {
        public LVector2 pos;
        public LVector2 size;
        public LFloat radius;
        public LFloat deg = new LFloat(180);
        public LFloat maxY;

        public bool IsCircle => radius > 0;
    }

    public class SkillPart
    {
        public bool DebugShow;
        public LFloat moveSpd;
        public LFloat startFrame;
        public SkillColliderInfo collider;
        public LVector3 impulseForce;
        public bool needForce;
        public bool isResetForce;

        public LFloat interval;
        public int otherCount;
        public int damage;
        
        private LFloat _animFrameScale = new LFloat(true, 1667);

        public LFloat NextTriggerTimer(int counter)
        {
            return StartTimer() + interval * counter;
        }

        public LFloat StartTimer()
        {
            return startFrame * _animFrameScale;
        }

        public LFloat DeadTimer()
        {
            return StartTimer() + interval * (otherCount + LFloat.half);
        }
    }

    public class SkillConfig : ScriptableObject
    {
        public LFloat CD;
        public LFloat doneDelay;
        public int targetLayer;
        public LFloat maxPartTime;
        public List<SkillPart> parts = new List<SkillPart>();

        public SkillConfig()
        {
            parts.Sort((a, b) => LMath.Sign(a.startFrame - b.startFrame));
            var time = LFloat.MinValue;
            foreach (var part in parts)
            {
                var partDeadTime = part.DeadTimer();
                if (partDeadTime > time)
                {
                    time = partDeadTime;
                }
            }

            maxPartTime = time + doneDelay;
        }
    }
}