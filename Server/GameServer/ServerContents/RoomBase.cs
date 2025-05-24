using BG.GameServer.Network;
using Dignus.Sockets.Interfaces;
using System.Collections.Concurrent;

namespace BG.GameServer.ServerContents
{
    internal abstract class RoomBase
    {
        public long RoomNumber { get; private set; }
        protected readonly ConcurrentDictionary<string, Player> _accountIdToPlayerMap = new();

        public RoomBase(long roomNumber)
        {
            RoomNumber = roomNumber;
        }
        public bool Join(Player player)
        {
            if (player == null)
            {
                return false;
            }
            var added = _accountIdToPlayerMap.TryAdd(player.AccountId, player);
            if (added)
            {
                player.SetRoom(this);
            }
            return added;
        }
        public void Leave(Player player)
        {
            if (player == null)
            {
                return;
            }
            _accountIdToPlayerMap.Remove(player.AccountId, out _);
        }

        public void Broadcast(IPacket packet)
        {
            foreach (var player in _accountIdToPlayerMap.Values)
            {
                player.Send(packet);
            }
        }
    }
}
