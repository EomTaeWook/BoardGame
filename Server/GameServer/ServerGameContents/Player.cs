using Assets.Scripts.GameContents;
using Dignus.Actor.Core;
using Dignus.Actor.Network;
using Dignus.Sockets.Interfaces;

namespace BG.GameServer.ServerGameContents
{
    internal class Player(string accountId,
        string nickname,
        INetworkSessionRef sessionRef) : IPlayer
    {
        public string AccountId { get; private set; } = accountId;

        public string Nickname { get; private set; } = nickname;

        public INetworkSessionRef SessionRef { get; init; } = sessionRef;

        public IActorRef RoomActorRef { get; private set; }

        public void SetRoom(IActorRef roomActorRef)
        {
            RoomActorRef = roomActorRef;
        }
        public void Send(IPacket packet)
        {
            SessionRef.SendAsync(packet);
        }
    }
}
