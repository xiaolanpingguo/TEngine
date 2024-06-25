using UnityEngine;


namespace Lockstep.Game
{
    public static class EntityUnityExt
    {
        public static Transform GetUnityTransform(this Entity value)
        {
            return value.engineTransform as Transform;
        }
    }
}