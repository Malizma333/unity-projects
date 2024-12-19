using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public NetManager manager;

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
    }


    public void StartGame()
    {
        manager.inGame = true;
        manager.ServerChangeScene("SampleScene");
    }
}
