using System;
using System.Runtime.InteropServices;
using System.Text;


namespace Lockstep.Framework
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct LTransform2D
    {
        public LVector2 pos;
        public LFloat y;
        public LFloat deg;
    }

    [Serializable]
    public class CTransform2D
    {
        public CTransform2D() { }
        public CTransform2D(LVector2 pos, LFloat y) : this(pos, y, LFloat.zero) { }
        public CTransform2D(LVector2 pos) : this(pos, LFloat.zero, LFloat.zero) { }
        public CTransform2D(LVector2 pos, LFloat y, LFloat deg)
        {
            this.pos = pos;
            this.y = y;
            this.deg = deg;
        }

        public LVector2 pos;
        public LFloat y;
        public LFloat deg; //same as Unity CW deg(up) =0

        public LVector2 forward
        {
            //等同于2D  up
            get
            {
                LFloat s, c;
                var ccwDeg = (-deg + 90);
                LMath.SinCos(out s, out c, LMath.Deg2Rad * ccwDeg);
                return new LVector2(c, s);
            }
            set => deg = ToDeg(value);
        }

        public static LFloat ToDeg(LVector2 value)
        {
            var ccwDeg = LMath.Atan2(value.y, value.x) * LMath.Rad2Deg;
            var deg = 90 - ccwDeg;
            return AbsDeg(deg);
        }

        public static LFloat TurnToward(LVector2 targetPos, LVector2 currentPos, LFloat cursDeg, LFloat turnVal, out bool isLessDeg)
        {
            var toTarget = (targetPos - currentPos).normalized;
            var toDeg = CTransform2D.ToDeg(toTarget);
            return TurnToward(toDeg, cursDeg, turnVal, out isLessDeg);
        }

        public static LFloat TurnToward(LFloat toDeg, LFloat cursDeg, LFloat turnVal, out bool isLessDeg)
        {
            var curDeg = CTransform2D.AbsDeg(cursDeg);
            var diff = toDeg - curDeg;
            var absDiff = LMath.Abs(diff);
            isLessDeg = absDiff < turnVal;
            if (isLessDeg)
            {
                return toDeg;
            }
            else
            {
                if (absDiff > 180)
                {
                    if (diff > 0)
                    {
                        diff -= 360;
                    }
                    else
                    {
                        diff += 360;
                    }
                }

                return curDeg + turnVal * LMath.Sign(diff);
            }
        }

        public static LFloat AbsDeg(LFloat deg)
        {
            var rawVal = deg._val % ((LFloat)360)._val;
            return new LFloat(true, rawVal);
        }

        public void Reset()
        {
            pos = LVector2.zero;
            y = LFloat.zero;
            deg = LFloat.zero;
        }

        public LVector2 TransformPoint(LVector2 point)
        {
            return pos + TransformDirection(point);
        }

        public LVector2 TransformVector(LVector2 vec)
        {
            return TransformDirection(vec);
        }

        public LVector2 TransformDirection(LVector2 dir)
        {
            var y = forward;
            var x = forward.RightVec();
            return dir.x * x + dir.y * y;
        }

        public static LTransform2D operator +(CTransform2D a, CTransform2D b)
        {
            return new LTransform2D { pos = a.pos + b.pos, y = a.y + b.y, deg = a.deg + b.deg };
        }

        public LVector3 Pos3
        {
            get => new LVector3(pos.x, y, pos.y);
            set
            {
                pos = new LVector2(value.x, value.z);
                y = value.y;
            }
        }

        public override string ToString()
        {
            return $"(deg:{deg} pos:{pos} y:{y})";
        }

        public void WriteBackup(Serializer writer)
        {
            writer.Write(deg);
            writer.Write(pos);
            writer.Write(y);
        }

        public void ReadBackup(Deserializer reader)
        {
            deg = reader.ReadLFloat();
            pos = reader.ReadLVector2();
            y = reader.ReadLFloat();
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;
            hash += deg.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += pos.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += y.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "deg" + ":" + deg.ToString());
            sb.AppendLine(prefix + "pos" + ":" + pos.ToString());
            sb.AppendLine(prefix + "y" + ":" + y.ToString());
        }
    }
}