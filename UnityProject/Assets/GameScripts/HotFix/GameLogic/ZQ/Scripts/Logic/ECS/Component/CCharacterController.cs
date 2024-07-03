using System;
using System.Text;
using Lockstep.Framework;
using static UnityEngine.GraphicsBuffer;


namespace Lockstep.Game
{

    [Serializable]
    public class CCharacterController : IComponent
    {
        private bool _hasReachTarget = false;

        public LFloat MoveSpd = 5;
        public LFloat TurnSpd = 360;

        public CCharacterController(Entity entity) : base(entity)
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

            PlayerCommand input = World.Instance.GetPlayerInput((byte)Entity.EntityId);
            var needChase = input.inputUV.sqrMagnitude > new LFloat(true, 10);
            if (needChase)
            {
                var dir = input.inputUV.normalized;
                Entity.LTrans2D.pos = Entity.LTrans2D.pos + dir * MoveSpd * deltaTime;
                var targetDeg = dir.ToDeg();
                Entity.LTrans2D.deg = CTransform2D.TurnToward(targetDeg, Entity.LTrans2D.deg, TurnSpd * deltaTime, out var hasReachDeg);
            }

            _hasReachTarget = !needChase;
        }

        public override void Serialize(Serializer writer)
        {
            writer.Write(_hasReachTarget);
            writer.Write(MoveSpd);
            writer.Write(TurnSpd);
        }

        public override void Deserialize(Deserializer reader)
        {
            _hasReachTarget = reader.ReadBoolean();
            MoveSpd = reader.ReadLFloat();
            TurnSpd = reader.ReadLFloat();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += _hasReachTarget.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += MoveSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += TurnSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "hasReachTarget" + ":" + _hasReachTarget.ToString());
            sb.AppendLine(prefix + "moveSpd" + ":" + MoveSpd.ToString());
            sb.AppendLine(prefix + "turnSpd" + ":" + TurnSpd.ToString());
        }
    }
}