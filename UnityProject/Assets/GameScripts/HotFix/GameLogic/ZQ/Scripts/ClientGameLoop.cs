using System.Collections.Generic;
using GameBase;
using TEngine;
using UnityEngine;
using C2DS;
using Google.Protobuf;
using System.Linq;
using Sirenix.Utilities;


namespace Lockstep.Game
{
    public class PlayerInfo
    {
        public string ProfileId;
        public int ProfileIdHash;
        public int PlayerIndex;
    }

    public class ClientGameLoop : Singleton<ClientGameLoop>
    {
        private enum State
        {
            Start,
            JoinServer,
            Playing,
            End
        }

        private const string _ip = "127.0.0.1";
        private const ushort _port = 10600;
        private const float _networkUpdateInterval = 0.02f;
        private float _tickTimer = 0.0f;

        private World _world;
        private NetworkModule _networkModule = null;
        private StateMachine<State> _stateMachine;

        private PlayerInfo _playerInfo = new PlayerInfo();

        protected override void Init()
        {
            InputManager.Init();

            _networkModule = new NetworkModule(_ip, _port);
            _networkModule.Init();
            _networkModule.RegisterMessage((ushort)C2DS_MSG_ID.IdC2DsJoinServerRes, typeof(C2DSJoinServerRes), OnMsgJoinServerRes);
            _networkModule.RegisterMessage((ushort)C2DS_MSG_ID.IdDs2CStartGameReq, typeof(DS2CStartGameReq), OnMsgGameStartReq);
            _networkModule.RegisterMessage((ushort)C2DS_MSG_ID.IdDs2CServerFrameReq, typeof(DS2CServerFrameReq), OnMsgServerFrameReq);

            _world = new World(_networkModule);

            _stateMachine = new StateMachine<State>();
            _stateMachine.Add(State.Start, null, UpdateStart, null);
            _stateMachine.Add(State.JoinServer, EnterJoinServer, null, null);
            _stateMachine.Add(State.Playing, null, UpdatePlaying, null);
            _stateMachine.Add(State.End, null, () => { }, null);
            _stateMachine.SwitchTo(State.Start);

            _playerInfo.ProfileId = "1234";
            _playerInfo.ProfileIdHash = _playerInfo.ProfileId.GetHashCode();
        }

        public void CreateGame()
        {
            _world.Init();
            Log.Info($"Game Start");
        }

        public void Update()
        {
            UpdateNetwork();
            _stateMachine.Update();
        }

        private void UpdateNetwork()
        {
            long timeNow = TimeHelper.TimeStampNowMs();
            _networkModule.Update(timeNow);
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

        private void UpdateStart()
        {
            if (_networkModule.IsConnected) 
            {
                _stateMachine.SwitchTo(State.JoinServer);
            }
        }

        private void EnterJoinServer()
        {
            C2DS.C2DSJoinServerReq req = new C2DS.C2DSJoinServerReq();
            req.ProfileId = _playerInfo.ProfileId;
            _networkModule.Send(req,  (ushort)C2DS.C2DS_MSG_ID.IdC2DsJoinServerReq);
        }

        private void UpdatePlaying()
        {
            InputManager.Update();
            //UpdateMockNetwork();
            _world.Update();
        }

        private int ProfileId2Hash(string profileId)
        {
            return profileId.GetHashCode();
        }

#region network msg
        private void OnMsgJoinServerRes(ushort messageId, int rpcId, IMessage message)
        {
            if (message == null || message is not C2DSJoinServerRes res)
            {
                Log.Error($"OnMsgJoinServerRes error: cannot convert message to C2DSJoinServerRes");
                return;
            }

            if (res.ErrorCode != C2DS_ERROR_CODE.Success)
            {
                Log.Error($"OnMsgJoinServerRes: join sever error, errorcode:{res.ErrorCode}.");
                return;
            }

            int playerIndex = res.PlayerIndex;
            _playerInfo.PlayerIndex = playerIndex;
            Log.Info($"OnMsgJoinServerRes: join sever success, player index:{playerIndex}.");
        }

        private void OnMsgGameStartReq(ushort messageId, int rpcId, IMessage message)
        {
            if (message == null || message is not DS2CStartGameReq res)
            {
                Log.Error($"OnGameStartReq error: cannot convert message to DS2CStartGameReq");
                return;
            }

            int playerCount = res.PlayerCount;
            Log.Info($"OnMsgGameStartReq: game start!, playerCount:{playerCount}.");
            _world.OnGameStart(playerCount, _playerInfo.PlayerIndex);
            _stateMachine.SwitchTo(State.Playing);
        }

        private void OnMsgServerFrameReq(ushort messageId, int rpcId, IMessage message)
        {
            if (message == null || message is not DS2CServerFrameReq req)
            {
                Log.Error($"OnServerFrameReq error: cannot convert message to DS2CServerFrameReq");
                return;
            }

            int startTick = req.StartTick;
            List<C2DS.ServerFrame> msgframes = req.ServrFrame.ToList();
            List<ServerFrame> serverFrames = new List<ServerFrame>();
            foreach(var msgframe in msgframes) 
            {
                serverFrames.Add(MsgServerFrame2GameFrame(msgframe));
            }

            _world.OnServerFrameRecieved(serverFrames);
        }

        private PlayerCommand MsgInput2GameInput(C2DS.PlayerInput msgInpt)
        {
            PlayerCommand playerCommand = default;
            playerCommand.Tick = msgInpt.Tick;
            playerCommand.inputUV._x = msgInpt.Horizontal;
            playerCommand.inputUV._y = msgInpt.Vertical;
            playerCommand.ButtonField = msgInpt.Button;
            playerCommand.EntityId = (ProfileId2Hash(msgInpt.ProfileId));
            return playerCommand;
        }

        private ServerFrame MsgServerFrame2GameFrame(C2DS.ServerFrame msgFrame)
        {
            ServerFrame serverFrame = new ServerFrame();
            serverFrame.tick = msgFrame.Tick;
            serverFrame.Inputs = new PlayerCommand[msgFrame.PlayerInputs.Count];

            List<PlayerCommand> inputs = new List<PlayerCommand ();
            foreach (var msgInput in msgFrame.PlayerInputs)
            {
                inputs.Add(MsgInput2GameInput(msgInput));
            }

            serverFrame.Inputs.AddRange(inputs);
            return serverFrame;
        }
        #endregion network msg
    }
}