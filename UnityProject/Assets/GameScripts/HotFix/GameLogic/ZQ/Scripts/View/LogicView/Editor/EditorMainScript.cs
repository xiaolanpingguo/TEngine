using Lockstep.Framework;
using UnityEditor;
using UnityEngine;



namespace Lockstep.Game
{

    [CustomEditor(typeof(Game))]
    public class EditorMainScript : Editor
    {
        private Game _owner;
        public int rollbackTickCount = 60;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _owner = Game.Instance;
            var world = World.Instance;
            EditorGUILayout.LabelField("CurTick " + world.Tick);
            rollbackTickCount = EditorGUILayout.IntField("RollbackTickCount", rollbackTickCount);
            if (GUILayout.Button("Rollback"))
            {
                SimulatorService.Instance.__debugRockbackToTick = world.Tick - rollbackTickCount;
            }
            if (GUILayout.Button("Resume"))
            {
                world.IsPause = false;
            }
        }
    }
}