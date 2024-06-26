using System;
using System.Text;
using Lockstep.Framework;
using static UnityEngine.GraphicsBuffer;


namespace Lockstep.Game
{

    [Serializable]
    public class CCharacterController : IComponent
    {
        public Player player => (Player)Entity;
        public PlayerCommands input => player.input;

        private LFloat _moveSpd = 5;
        private LFloat _turnSpd = 360;
        public bool _hasReachTarget = false;

        public CCharacterController(Entity entity) : base(entity)
        {
        }

        public override void Update(LFloat deltaTime)
        {
            if (!Entity.rigidbody.isOnFloor)
            {
                return;
            }

            var needChase = input.inputUV.sqrMagnitude > new LFloat(true, 10);
            if (needChase)
            {
                var dir = input.inputUV.normalized;
                Entity.LTrans2D.pos = Entity.LTrans2D.pos + dir * _moveSpd * deltaTime;
                var targetDeg = dir.ToDeg();
                Entity.LTrans2D.deg = CTransform2D.TurnToward(targetDeg, Entity.LTrans2D.deg, _turnSpd * deltaTime, out var hasReachDeg);
            }

            _hasReachTarget = !needChase;
        }

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(_hasReachTarget);
        }

        public override void ReadBackup(Deserializer reader)
        {
            _hasReachTarget = reader.ReadBoolean();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += _hasReachTarget.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "hasReachTarget" + ":" + _hasReachTarget.ToString());
        }
    }
}