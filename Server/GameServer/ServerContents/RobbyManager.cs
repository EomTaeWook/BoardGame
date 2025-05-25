using BG.GameServer.Network;
using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System.Collections.Concurrent;

namespace BG.GameServer.ServerContents
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    internal class RobbyManager(IServiceProvider serviceProvider) : ISessionComponent
    {
        private readonly ConcurrentDictionary<string, Player> _players = new();
        private readonly ConcurrentDictionary<long, RoomBase> _rooms = new();
        public bool TryAddPlayer(Player player)
        {
            return _players.TryAdd(player.AccountId, player);
        }
        public bool TryRemovePlayer(Player player)
        {
            return _players.Remove(player.AccountId, out Player _);
        }

        public bool TryCreateGameRoom(GameType gameType, out RoomBase room)
        {
            room = null;
            if (gameType == GameType.Max)
            {
                return false;
            }
            var roomNumber = -1;
            for (int i = 0; i < 10; ++i)
            {
                roomNumber = Random.Shared.Next(1000, 99999);
                if (_rooms.TryAdd(roomNumber, null) == true)
                {
                    break;
                }
            }

            if (_rooms.TryGetValue(roomNumber, out RoomBase roomBase) == false)
            {
                return false;
            }

            if (roomBase != null)
            {
                return false;
            }

            if (gameType == GameType.WallGo)
            {
                room = new WallGoRoom(roomNumber, serviceProvider);
            }

            _rooms[roomNumber] = room;

            return true;
        }

        public bool TryGetGameRoom(long roomNumber, out RoomBase room)
        {
            return _rooms.TryGetValue(roomNumber, out room);
        }

        public bool RemoveRoom(long roomNumber)
        {
            return _rooms.TryRemove(roomNumber, out var _);
        }

        void ISessionComponent.SetSession(ISession session)
        {
        }

        void ISessionComponent.Dispose()
        {
        }
    }
}
