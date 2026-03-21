using BG.GameServer.Messages;
using Dignus.Actor.Network;
using Dignus.Log;
using Protocol.GSAndClient;
using System.Threading;
using System.Threading.Tasks;

namespace BG.GameServer.Network
{
    internal class HeartBeat
    {
        private const int MaxPingPongFailures = 5;
        private const int PingpongWarningDelay = 60000;

        private int _currentPingPongFailCount = 0;
        private INetworkSessionRef _sessionRef;
        private CancellationTokenSource _cancellationTokenSource;
        private int _pinging = 0;
        private int _currentPingPongIndex = 0;

        public HeartBeat()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public async Task SendPingAsync(ushort protocol)
        {
            if (_sessionRef == null)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _pinging, 1, 0) != 0)
            {
                LogHelper.Error($"duplicated pinging! session id: {_sessionRef.GetHashCode()}");
                return;
            }
            var packet = Packet.MakePacket((ushort)PacketCategory.Lobby, protocol, new Ping());
            _sessionRef.SendAsync(packet);

            await CheckPongDelayAsync(protocol, _currentPingPongIndex);
        }

        private async Task CheckPongDelayAsync(ushort protocol, int sendIndex)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            await Task.Delay(PingpongWarningDelay, _cancellationTokenSource.Token);

            if (_sessionRef == null)
            {
                return;
            }

            if (_currentPingPongIndex == sendIndex)
            {
                _currentPingPongFailCount++;
                if (_currentPingPongFailCount >= MaxPingPongFailures)
                {
                    _sessionRef.Post(new KickUserMessage(ErrorCode.PingPongTimeout));
                    Dispose();
                    return;
                }
            }

            Interlocked.Exchange(ref _pinging, 0);

            _ = SendPingAsync(protocol);
        }
        public void Pong()
        {
            _currentPingPongIndex++;
        }
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _currentPingPongFailCount = 0;
        }

        public void SetSession(INetworkSessionRef session)
        {
            _sessionRef = session;
            _ = SendPingAsync((ushort)GSCProtocol.Ping);
        }
    }
}
