using BG.GameServer.Messages;
using BG.GameServer.Network;
using BG.GameServer.ServerGameContents;
using Dignus.Actor.Core;
using Dignus.Actor.Core.Messages;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BG.GameServer.Actors
{
    internal abstract class RoomBaseActor(int roomId, GameType gameType, int maxUserCount, IActorRef roomManagerRef) : ActorBase
    {
        public int MaxUserCount { get; private set; } = maxUserCount;

        public int RoomId { get; private set; } = roomId;

        public Player Host { get => _hostPlayer; }

        protected readonly Dictionary<string, Player> _accountIdToPlayerMap = [];

        private Player _hostPlayer;
        private bool _isStarted;

        private readonly IActorRef _roomManagerRef = roomManagerRef;

        public abstract StartGameRoomReason StartGame();
        public abstract void Dispose();
        protected abstract ValueTask HandleGameMessage(IActorMessage message, IActorRef sender);

        private bool IsFull()
        {
            return _accountIdToPlayerMap.Count >= MaxUserCount;
        }
        public ValueTask HandleLeaveMember(Player player, IActorRef sender)
        {
            if (player == null)
            {
                sender.Post(new KickUserMessage(ErrorCode.InvalidRequest));
                return ValueTask.CompletedTask;
            }

            _accountIdToPlayerMap.Remove(player.AccountId, out player);
            _roomManagerRef.Post(new RoomStateUpdated(RoomId, _accountIdToPlayerMap.Count, _isStarted), Self);

            player.SetRoom(null);

            if (_hostPlayer.AccountId == player.AccountId)
            {
                if (_accountIdToPlayerMap.Count > 0)
                {
                    _hostPlayer = _accountIdToPlayerMap.ElementAt(0).Value;
                }
            }


            var members = new List<PlayerModel>();

            foreach (var member in _accountIdToPlayerMap.Values)
            {
                members.Add(new PlayerModel()
                {
                    AccountId = member.AccountId,
                    Nickname = member.Nickname,
                    IsHost = _hostPlayer.AccountId == member.AccountId,
                });
            }

            Broadcast(Packet.MakePacket(GSCProtocol.LeaveRoomResponse,
                new LeaveRoomResponse()
                {
                    Members = members
                }));

            return ValueTask.CompletedTask;

        }
        private ValueTask HandleJoinMember(Player player, IActorRef sender)
        {
            if (player == null)
            {
                sender.Post(new KickUserMessage(ErrorCode.InvalidRequest));
                return ValueTask.CompletedTask;
            }
            if(_isStarted)
            {
                player.Send(Packet.MakePacket(GSCProtocol.JoinRoomResponse, new JoinRoomResponse()
                {
                    FailedJoinRoomReason = JoinRoomReason.GameAlreadyStarted,
                }));

                return ValueTask.CompletedTask;
            }
            if (IsFull())
            {
                player.Send(Packet.MakePacket(GSCProtocol.JoinRoomResponse, new JoinRoomResponse()
                {
                    FailedJoinRoomReason = JoinRoomReason.IsFull,
                }));

                return ValueTask.CompletedTask;
            }

            var added = _accountIdToPlayerMap.TryAdd(player.AccountId, player);
            if (added)
            {
                if (_accountIdToPlayerMap.Count == 1)
                {
                    _hostPlayer = player;
                }
                player.SetRoom(Self);

                var members = new List<PlayerModel>();

                foreach(var member in _accountIdToPlayerMap.Values)
                {
                    members.Add(new PlayerModel()
                    {
                        AccountId = member.AccountId,
                        Nickname = member.Nickname,
                        IsHost = _hostPlayer.AccountId == member.AccountId,
                    });
                }

                _roomManagerRef.Post(new RoomStateUpdated(RoomId, _accountIdToPlayerMap.Count, _isStarted), Self);

                Broadcast(Packet.MakePacket(GSCProtocol.JoinRoomResponse,
                    new JoinRoomResponse()
                    {
                        FailedJoinRoomReason = JoinRoomReason.Success,
                        Members = members
                    }));
            }
            else
            {
                sender.Post(new KickUserMessage(ErrorCode.InvalidRequest));
            }
            return ValueTask.CompletedTask;
        }
        private ValueTask HandleStartGameRoom(Player player, IActorRef sender)
        {
            if (player == null)
            {
                sender.Post(new KickUserMessage(ErrorCode.InvalidRequest));
                return ValueTask.CompletedTask;
            }

            if(_hostPlayer.AccountId != player.AccountId)
            {
                return ValueTask.CompletedTask;
            }

            var startRoomReason = StartGame();

            if (startRoomReason == StartGameRoomReason.Success)
            {
                _isStarted = true;
            }

            _roomManagerRef.Post(new RoomStateUpdated(RoomId, _accountIdToPlayerMap.Count, _isStarted), Self);

            var startRoomResponse = new StartGameRoomResponse()
            {
                GameType = gameType,
                StartGameRoomReason = startRoomReason,
                RoomId = RoomId
            };

            foreach (var member in _accountIdToPlayerMap.Values)
            {
                member.SessionRef.Post(new PlayerMessage<StartGameRoomResponse>(startRoomResponse, member), Self);
            }



            return ValueTask.CompletedTask;
        }
        protected override ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {
            return message switch
            {
                JoinMemberMessage request => HandleJoinMember(request.Player, sender),
                LeaveMemberMessage request => HandleLeaveMember(request.Player, sender),
                StartGameRoomMessage request => HandleStartGameRoom(request.Player, sender),
                _ => HandleGameMessage(message, sender)
            };
        }

        protected void Broadcast(IPacket packet)
        {
            foreach (var player in _accountIdToPlayerMap.Values)
            {
                player.Send(packet);
            }
        }
        public override void OnKill()
        {
            Broadcast(Packet.MakePacket(GSCProtocol.LeaveRoomResponse,
                    new LeaveRoomResponse()
                    {
                        Members = []
                    }));
            _accountIdToPlayerMap.Clear();
            base.OnKill();
        }
    }
}
