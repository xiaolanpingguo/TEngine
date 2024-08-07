﻿using System;
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
        private CollisionSystem _collisionSystem;

        private static Dictionary<int, ColliderPrefab> _entityId2Collider = new Dictionary<int, ColliderPrefab>();
        private static Dictionary<ILPTriggerEventHandler, ColliderProxy> _mono2ColProxy = new ();
        private static Dictionary<ColliderProxy, ILPTriggerEventHandler> _colProxy2Mono = new ();

        private int[] _allTypes = new int[] { 0, 1, 2 };

        public CollisionConfig config;
        public bool[] collisionMatrix => config.collisionMatrix;
        public LVector3 pos => config.pos;
        public LFloat worldSize => config.worldSize;
        public LFloat minNodeSize => config.minNodeSize;
        public LFloat loosenessval => config.loosenessval;

        public int showTreeId = 0;

        public PhysicSystem(World world) : base(world) { }

        public override void Init()
        {
            config = GameConfigSingleton.Instance.CollisionConfig;
            _collisionSystem = new CollisionSystem()
            {
                worldSize = worldSize,
                pos = pos,
                minNodeSize = minNodeSize,
                loosenessval = loosenessval
            };

            _collisionSystem.Start(collisionMatrix, _allTypes);
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

        public ColliderProxy GetCollider(int id)
        {
            return _collisionSystem.GetCollider(id);
        }

        public bool Raycast(int layerMask, Ray2D ray, out LRaycastHit2D ret)
        {
            return Raycast(layerMask, ray, out ret, LFloat.MaxValue);
        }

        public bool Raycast(int layerMask, Ray2D ray, out LRaycastHit2D ret, LFloat maxDistance)
        {
            ret = new LRaycastHit2D();
            LFloat t = LFloat.one; 
            int id;
            if (DoRaycast(layerMask, ray, out t, out id, maxDistance)) 
            {
                ret.point = ray.origin + ray.direction * t;
                ret.distance = t * ray.direction.magnitude;
                ret.colliderId = id;
                return true;
            }

            return false;
        }

        public void QueryRegion(int layerType, LVector2 pos, LVector2 size, LVector2 forward, FuncCollision callback)
        {
            _QueryRegion(layerType, pos, size, forward, callback);
        }

        public void QueryRegion(int layerType, LVector2 pos, LFloat radius, FuncCollision callback)
        {
            _QueryRegion(layerType, pos, radius, callback);
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

        public void RegisterEntity(int entityId, Entity entity, int layer)
        {
            ColliderPrefab collider = null;
            if (!_entityId2Collider.TryGetValue(entityId, out collider)) 
            {
                collider = CreateColliderPrefab(entity.ColliderData);
            }
            else
            {
                _entityId2Collider[entityId] = collider;
            }

            AttachToColSystem(layer, collider,  entity);
        }

        public ColliderPrefab CreateColliderPrefab(ColliderData data)
        {
            if (data == null)
            {
                return null;
            }

            CBaseShape collider = null;
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