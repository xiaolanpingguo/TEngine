using System;
using UnityEngine;


namespace Lockstep.Framework 
{
    public class ColliderDataMono : UnityEngine.MonoBehaviour 
    {
        public ColliderData colliderData;
    }


    [Serializable]
    public partial class ColliderData :IComponent, IAfterBackup
    {
        [Header("Offset")]
        public LFloat y;
        public LVector2 pos;

        [Header("Collider data")]
        public LFloat high;
        public LFloat radius;
        public LVector2 size;
        public LVector2 up;
        public LFloat deg;

        public void OnAfterDeserialize()
        {
        }
    }
}