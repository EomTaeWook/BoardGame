using BG.GameServer.Network;
using Dignus.Sockets.Interfaces;
using System.Collections.Concurrent;

namespace BG.GameServer.ServerContents
{
    internal abstract class RoomBase
    {
        public long RoomNumber { get; private set; }
        public Player Host { get => _hostPlayer; }
        protected readonly ConcurrentDictionary<string, Player> _accountIdToPlayerMap = new();
        private readonly int _maxUserCount;
        private Player _hostPlayer;
        public RoomBase(long roomNumber, int maxUserCount)
        {
            RoomNumber = roomNumber;
            _maxUserCount = maxUserCount;
        }
        public ICollection<Player> Members()
        {
            return _accountIdToPlayerMap.Values;
        }
        public string GetHostAccountId()
        {
            if (_hostPlayer == null)
            {
                return string.Empty;
            }
            return _hostPlayer.AccountId;
        }
        public bool IsEmpty()
        {
            return _accountIdToPlayerMap.IsEmpty;
        }

        public bool IsFull()
        {
            return _accountIdToPlayerMap.Count >= _maxUserCount;
        }
        public bool Join(Player player)
        {
            if (player == null)
            {
                return false;
            }

            if (IsFull())
            {
                return false;
            }

            var added = _accountIdToPlayerMap.TryAdd(player.AccountId, player);
            if (added)
            {
                if (_accountIdToPlayerMap.Count == 1)
                {
                    _hostPlayer = player;
                }

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

            if (_hostPlayer.AccountId == player.AccountId)
            {
                if (IsEmpty() == false)
                {
                    _hostPlayer = _accountIdToPlayerMap.ElementAt(0).Value;
                }
            }
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
