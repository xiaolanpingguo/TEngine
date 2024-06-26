//using Lockstep.Framework;
//using UnityEditor;
//using UnityEngine;


//namespace Lockstep.Game
//{
//    [CustomEditor(typeof(EntityView))]
//    public class EditorEnemyView : Editor
//    {
//        private EnemyView owner;
//        public LVector3 force;
//        public LFloat resetYSpd;
//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//            owner = target as EntityView;
//            force = LMathUtils.Vector3Field("force", force);
//            if (GUILayout.Button("AddImpulse"))
//            {
//                owner.entity.rigidbody.AddImpulse(force);
//            }

//            resetYSpd = LMathUtils.FloatField("resetYSpd", resetYSpd);
//            if (GUILayout.Button("ResetSpeed"))
//            {
//                owner.entity.rigidbody.ResetSpeed(resetYSpd);
//            }
//        }
//    }
//}