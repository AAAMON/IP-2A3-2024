using System;

namespace roomconnectivity
{
    public interface IRoom
    {
        bool IsRoomFull();
        int GetPlayersNumber();
        int GetRoomId();
        string GetRoomName();
        bool AddUser(User user);
        string[] GetPlayerNames();
        bool KickUser(User user);
    }
}
