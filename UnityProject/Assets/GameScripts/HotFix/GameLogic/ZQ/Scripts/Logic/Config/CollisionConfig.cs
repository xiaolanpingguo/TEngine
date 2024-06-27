using Lockstep.Framework;
using UnityEngine;
using System.Collections.Generic;


namespace Lockstep.Game
{
    public enum EColliderLayer
    {
        Static,
        Enemy,
        Player,
        EnumCount
    }

    public class CollisionConfig : ScriptableObject
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
        public bool[] collisionMatrix = new bool[(int)EColliderLayer.EnumCount * (int)EColliderLayer.EnumCount];

        private string[] _colliderLayerNames;

        public string[] ColliderLayerNames
        {
            get
            {
                if (_colliderLayerNames == null || _colliderLayerNames.Length == 0)
                {
                    var lst = new List<string>();
                    for (int i = 0; i < (int)EColliderLayer.EnumCount; i++)
                    {
                        lst.Add(((EColliderLayer)i).ToString());
                    }

                    _colliderLayerNames = lst.ToArray();
                }

                return _colliderLayerNames;
            }
        }

        public void SetColliderPair(int a, int b, bool val)
        {
            collisionMatrix[a * (int)EColliderLayer.EnumCount + b] = val;
            collisionMatrix[b * (int)EColliderLayer.EnumCount + a] = val;
        }

        public bool GetColliderPair(int a, int b)
        {
            return collisionMatrix[a * (int)EColliderLayer.EnumCount + b];
        }
    }
}