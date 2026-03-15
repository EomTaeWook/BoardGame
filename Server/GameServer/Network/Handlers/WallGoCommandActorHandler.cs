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
        public Task RemoveWall(ClientActor clientActor, RemoveWallReqeust reqeust)
        {
            return Task.CompletedTask;
        }
        public Task PlaceWall(ClientActor clientActor, PlaceWall placeWall)
        {
            return Task.CompletedTask;
        }
        public Task MovePiece(ClientActor clientActor, MovePieceReqeust request)
        {
            return Task.CompletedTask;
        }
        public Task SpawnPiece(ClientActor clientActor, SpawnPieceReqeust request)
        {
            return Task.CompletedTask;
        }
        public Task PlaceWall(ClientActor clientActor, PlaceWallReqeust request)
        {
            return Task.CompletedTask;
        }
    }
}
