using System;
using System.Collections.Generic;
using Lockstep.Framework;
using UnityEngine;
using UnityEngine.Profiling;
using Ray2D = Lockstep.Framework.Ray2D;
using TEngine;


namespace Lockstep.Game 
{
    public class PhysicSystem : IGameSystem 
    {
        private static PhysicSystem _instance;
        public static PhysicSystem Instance => _instance;
        private CollisionSystem _collisionSystem;
        public CollisionConfig config;

        static Dictionary<int, ColliderPrefab> _fabId2ColPrefab = new Dictionary<int, ColliderPrefab>();
        static Dictionary<int, int> _fabId2Layer = new Dictionary<int, int>();

        static Dictionary<ILPTriggerEventHandler, ColliderProxy> _mono2ColProxy = new ();
        static Dictionary<ColliderProxy, ILPTriggerEventHandler> _colProxy2Mono = new ();

        public bool[] collisionMatrix => config.collisionMatrix;
        public LVector3 pos => config.pos;
        public LFloat worldSize => config.worldSize;
        public LFloat minNodeSize => config.minNodeSize;
        public LFloat loosenessval => config.loosenessval;

        private int[] allTypes = new int[] {0, 1, 2};

        public int showTreeId = 0;

        public override void Init()
        {
            _instance = this;
            config = GameConfigSingleton.Instance.CollisionConfig;

            if (_instance != this)
            {
               Log.Error("Duplicate CollisionSystemAdapt!");
                return;
            }

            _collisionSystem = new CollisionSystem()
            {
                worldSize = worldSize,
                pos = pos,
                minNodeSize = minNodeSize,
                loosenessval = loosenessval
            };

            _collisionSystem.Start(collisionMatrix, allTypes);
            _collisionSystem.funcGlobalOnTriggerEvent += GlobalOnTriggerEvent;
        }

        public override void Update(LFloat deltaTime)
        {
            _collisionSystem.ShowTreeId = showTreeId;
            _collisionSystem.Update(deltaTime);
        }

        public static void GlobalOnTriggerEvent(ColliderProxy a, ColliderProxy b, ECollisionEvent type)
        {
            if (_colProxy2Mono.TryGetValue(a, out var handlera)) 
            {
                CollisionSystem.TriggerEvent(handlera, b, type);
            }

            if (_colProxy2Mono.TryGetValue(b, out var handlerb)) 
            {
                CollisionSystem.TriggerEvent(handlerb, a, type);
            }
        }

        public static ColliderProxy GetCollider(int id)
        {
            return _instance._collisionSystem.GetCollider(id);
        }

        public static bool Raycast(int layerMask, Ray2D ray, out LRaycastHit2D ret)
        {
            return Raycast(layerMask, ray, out ret, LFloat.MaxValue);
        }

        public static bool Raycast(int layerMask, Ray2D ray, out LRaycastHit2D ret, LFloat maxDistance)
        {
            ret = new LRaycastHit2D();
            LFloat t = LFloat.one; 
            int id;
            if (_instance.DoRaycast(layerMask, ray, out t, out id, maxDistance)) 
            {
                ret.point = ray.origin + ray.direction * t;
                ret.distance = t * ray.direction.magnitude;
                ret.colliderId = id;
                return true;
            }

            return false;
        }

        public static void QueryRegion(int layerType, LVector2 pos, LVector2 size, LVector2 forward, FuncCollision callback)
        {
            _instance._QueryRegion(layerType, pos, size, forward, callback);
        }

        public static void QueryRegion(int layerType, LVector2 pos, LFloat radius, FuncCollision callback)
        {
            _instance._QueryRegion(layerType, pos, radius, callback);
        }

        private void _QueryRegion(int layerType, LVector2 pos, LVector2 size, LVector2 forward, FuncCollision callback)
        {
            _collisionSystem.QueryRegion(layerType, pos, size, forward, callback);
        }

        private void _QueryRegion(int layerType, LVector2 pos, LFloat radius, FuncCollision callback)
        {
            _collisionSystem.QueryRegion(layerType, pos, radius, callback);
        }

        public bool DoRaycast(int layerMask, Ray2D ray, out LFloat t, out int id, LFloat maxDistance)
        {
            var ret = _collisionSystem.Raycast(layerMask, ray, out t, out id, maxDistance);
            return ret;
        }

        public void RigisterPrefab(int prefabId, int val)
        {
            _fabId2Layer[prefabId] = val;
        }

        public void RegisterEntity(int prefabId, Entity entity)
        {
            ColliderPrefab prefab = null;
            var fab = World.Instance.LoadPrefab(prefabId);
            if (!_fabId2ColPrefab.TryGetValue(prefabId, out prefab)) 
            {
                prefab = CreateColliderPrefab(fab, entity.colliderData);
            }

            AttachToColSystem(_fabId2Layer[prefabId], prefab,  entity);
        }

        public ColliderPrefab CreateColliderPrefab(GameObject fab, ColliderData data)
        {
            CBaseShape collider = null;
            if (data == null)
            {
                return null;
            }

            if (data.radius > 0)
            {
                //circle
                collider = new CCircle(data.radius);
            }
            else
            {
                //obb
                collider = new COBB(data.size, data.deg);
            }

            var colFab = new ColliderPrefab();
            colFab.parts.Add(new ColliderPart()
            {
                transform = new CTransform2D(LVector2.zero),
                collider = collider
            });

            return colFab;
        }

        public void AttachToColSystem(int layer, ColliderPrefab prefab, Entity entity)
        {
            var proxy = new ColliderProxy();
            proxy.EntityObject = entity;
            proxy.Init(prefab, entity.LTrans2D);
            proxy.IsStatic = false;
            proxy.LayerType = layer;
            var eventHandler = entity;
            if (eventHandler != null)
            {
                _mono2ColProxy[eventHandler] = proxy;
                _colProxy2Mono[proxy] = eventHandler;
            }

            _collisionSystem.AddCollider(proxy);
        }

        public void RemoveCollider(ILPTriggerEventHandler handler)
        {
            if (_mono2ColProxy.TryGetValue(handler, out var proxy)) 
            {
                RemoveCollider(proxy);
                _mono2ColProxy.Remove(handler);
                _colProxy2Mono.Remove(proxy);
            }
        }

        public void RemoveCollider(ColliderProxy collider)
        {
            _collisionSystem.RemoveCollider(collider);
        }
    }
}