using BG.GameServer.Internals;
using BG.GameServer.ServerGameContents;
using Dignus.DependencyInjection.Attributes;
using Dignus.Log;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace BG.GameServer.Network.Handlers
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    internal class CGProtocolHandler(LobbyManager lobbyManager,
        SessionManager sessionManager,
        Broadcaster broadcaster) : IProtocolHandler<string>, ISessionComponent
    {
        private HeartBeat _heartBeat;
        private Player _player;
        private ISession _session;
        public T DeserializeBody<T>(string body)
        {
            return JsonSerializer.Deserialize<T>(body);
        }
        void ISessionComponent.Dispose()
        {
            if (_player != null)
            {
                var room = _player.Room;
                if (room != null)
                {
                    LeaveRoom(new LeaveRoom());
                }
                sessionManager.TryRemovePlayer(_player);
            }
            _player = null;
        }
        public void GetRoomList(GetRoomList getRoomList)
        {
            if (_player == null)
            {
                _session.Dispose();
                return;
            }
            var roomList = lobbyManager.GetRooms(getRoomList.Page, getRoomList.ItemSize);

            var roomInfos = new List<RoomInfo>();

            foreach (var item in roomList)
            {
                roomInfos.Add(new RoomInfo()
                {
                    GameType = item.GameType,
                    MemberCount = item.GetMembers().Count,
                    RoomId = item.RoomNumber
                });
            }
            _player.Send(Packet.MakePacket(GSCProtocol.GetRoomListResponse, new GetRoomListResponse()
            {
                Page = getRoomList.Page,
                RoomList = roomInfos
            }));
        }
        public void StartGameRoom(StartGameRoom _)
        {
            if (_player == null)
            {
                _session.Dispose();
                return;
            }

            var room = _player.Room;

            if (room == null)
            {
                _session.Dispose();
                return;
            }

            if (room.Host.AccountId != _player.AccountId)
            {
                return;
            }

            var startRoomReason = room.StartGame();

            room.Broadcast(Packet.MakePacket(GSCProtocol.StartGameRoomResponse, new StartGameRoomResponse()
            {
                StartGameRoomReason = startRoomReason,
                GameType = room.GameType,
            }));
        }
        public void CreateRoom(CreateRoom createRoom)
        {
            if (_player == null)
            {
                _session.Dispose();
                return;
            }

            var room = _player.Room;

            if (room != null)
            {
                _player.Send(Packet.MakePacket(GSCProtocol.CreateRoomResponse,
                    new CreateRoomResponse()
                    {
                        Ok = false,
                    }));

                return;
            }

            var gameType = (GameType)createRoom.GameType;


            if (createRoom.RoomMode == RoomMode.Public)
            {
                lobbyManager.TryCreateGameRoom(gameType, out room);
            }
            else if (createRoom.RoomMode == RoomMode.Private)
            {
                lobbyManager.TryCreatePrivateGameRoom(gameType, out room);
            }
            else
            {
                LogHelper.Error($"Invalid create room request. unsupported RoomMode : {createRoom.RoomMode}");
                _session.Dispose();
                return;
            }

            if (room == null)
            {
                _player.Send(Packet.MakePacket(GSCProtocol.CreateRoomResponse,
                    new CreateRoomResponse()
                    {
                        Ok = false,
                    }));

                return;
            }

            _player.Send(Packet.MakePacket(GSCProtocol.CreateRoomResponse,
                    new CreateRoomResponse()
                    {
                        Ok = true,
                        RoomNumber = room.RoomNumber
                    }));

            JoinRoom(new JoinRoom()
            {
                RoomMode = createRoom.RoomMode,
                RoomNumber = room.RoomNumber
            });
        }
        public void JoinRoom(JoinRoom joinRoom)
        {
            if (_player == null)
            {
                _session.Dispose();
                return;
            }

            if (lobbyManager.TryGetGameRoom(joinRoom.RoomNumber, joinRoom.RoomMode, out var room) == false)
            {
                _player.Send(Packet.MakePacket(GSCProtocol.JoinRoomResponse,
                    new JoinRoomResponse()
                    {
                        FailedJoinRoomReason = JoinRoomReason.NotFound,
                    }));

                return;
            }

            if (room.Join(_player))
            {
                var roomMembers = new List<PlayerModel>();

                foreach (var member in room.GetMembers())
                {
                    var isHost = room.Host == member;
                    roomMembers.Add(new PlayerModel()
                    {
                        AccountId = member.AccountId,
                        IsHost = isHost,
                        Nickname = member.Nickname,
                    });
                }

                room.Broadcast(Packet.MakePacket(GSCProtocol.JoinRoomResponse, new JoinRoomResponse()
                {
                    FailedJoinRoomReason = JoinRoomReason.Success,
                    Members = roomMembers
                }));
            }
            else
            {
                room.Broadcast(Packet.MakePacket(GSCProtocol.JoinRoomResponse, new JoinRoomResponse()
                {
                    FailedJoinRoomReason = JoinRoomReason.IsFull,
                }));
            }
        }
        public void LeaveRoom(LeaveRoom leaveRoom)
        {
            if (_player == null)
            {
                _session.Dispose();
                return;
            }
            var room = _player.Room;
            if (room == null)
            {
                return;
            }

            room.Leave(_player);

            var roomMembers = new List<PlayerModel>();

            foreach (var member in room.GetMembers())
            {
                var isHost = room.Host == member;
                roomMembers.Add(new PlayerModel()
                {
                    AccountId = member.AccountId,
                    IsHost = isHost,
                    Nickname = member.Nickname,
                });
            }

            room.Broadcast(Packet.MakePacket(GSCProtocol.LeaveRoomResponse, new LeaveRoomResponse()
            {
                Members = roomMembers
            }));

            _player.SetRoom(null);

            if (room.IsEmpty())
            {
                var roomNumber = room.RoomNumber;
                lobbyManager.RemoveRoom(leaveRoom.RoomMode, roomNumber);
                room.Dispose();

                var packet = Packet.MakePacket((ushort)PacketCategory.Lobby,
                    (int)GSCProtocol.RemoveGameRoom,
                    new RemoveGameRoom()
                    {
                        RoomNumber = roomNumber
                    });

                broadcaster.EnqueueBroadcast(packet);
            }
        }

        public void Login(Login login)
        {
            if (string.IsNullOrEmpty(login.AccountId))
            {
                LogHelper.Error($"account Id is empty");
                _session.Dispose();
                return;
            }

            if (string.IsNullOrEmpty(login.Nickname))
            {
                _session.Dispose();
                return;
            }

            var alreadyLoginPlayer = sessionManager.GetPlayer(login.AccountId);
            if (alreadyLoginPlayer != null)
            {
                alreadyLoginPlayer.Send(Packet.MakePacket(GSCProtocol.LoginResponse, new LoginResponse()
                {
                    LoginReason = LoginReason.DuplicateLogin,
                }));
                alreadyLoginPlayer.Close();
                sessionManager.TryRemovePlayer(alreadyLoginPlayer);
            }

            _player = new Player(login.AccountId, login.Nickname, _session);

            if (sessionManager.TryAddPlayer(_player) == false)
            {
                _player.Send(Packet.MakePacket(GSCProtocol.LoginResponse, new LoginResponse()
                {
                    LoginReason = LoginReason.DuplicateLogin,
                }));

                _session.Dispose();
                return;
            }

            foreach (var component in _session.GetSessionComponents())
            {
                if (component is IPlayerComponent playerComponent)
                {
                    playerComponent.SetPlayer(_player);
                }
            }

            _player.Send(Packet.MakePacket(GSCProtocol.LoginResponse, new LoginResponse()
            {
                LoginReason = LoginReason.Success,
            }));
        }
        public void Pong(Pong pong)
        {
            if(_heartBeat != null)
            {
                _heartBeat.Pong();
            }
        }

        void ISessionComponent.SetSession(ISession session)
        {
            _session = session;

            foreach (var component in _session.GetSessionComponents())
            {
                if (component is HeartBeat heartBeat)
                {
                    _heartBeat = heartBeat;
                }
            }
        }
    }
}
