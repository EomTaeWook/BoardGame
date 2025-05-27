using Dignus.Log;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System.Threading;
using System.Threading.Tasks;

namespace BG.GameServer.Network
{
    internal class HeartBeat : ISessionComponent
    {
        private const int MaxPingPongFailures = 5;
        private const int PingpongWarningDelay = 60000;

        private int _currentPingPongFailCount = 0;
        private ISession _session;
        private CancellationTokenSource _cancellationTokenSource;
        private int _pinging = 0;
        private int _currentPingPongIndex = 0;

        public HeartBeat()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public async Task SendPingAsync(ushort protocol)
        {
            if (_session == null)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _pinging, 1, 0) != 0)
            {
                LogHelper.Error($"duplicated pinging! session id: {_session.Id}");
                return;
            }
            var packet = Packet.MakePacket((ushort)PacketCategory.Lobby, protocol, new Ping());
            _session.TrySend(packet);

            await CheckPongDelayAsync(protocol, _currentPingPongIndex);
        }

        private async Task CheckPongDelayAsync(ushort protocol, int sendIndex)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            await Task.Delay(PingpongWarningDelay, _cancellationTokenSource.Token);

            if (_session == null)
            {
                return;
            }

            if (_currentPingPongIndex == sendIndex)
            {
                _currentPingPongFailCount++;
                if (_currentPingPongFailCount >= MaxPingPongFailures)
                {
                    Dispose();
                    return;
                }
            }

            _ = SendPingAsync(protocol);
        }
        public void Pong()
        {
            _currentPingPongIndex++;
        }
        public void Dispose()
        {
            if (_session == null)
            {
                return;
            }
            _session.Dispose();
            _session = null;

            _cancellationTokenSource.Cancel();
            _currentPingPongFailCount = 0;
        }

        public void SetSession(ISession session)
        {
            _session = session;
        }
    }
}
