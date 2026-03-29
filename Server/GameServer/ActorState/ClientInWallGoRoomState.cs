using Assets.Scripts.GameContents.WallGo;
using BG.GameServer.Messages;
using BG.GameServer.ServerGameContents;
using Dignus.Actor.Abstractions;
using Dignus.Log;
using Protocol.GSAndClient;
using System.Threading.Tasks;

namespace BG.GameServer.ActorState
{
    internal class ClientInWallGoRoomState(Player player) : IClientState
    {
        public ValueTask HandlePacket(IActorMessage message)
        {
            return message switch
            {
                GetRoomList => ValueTask.CompletedTask,
                RemoveWallReqeust reqeust => HandleRemoveWallReqeust(reqeust),
                PlaceWall reqeust => HandlePlaceWall(reqeust),
                MovePieceReqeust reqeust => HandleMovePieceReqeust(reqeust),
                SpawnPieceReqeust reqeust => HandleSpawnPieceReqeust(reqeust),
                PlaceWallReqeust reqeust => HandlePlaceWallReqeust(reqeust),
                _ => UnhandleMessage(message)
            };
        }
        private static ValueTask UnhandleMessage(IActorMessage message)
        {
            LogHelper.Error($"[ClientInWallGoRoomState]: {message.GetType()}");
            return ValueTask.CompletedTask;
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
