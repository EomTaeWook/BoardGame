using BG.GameServer.ServerGameContents;
using Dignus.Actor.Core.Messages;
using Protocol.GSAndClient;
using Protocol.GSAndClient.Models;

namespace BG.GameServer.Messages
{
    internal readonly record struct KickUserMessage(ErrorCode Reason) : IActorMessage;
    internal readonly record struct ActorMessage<TMessage>(TMessage Value) : IActorMessage;
    internal readonly record struct PlayerMessage<TMessage>(TMessage Value, Player Player) : IActorMessage;

    internal readonly record struct AcceptedMessage() : IActorMessage;

    internal record JoinMemberMessage(Player Player) : IActorMessage;
    internal record LeaveMemberMessage(Player Player) : IActorMessage;
    internal record RoomStateUpdated(int RoomId, int CurrentUserCount, bool IsStarted) : IActorMessage;
    internal record StartGameRoomMessage(Player Player) : IActorMessage;
    internal record StartGameMessage(GameType GameType, StartGameRoomReason StartGameRoomReason) : IActorMessage;
}
