#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace Lockstep.Game
{
    [CustomEditor(typeof(TestRigidbody))]
    public class EditorTestRigidbody : Editor
    {
        private TestRigidbody owner;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            owner = target as TestRigidbody;
            if (GUILayout.Button("AddImpulse"))
            {
                owner.AddImpulse();
            }

            if (GUILayout.Button("ResetSpeed"))
            {
                owner.ResetSpeed(owner.resetYSpd);
            }
        }
    }
}
#endif