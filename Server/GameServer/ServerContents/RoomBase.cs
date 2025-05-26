using BG.GameServer.Network;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System.Collections.Concurrent;

namespace BG.GameServer.ServerContents
{
    internal abstract class RoomBase
    {
        public GameType GameType { get; private set; }
        public int MinUserCount { get; private set; }
        public int MaxUserCount { get; private set; }

        public int RoomNumber { get; private set; }
        public Player Host { get => _hostPlayer; }
        protected readonly ConcurrentDictionary<string, Player> _accountIdToPlayerMap = new();
        private Player _hostPlayer;

        public abstract bool StartGame();
        public RoomBase(int roomNumber, GameType gameType, int minUserCount, int maxUserCount)
        {
            MinUserCount = minUserCount;
            RoomNumber = roomNumber;
            MaxUserCount = maxUserCount;
            GameType = gameType;
        }
        public ICollection<Player> GetMembers()
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
            return _accountIdToPlayerMap.Count >= MaxUserCount;
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
