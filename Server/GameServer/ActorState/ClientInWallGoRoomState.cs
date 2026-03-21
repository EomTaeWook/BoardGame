using Assets.Scripts.GameContents.WallGo;
using BG.GameServer.Messages;
using BG.GameServer.Network;
using BG.GameServer.ServerGameContents;
using Protocol.GSAndClient;
using System.Threading.Tasks;

namespace BG.GameServer.ActorState
{
    internal class ClientInWallGoRoomState(Player player) : IClientState
    {
        public ValueTask HandlePacket(object packet)
        {
            return packet switch
            {
                RemoveWallReqeust reqeust => HandleRemoveWallReqeust(reqeust),
                PlaceWall reqeust => HandlePlaceWall(reqeust),
                MovePieceReqeust reqeust => HandleMovePieceReqeust(reqeust),
                SpawnPieceReqeust reqeust => HandleSpawnPieceReqeust(reqeust),
                PlaceWallReqeust reqeust => HandlePlaceWallReqeust(reqeust),
                _ => ValueTask.CompletedTask
            };
        }
        public ValueTask HandlePlaceWallReqeust(PlaceWallReqeust message)
        {
            player.RoomActorRef.Post(new PlayerMessage<PlaceWallReqeust>(message, player));
            return ValueTask.CompletedTask;
        }
        public ValueTask HandleSpawnPieceReqeust(SpawnPieceReqeust message)
        {
            player.RoomActorRef.Post(new PlayerMessage<SpawnPieceReqeust>(message, player));
            return ValueTask.CompletedTask;
        }
        public ValueTask HandleMovePieceReqeust(MovePieceReqeust message)
        {
            player.RoomActorRef.Post(new PlayerMessage<MovePieceReqeust>(message, player));
            return ValueTask.CompletedTask;
        }
        public ValueTask HandlePlaceWall(MovePieceReqeust message)
        {
            player.RoomActorRef.Post(new PlayerMessage<MovePieceReqeust>(message, player));
            return ValueTask.CompletedTask;
        }
        public ValueTask HandlePlaceWall(PlaceWall message)
        {
            player.RoomActorRef.Post(new PlayerMessage<PlaceWall>(message, player));
            return ValueTask.CompletedTask;
        }
        public ValueTask HandleRemoveWallReqeust(RemoveWallReqeust message)
        {
            player.RoomActorRef.Post(new PlayerMessage<RemoveWallReqeust>(message, player));

            return ValueTask.CompletedTask;
        }
        public void OnEnter()
        {
        }
        public void OnExit()
        {
        }
    }
}
