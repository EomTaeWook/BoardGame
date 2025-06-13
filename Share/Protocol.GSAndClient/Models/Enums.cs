// Copyright (c) 2021 EomTaeWook
// MIT License — https://opensource.org/licenses/MIT
// Part of Dignus Library

namespace Protocol.GSAndClient.Models
{
    public enum LoginReason
    {
        Success,
        AlreadyLogin,

        Max,
    }

    public enum JoinRoomReason
    {
        Success,
        IsFull,
        NotFound,

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
