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
        private readonly MpscBoundedQueue<IPacket> _broadcastQueue = new(10000);

        private int _processing = 0;

        public bool EnqueueBroadcast(IPacket packet)
        {
            if (_broadcastQueue.TryEnqueue(packet) == false)
            {
                LogHelper.Fatal("broadcast queue is full. dropping message.");
                return false;
            }
            
            if (Interlocked.CompareExchange(ref _processing, 1, 0) == 1)
            {
                return true;
            }

            _ = Task.Run(BroadcastToAll);

            return true;
        }

        private void BroadcastToAll()
        {
            while (true)
            {
                try
                {
                    while (_broadcastQueue.TryDequeue(out var packet))
                    {
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

                if (_broadcastQueue.IsEmpty)
                {
                    break;
                }

                if (Interlocked.CompareExchange(ref _processing, 1, 0) == 1)
                {
                    break;
                }                
            }
        }
    }
}
