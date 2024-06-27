using UnityEngine;


namespace Lockstep.Game 
{

    //[Serializable]
    //public class EntityConfig 
    //{
    //    public virtual object Entity { get; }
    //    public string prefabPath;

    //    public void CopyTo(object dst)
    //    {
    //        if (Entity.GetType() != dst.GetType()) 
    //        {
    //            return;
    //        }

    //        FieldInfo[] fields = dst.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
    //        foreach (var field in fields)
    //        {
    //            CopyTo(field.GetValue(dst), field.GetValue(Entity));
    //        }
    //    }

    //    void CopyTo(object dst, object src)
    //    {
    //        if (dst == null || src == null || src.GetType() != dst.GetType()) 
    //        {
    //            return;
    //        }

    //        FieldInfo[] fields = dst.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
    //        foreach (var field in fields)
    //        {
    //            var type = field.FieldType;
    //            field.SetValue(dst, field.GetValue(src));
    //        }
    //    }
    //}

    public class GameConfig : ScriptableObject 
    {
        public string PrefabPath = "Prefabs/";
        public string PlayerPrefabPath = "Prefabs/Player.prefab";
        public string EnemyPrefabPath = "Prefabs/Enemy.prefab";
        public string SpawnerPrefabPath = "Prefabs/Spawner.prefeb";
        public string RecorderFilePath;
        public string DumpStrPath;
    }
}