using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{

    [Serializable]
    public class CMover : IComponent
    {
        public Player player => (Player)Entity;
        public PlayerCommands input => player.input;

        public LFloat speed => player.moveSpd;
        public bool hasReachTarget = false;
        public bool needMove = true;

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
                transform.pos = transform.pos + dir * speed * deltaTime;
                var targetDeg = dir.ToDeg();
                transform.deg = CTransform2D.TurnToward(targetDeg, transform.deg, player.turnSpd * deltaTime, out var hasReachDeg);
            }

            hasReachTarget = !needChase;
        }

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(hasReachTarget);
            writer.Write(needMove);
        }

        public override void ReadBackup(Deserializer reader)
        {
            hasReachTarget = reader.ReadBoolean();
            needMove = reader.ReadBoolean();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += hasReachTarget.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += needMove.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "hasReachTarget" + ":" + hasReachTarget.ToString());
            sb.AppendLine(prefix + "needMove" + ":" + needMove.ToString());
        }
    }
}