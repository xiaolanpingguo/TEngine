using Lockstep.Framework;
using UnityEngine;


namespace Lockstep.Framework
{
    public static partial class LMathExtension
    {
        public static LVector2 ToLVector2(this Vector2Int vec)
        {
            return new LVector2(true, vec.x * LFloat.Precision, vec.y * LFloat.Precision);
        }

        public static LVector3 ToLVector3(this Vector3Int vec)
        {
            return new LVector3(true, vec.x * LFloat.Precision, vec.y * LFloat.Precision, vec.z * LFloat.Precision);
        }

        public static LVector2Int ToLVector2Int(this Vector2Int vec)
        {
            return new LVector2Int(vec.x, vec.y);
        }

        public static LVector3Int ToLVector3Int(this Vector3Int vec)
        {
            return new LVector3Int(vec.x, vec.y, vec.z);
        }
        public static Vector2Int ToVector2Int(this LVector2Int vec)
        {
            return new Vector2Int(vec.x, vec.y);
        }

        public static Vector3Int ToVector3Int(this LVector3Int vec)
        {
            return new Vector3Int(vec.x, vec.y, vec.z);
        }
    }
}