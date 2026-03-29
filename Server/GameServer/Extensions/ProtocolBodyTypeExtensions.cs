using Assets.Scripts.GameContents.WallGo;
using BG.GameServer.Internals;
using Protocol.GSAndClient;

namespace BG.GameServer.Extensions
{
    internal static class ProtocolBodyTypeExtensions
    {
        public static void RegisterServerProtocols(this SystemProtocolMapper protocolBodyTypeMapper)
        {
            protocolBodyTypeMapper.AddMapping<Login>(CGSProtocol.Login);
            protocolBodyTypeMapper.AddMapping<LeaveRoom>(CGSProtocol.LeaveRoom);
            protocolBodyTypeMapper.AddMapping<JoinRoom>(CGSProtocol.JoinRoom);
            protocolBodyTypeMapper.AddMapping<CreateRoom>(CGSProtocol.CreateRoom);
            protocolBodyTypeMapper.AddMapping<StartGameRoom>(CGSProtocol.StartGameRoom);
            protocolBodyTypeMapper.AddMapping<GetRoomList>(CGSProtocol.GetRoomList);
            protocolBodyTypeMapper.AddMapping<Pong>(CGSProtocol.Pong);
        }
        public static void RegisterServerProtocols(this WallGoProtocolMapper protocolBodyTypeMapper)
        {
            protocolBodyTypeMapper.AddMapping<PlaceWallReqeust>(WallGoCommandProtocol.PlaceWall);
            protocolBodyTypeMapper.AddMapping<MovePieceReqeust>(WallGoCommandProtocol.MovePiece);
            protocolBodyTypeMapper.AddMapping<SpawnPieceReqeust>(WallGoCommandProtocol.SpawnPiece);
            protocolBodyTypeMapper.AddMapping<RemoveWallReqeust>(WallGoCommandProtocol.RemoveWall);
        }
    }
}
