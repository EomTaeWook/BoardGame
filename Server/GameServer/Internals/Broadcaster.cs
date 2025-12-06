using Dignus.Collections;
using Dignus.DependencyInjection.Attributes;
using Dignus.Log;
using Dignus.Sockets.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BG.GameServer.Internals
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Singleton)]
    internal class Broadcaster(SessionManager sessionManager)
    {
        private readonly SynchronizedArrayQueue<IPacket> _broadcastQueue = [];

        private int _processing = 0;

        public bool EnqueueBroadcast(IPacket packet)
        {
            if (_broadcastQueue.Count > 50000)
            {
                LogHelper.Fatal("broadcast queue is full. dropping message.");
                return false;
            }

            _broadcastQueue.Add(packet);

            if (Interlocked.CompareExchange(ref _processing, 1, 0) == 1)
            {
                return true;
            }

            _ = Task.Run(BroadcastToAll);

            return true;
        }

        public void SetSession(ISession session)
        {
            throw new NotImplementedException();
        }

        private void BroadcastToAll()
        {
            try
            {
                while (_broadcastQueue.Count > 0)
                {
                    if (_broadcastQueue.TryRead(out var packet) == false)
                    {
                        break;
                    }

                    var players = sessionManager.AllPlayers.ToArray();

                    Parallel.For(0, players.Length, index =>
                    {
                        var player = players[index];

                        player.Send(packet);
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            finally
            {
                Interlocked.Exchange(ref _processing, 0);
            }
        }
    }
}
