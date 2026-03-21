// Copyright (c) 2021 EomTaeWook
// MIT License — https://opensource.org/licenses/MIT
// Part of Dignus Library

namespace Protocol.GSAndClient.Models
{
    public enum LoginReason
    {
        Success,
        DuplicateLogin,

        Max,
    }

    public enum JoinRoomReason
    {
        Success,
        IsFull,
        NotFound,
        GameAlreadyStarted,

        Max,
    }

    public enum StartGameRoomReason
    {
        Success,
        NotEnoughUser,

        Max,
    }

    public enum RoomMode
    {
        Public,
        Private,

        Max
    }


}
