using BG.GameServer.Internals;
using BG.GameServer.Messages;
using BG.GameServer.Models;
using BG.GameServer.Network;
using BG.GameServer.ServerGameContents;
using Dignus.Actor.Abstractions;
using Dignus.Actor.Core;
using Dignus.Collections;
using Dignus.DependencyInjection.Extensions;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BG.GameServer.Actors
{
    internal class RoomManagerActor(IServiceProvider serviceProvider) : ActorBase
    {
        private readonly Dictionary<int, IActorRef> _rooms = [];

        private readonly Dictionary<IActorRef, RoomSummary> _roomSummaries = [];

        private readonly UniqueSet<int> _roomNumbers = [];

        private const int InvalidRoomId = -1;

        private Broadcaster _broadcaster = serviceProvider.GetService<Broadcaster>();
        private int GenerateRoomNumber()
        {
            for (int i = 0; i < 10; ++i)
            {
                var roomNumber = Random.Shared.Next(1000, 99999);

                if (_roomNumbers.Contains(roomNumber) == false)
                {
                    if (_roomNumbers.Add(roomNumber) == true)
                    {
                        return roomNumber;
                    }
                }
            }
            return InvalidRoomId;
        }

        public ValueTask HandleGetRoomList(PlayerMessage<GetRoomList> message, IActorRef sender)
        {
            var page = message.Value.Page;
            var size = message.Value.ItemSize;

            var rooms = _roomSummaries.Values.Skip(page * size).Take(size);

            GetRoomListResponse responsePacket = new()
            {
                Page = page,
                RoomList = []
            };

            foreach(var item in rooms)
            {
                if(item.IsStarted)
                {
                    continue;
                }

                responsePacket.RoomList.Add(new RoomInfo()
                {
                    GameType = item.GameType,
                    MemberCount = item.CurrentUserCount,
                    RoomId = item.RoomId,
                    RoomMode = item.RoomMode
                });
            }

            message.Player.SessionRef.Send(Packet.MakePacket(GSCProtocol.GetRoomListResponse,
                responsePacket));

            return ValueTask.CompletedTask;
        }

        private int CreateGameRoom(GameType gameType, RoomMode roomMode)
        {
            var roomId = -1;
            if (gameType == GameType.Max)
            {
                return InvalidRoomId;
            }

            roomId = GenerateRoomNumber();

            if (_rooms.ContainsKey(roomId))
            {
                return InvalidRoomId;
            }

            var actorSystem = serviceProvider.GetService<ActorSystem>();
            IActorRef roomActorRef = null;
            var maxUserCount = 0;
            if (gameType == GameType.WallGo)
            {
                maxUserCount = WallGoRoom.MaxPlayerCount;
                roomActorRef = actorSystem.Spawn(() => new WallGoRoom(roomId, Self));
            }

            if (roomActorRef == null)
            {
                return InvalidRoomId;
            }
            var roomSummary = new RoomSummary(roomId, roomMode, gameType, 0, maxUserCount);

            _rooms[roomId] = roomActorRef;
            _roomSummaries[roomActorRef] = roomSummary;

            return roomId;
        }
        public async ValueTask HandleCreateRoom(PlayerMessage<CreateRoom> message, IActorRef sender)
        {
            var player = message.Player;
            if (player == null || player.RoomActorRef != null)
            {
                sender.Post(new KickUserMessage(ErrorCode.InvalidRequest), Self);

                return;
            }

            var request = message.Value;

            var roomId = CreateGameRoom((GameType)request.GameType, request.RoomMode);

            if(roomId == InvalidRoomId)
            {
                message.Player.Send(Packet.MakePacket(GSCProtocol.CreateRoomResponse,
                    new CreateRoomResponse()
                    {
                        Ok = false,
                    }));

                return;
            }

            message.Player.Send(Packet.MakePacket(GSCProtocol.CreateRoomResponse,
                    new CreateRoomResponse()
                    {
                        Ok = true,
                        RoomNumber = roomId
                    }));


            var joinRoom = new JoinRoom()
            {
                RoomMode = request.RoomMode,
                RoomNumber = roomId
            };

            await HandleJoinRoom(new PlayerMessage<JoinRoom>(joinRoom, player), sender);
        }
        public ValueTask HandleJoinRoom(PlayerMessage<JoinRoom> message, IActorRef sender)
        {
            var player = message.Player;
            if (player == null || player.RoomActorRef != null)
            {
                sender.Post(new KickUserMessage(ErrorCode.InvalidRequest), Self);
                return ValueTask.CompletedTask;
            }

            var request = message.Value;

            if(_rooms.TryGetValue(request.RoomNumber, out var roomActorRef) == false)
            {
                player.Send(Packet.MakePacket(GSCProtocol.JoinRoomResponse,
                        new JoinRoomResponse()
                        {
                            FailedJoinRoomReason = JoinRoomReason.NotFound,
                        }));

                return ValueTask.CompletedTask;
            }

            roomActorRef.Post(new JoinMemberMessage(player));

            return ValueTask.CompletedTask;
        }
        private ValueTask HandleUpdateParticipantCount(RoomStateUpdated message, IActorRef sender)
        {
            if(_roomSummaries.TryGetValue(sender, out var roomSummary) == false)
            {
                sender.Kill();
                return ValueTask.CompletedTask;
            }

            roomSummary.CurrentUserCount = message.CurrentUserCount;
            roomSummary.IsStarted = message.IsStarted;

            if (roomSummary.CurrentUserCount == 0)
            {
                _rooms.Remove(roomSummary.RoomId);
                _roomSummaries.Remove(sender);
                var packet = Packet.MakePacket(GSCProtocol.RemoveGameRoom, new RemoveGameRoom()
                {
                    RoomNumber = roomSummary.RoomId
                });
                _broadcaster.EnqueueBroadcast(packet);
                sender.Kill();
            }

            return ValueTask.CompletedTask;
        }

        protected override ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {
            return message switch
            {
                PlayerMessage<GetRoomList> request => HandleGetRoomList(request, sender),
                PlayerMessage<CreateRoom> request => HandleCreateRoom(request, sender),
                PlayerMessage<JoinRoom> request => HandleJoinRoom(request, sender),
                RoomStateUpdated request => HandleUpdateParticipantCount(request, sender),
                _ => ValueTask.CompletedTask
            };
        }
    }
}
