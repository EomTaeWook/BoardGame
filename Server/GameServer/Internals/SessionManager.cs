using BG.GameServer.ServerGameContents;
using Dignus.DependencyInjection.Attributes;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BG.GameServer.Internals
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    internal class SessionManager
    {
        private readonly ConcurrentDictionary<string, Player> _players = new();

        public Player GetPlayer(string accountId)
        {
            _players.TryGetValue(accountId, out Player player);

            return player;
        }
        public bool TryAddPlayer(Player player)
        {
            return _players.TryAdd(player.AccountId, player);
        }
        public bool TryRemovePlayer(Player player)
        {
            return _players.Remove(player.AccountId, out Player _);
        }
        public IEnumerable<Player> AllPlayers { get => _players.Values; }
    }
}
