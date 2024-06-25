using System;
using Lockstep.Framework;
using Lockstep.Game;
using UnityEditor;
using UnityEngine;


namespace Lockstep.Game
{
    public static class LMathUtils
    {
        public static LVector2 ToLVector2(this UnityEngine.Vector2 vec)
        {
            return new LVector2(
                LMath.ToLFloat(vec.x),
                LMath.ToLFloat(vec.y));
        }

        public static LVector3 ToLVector3(this UnityEngine.Vector3 vec)
        {
            return new LVector3(
                LMath.ToLFloat(vec.x),
                LMath.ToLFloat(vec.y),
                LMath.ToLFloat(vec.z));
        }
        public static LVector2 ToLVector2XZ(this UnityEngine.Vector3 vec)
        {
            return new LVector2(
                LMath.ToLFloat(vec.x),
                LMath.ToLFloat(vec.z));
        }
        public static UnityEngine.Vector2 ToVector2(this LVector2 vec)
        {
            return new UnityEngine.Vector2(vec.x.ToFloat(), vec.y.ToFloat());
        }
        public static UnityEngine.Vector3 ToVector3(this LVector2 vec)
        {
            return new UnityEngine.Vector3(vec.x.ToFloat(), vec.y.ToFloat(), 0);
        }
        public static UnityEngine.Vector3 ToVector3XZ(this LVector2 vec, LFloat y)
        {
            return new UnityEngine.Vector3(vec.x.ToFloat(), y.ToFloat(), vec.y.ToFloat());
        }
        public static UnityEngine.Vector3 ToVector3XZ(this LVector2 vec)
        {
            return new UnityEngine.Vector3(vec.x.ToFloat(), 0, vec.y.ToFloat());
        }
        public static UnityEngine.Vector3 ToVector3(this LVector3 vec)
        {
            return new UnityEngine.Vector3(vec.x.ToFloat(), vec.y.ToFloat(), vec.z.ToFloat());
        }
        public static Rect ToRect(this LRect vec)
        {
            return new Rect(vec.position.ToVector2(), vec.size.ToVector2());
        }

        public static Vector2 ToVector2XZ(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }

        public static Vector3 ToVector3(this Vector2 vec, int y = 1)
        {
            return new Vector3(vec.x, y, vec.y);
        }

#if UNITY_EDITOR
        public static LFloat FloatField(string label, LFloat value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.FloatField(label, value.ToFloat(), options).ToLFloat();
        }
        public static LVector2 Vector2Field(string label, LVector2 value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Vector2Field(label, value.ToVector2(), options).ToLVector2();
        }
        public static LVector3 Vector3Field(string label, LVector3 value, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Vector3Field(label, value.ToVector3(), options).ToLVector3();
        }
#endif
    }
}



namespace Lockstep.Game
{
    public class IComponent : BaseFormater
    {
        public BaseEntity baseEntity { get; private set; }
        public CTransform2D transform { get; private set; }

        public virtual void BindEntity(BaseEntity entity)
        {
            this.baseEntity = entity;
            transform = entity.transform;
        }

        public virtual void DoAwake() { }
        public virtual void DoStart() { }
        public virtual void DoUpdate(LFloat deltaTime) { }
        public virtual void DoDestroy() { }
    }
}