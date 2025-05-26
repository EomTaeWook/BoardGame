using BG.GameServer.ServerContents;
using Dignus.DependencyInjection.Attributes;
using Dignus.Log;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using System.Text.Json;

namespace BG.GameServer.Network.Handlers
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    internal class CGProtocolHandler : IProtocolHandler<string>, ISessionComponent
    {
        private RobbyManager _robbyManager;
        private Player _player;
        private ISession _session;
        private HeartBeat _heartBeat;
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
                _robbyManager.TryRemovePlayer(_player);
            }

            if (_heartBeat != null)
            {
                _heartBeat.Dispose();
                _heartBeat = null;
            }

            _player = null;
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
            var isStart = room.StartGame();

            room.Broadcast(Packet.MakePacket(GSCProtocol.StartGameRoomResponse, new StartGameRoomResponse()
            {
                Ok = isStart,
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
            if (_robbyManager.TryCreateGameRoom(gameType, out room) == false)
            {
                _player.Send(Packet.MakePacket(GSCProtocol.CreateRoomResponse,
                    new CreateRoomResponse()
                    {
                        Ok = false,
                    }));
            }
            else
            {
                _player.Send(Packet.MakePacket(GSCProtocol.CreateRoomResponse,
                    new CreateRoomResponse()
                    {
                        Ok = true,
                        RoomNumber = room.RoomNumber
                    }));

                JoinRoom(new Protocol.GSAndClient.JoinRoom()
                {
                    RoomNumber = room.RoomNumber
                });
            }
        }
        public void JoinRoom(JoinRoom joinRoom)
        {
            if (_player == null)
            {
                _session.Dispose();
                return;
            }

            if (_robbyManager.TryGetGameRoom(joinRoom.RoomNumber, out var room) == false)
            {
                _player.Send(Packet.MakePacket(GSCProtocol.JoinRoomResponse,
                    new JoinRoomResponse()
                    {
                        Ok = false,
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
                    Ok = true,
                    Members = roomMembers
                }));
            }
            else
            {
                room.Broadcast(Packet.MakePacket(GSCProtocol.JoinRoomResponse, new JoinRoomResponse()
                {
                    Ok = false,
                }));
            }
        }
        public void LeaveRoom(LeaveRoom _)
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
                _robbyManager.RemoveRoom(room.RoomNumber);
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
            _player = new Player(login.AccountId, login.Nickname, _session);

            if (_robbyManager.TryAddPlayer(_player) == false)
            {
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
                Ok = true,
            }));
        }

        void ISessionComponent.SetSession(ISession session)
        {
            _session = session;

            foreach (var component in _session.GetSessionComponents())
            {
                if (component is RobbyManager robbyManager)
                {
                    _robbyManager = robbyManager;
                }
                else if (component is HeartBeat heartBeat)
                {
                    _heartBeat = heartBeat;
                }
            }


            if (_heartBeat != null)
            {
                _ = _heartBeat.SendPingAsync((ushort)GSCProtocol.Ping);
            }
        }
    }
}
