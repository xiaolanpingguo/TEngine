using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Lockstep.Framework;
using System.Security.Cryptography;


namespace Lockstep.Game 
{
    public enum EColliderLayer
    {
        Static,
        Enemy,
        Hero,
        EnumCount
    }

    [Serializable]
    public class EntityConfig 
    {
        public virtual object Entity { get; }
        public string prefabPath;

        public void CopyTo(object dst)
        {
            if (Entity.GetType() != dst.GetType()) 
            {
                return;
            }

            FieldInfo[] fields = dst.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                CopyTo(field.GetValue(dst), field.GetValue(Entity));
            }
        }

        void CopyTo(object dst, object src)
        {
            if (dst == null || src == null || src.GetType() != dst.GetType()) 
            {
                return;
            }

            FieldInfo[] fields = dst.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                var type = field.FieldType;
                field.SetValue(dst, field.GetValue(src));
            }
        }
    }


    [Serializable]
    public class EnemyConfig : EntityConfig 
    {
        public override object Entity => entity;
        public Enemy entity = new Enemy();
    }

    [Serializable]
    public class PlayerConfig : EntityConfig 
    {
        public override object Entity => entity;
        public Player entity = new Player();
    }

    [Serializable]
    public class SpawnerConfig : EntityConfig 
    {
        public override object Entity => entity;
        public Spawner entity = new Spawner();
    }


    [Serializable]
    public class CollisionConfig 
    {
        public LVector3 pos;
        public LFloat worldSize = new LFloat(60);
        public LFloat minNodeSize = new LFloat(1);
        public LFloat loosenessval = new LFloat(true, 1250);

        public LFloat percent = new LFloat(true, 100);
        public int count = 100;

        public int showTreeId = 0;

        public Vector2 scrollPos;
        public bool isShow = true;
        public bool[] collisionMatrix = new bool[(int) EColliderLayer.EnumCount * (int) EColliderLayer.EnumCount];

        private string[] _colliderLayerNames;

        public string[] ColliderLayerNames 
        {
            get 
            {
                if (_colliderLayerNames == null || _colliderLayerNames.Length == 0) 
                {
                    var lst = new List<string>();
                    for (int i = 0; i < (int) EColliderLayer.EnumCount; i++) 
                    {
                        lst.Add(((EColliderLayer) i).ToString());
                    }

                    _colliderLayerNames = lst.ToArray();
                }

                return _colliderLayerNames;
            }
        }

        public void SetColliderPair(int a, int b, bool val)
        {
            collisionMatrix[a * (int) EColliderLayer.EnumCount + b] = val;
            collisionMatrix[b * (int) EColliderLayer.EnumCount + a] = val;
        }

        public bool GetColliderPair(int a, int b)
        {
            return collisionMatrix[a * (int) EColliderLayer.EnumCount + b];
        }
    }


    [CreateAssetMenu(menuName = "GameConfig")]
    public class GameConfig : ScriptableObject 
    {
        public CollisionConfig CollisionConfig;
        public string RecorderFilePath;
        public string DumpStrPath;

        public List<PlayerConfig> player = new List<PlayerConfig>();
        public List<EnemyConfig> enemies = new List<EnemyConfig>();
        public List<SpawnerConfig> spawner = new List<SpawnerConfig>();
        public SkillConfig SkillCofnig = new SkillConfig();


        private T GetConfig<T>(List<T> lst, int id) where T: EntityConfig
        {
            if (id < 0 || id >= lst.Count) 
            {
                return null;
            }

            return lst[id];
        }

        public EntityConfig GetEnemyConfig(int id){return  GetConfig(enemies, id);}
        public EntityConfig GetPlayerConfig(int id){return  GetConfig(player, id);}
        public EntityConfig GetSpawnerConfig(int id){return  GetConfig(spawner, id);}   

        public SkillConfig GetSkillConfig()
        {
            return SkillCofnig;
        }
    }
}