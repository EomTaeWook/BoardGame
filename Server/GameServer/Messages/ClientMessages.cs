using BG.GameServer.ServerGameContents;
using Dignus.Actor.Core.Messages;
using Protocol.GSAndClient;

namespace BG.GameServer.Messages
{
    internal record KickUserMessage(ErrorCode Reason) : IActorMessage;
    internal record GetRoomListRequestMessage(GetRoomList Packet) : IActorMessage;
    internal record GetRoomListResultMessage(GetRoomListResponse Packet) : IActorMessage;
    internal record CreateRoomMessage(CreateRoom Packet, Player Player) : IActorMessage;
    internal record JoinRoomMessage(JoinRoom Packet, Player Player) : IActorMessage;
    internal record JoinMemberMessage(Player Player) : IActorMessage;
    internal record LeaveMemberMessage(Player Player) : IActorMessage;
    internal record UpdateParticipantCountMessage(int RoomId, int CurrentUserCount) : IActorMessage;
    internal record StartGameRoomMessage(Player Player) : IActorMessage;
}
