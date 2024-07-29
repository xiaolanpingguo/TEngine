using Lockstep.Framework;
using GameBase;
using TEngine;
using UnityEngine;


namespace Lockstep.Game
{
    public class LocalGameLoop : Singleton<LocalGameLoop>
    {
        private const float _networkUpdateInterval = 0.02f;
        private float _tickTimer = 0.0f;

        private World _world;

        protected override void Init()
        {
            _world = new World(null);
            InputManager.Init();
        }

        public void CreateGame()
        {
            _world.Init();
            Log.Info($"Game Start");
        }

        public void Update()
        {
            InputManager.Update();
            UpdateMockNetwork();
            _world.Update();
        }

        private void UpdateMockNetwork()
        {
            float delta = Time.deltaTime;
            _tickTimer += delta;
            if (_tickTimer <= _networkUpdateInterval) 
            {
                return;
            }
            _tickTimer = 0;

            var input = InputManager.CurrentInput;
            input.Tick = _world.Tick;
            input.EntityId = _world.LocalPlayerId;
            _world.PushServerFrame(_world.Tick, new PlayerCommand[] { input });
        }
    }
}