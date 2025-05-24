using Assets.Scripts.GameContents.Share;
using BG.GameServer.ServerContents;
using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System.Text.Json;

namespace BG.GameServer.Network.Handlers
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    internal class CGProtocolHandler : IProtocolHandler<string>, ISessionComponent
    {
        private RobbyManager _robbyManager;
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
                    room.Leave(_player);

                    room.Broadcast(Packet.MakePacket(GSCProtocol.LeaveRoomResponse, new LeaveRoomResponse()
                    {
                        AccountId = _player.AccountId,
                    }));
                }

                _robbyManager.TryRemovePlayer(_player);
            }
            _player = null;
        }
        public void CreateRoom(CreateRoom createRoom)
        {
            if (_player == null)
            {
                _session.Dispose();
                return;
            }
            var gameType = (GameType)createRoom.GameType;
            if (_robbyManager.TryCreateGameRoom(gameType, out var room) == false)
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
                room.Broadcast(Packet.MakePacket(GSCProtocol.JoinRoomResponse, new JoinRoomResponse()
                {
                    Ok = true,
                    AccountId = _player.AccountId,
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
            room.Broadcast(Packet.MakePacket(GSCProtocol.LeaveRoomResponse, new LeaveRoomResponse()
            {
                AccountId = _player.AccountId,
            }));
        }

        public void Login(Login login)
        {
            if (string.IsNullOrEmpty(login.AccountId))
            {
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
            }
        }
    }
}
