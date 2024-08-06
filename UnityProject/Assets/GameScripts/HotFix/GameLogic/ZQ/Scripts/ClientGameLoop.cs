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

        private PlayerInfo _myPlayerInfo = new PlayerInfo();

        protected override void Init()
        {
            InputManager.Init();

            _networkModule = new NetworkModule(_ip, _port);
            _networkModule.Init();
            _networkModule.RegisterMessage((ushort)C2DS_MSG_ID.IdC2DsJoinServerRes, typeof(C2DSJoinServerRes), OnMsgJoinServerRes);
            _networkModule.RegisterMessage((ushort)C2DS_MSG_ID.IdDs2CStartGameReq, typeof(DS2CStartGameReq), OnMsgGameStartReq);

            _world = new World(_networkModule);

            _stateMachine = new StateMachine<State>();
            _stateMachine.Add(State.Start, null, UpdateStart, null);
            _stateMachine.Add(State.JoinServer, EnterJoinServer, null, null);
            _stateMachine.Add(State.Playing, null, UpdatePlaying, null);
            _stateMachine.Add(State.End, null, () => { }, null);
            _stateMachine.SwitchTo(State.Start);

            _myPlayerInfo.ProfileId = "1234";
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
            req.ProfileId = _myPlayerInfo.ProfileId;
            _networkModule.Send(req, (ushort)C2DS.C2DS_MSG_ID.IdC2DsJoinServerReq);
        }

        private void UpdatePlaying()
        {
            InputManager.Update();
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

            Log.Info($"OnMsgJoinServerRes: join sever success, player profileId:{_myPlayerInfo.ProfileId}.");
        }

        private void OnMsgGameStartReq(ushort messageId, int rpcId, IMessage message)
        {
            if (message == null || message is not DS2CStartGameReq res)
            {
                Log.Error($"OnGameStartReq error: cannot convert message to DS2CStartGameReq");
                return;
            }

            if (res.Players.Count == 0)
            {
                Log.Error($"OnGameStartReq error: res.Players.Count == 0");
                return;
            }

            int playerCount = res.Players.Count;
            PlayerInfo[] players = new PlayerInfo[playerCount];
            bool found = false;
            for(int i = 0; i < playerCount; ++i)
            {
                C2DS.PlayerInfo msgPlayerInfo = res.Players[i];

                PlayerInfo info = new PlayerInfo();
                info.ProfileId = msgPlayerInfo.ProfileId;
                info.PlayerIndex = i;
                players[i] = info;
                if (info.ProfileId == _myPlayerInfo.ProfileId)
                {
                    found = true;
                }
            }
   
            if (!found) 
            {
                Log.Error($"OnGameStartReq error: can't found myself, profileId:{_myPlayerInfo.ProfileId}");
                return;
            }

            Log.Info($"OnMsgGameStartReq: game start!, playerCount:{playerCount}.");
            _world.OnGameStart(players, _myPlayerInfo);
            _stateMachine.SwitchTo(State.Playing);
        }
        #endregion network msg
    }
}