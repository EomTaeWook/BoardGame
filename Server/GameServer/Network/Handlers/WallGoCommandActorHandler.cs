using Assets.Scripts.GameContents.WallGo;
using BG.GameServer.Actors;
using Dignus.Sockets.Interfaces;
using Protocol.GSAndClient;
using System.Threading.Tasks;

namespace BG.GameServer.Network.Handlers
{
    internal class WallGoCommandActorHandler : IProtocolHandler<object>
    {
        public T DeserializeBody<T>(object body)
        {
            return (T)body;
        }
        public static async Task RemoveWall(ClientActor clientActor, RemoveWallReqeust request)
        {
            await clientActor.ProcessPacket(request);
        }
        public static async Task PlaceWall(ClientActor clientActor, PlaceWall request)
        {
            await clientActor.ProcessPacket(request);
        }
        public static async Task MovePiece(ClientActor clientActor, MovePieceReqeust request)
        {
            await clientActor.ProcessPacket(request);
        }
        public static async Task SpawnPiece(ClientActor clientActor, SpawnPieceReqeust request)
        {
            await clientActor.ProcessPacket(request);
        }
        public static async Task PlaceWall(ClientActor clientActor, PlaceWallReqeust request)
        {
            await clientActor.ProcessPacket(request);
        }
    }
}
