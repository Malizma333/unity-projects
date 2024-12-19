using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    public static Lobby inst;

    private NetworkManager manager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    public 

    void Start()
    {
        manager = GetComponent<NetworkManager>();

        if (inst == null) inst = this;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);


        Debug.Log("Started");

    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        Debug.Log("Lobby Created!");

        if (callback.m_eResult != EResult.k_EResultOK) return;

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress",
            SteamUser.GetSteamID().ToString());


        UI_Utils.MainMenuInstance.SetActive(false);
        UI_Utils.LobbyMenuInstance.SetActive(true);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress");

        manager.networkAddress = hostAddress;
        manager.StartClient();

        UI_Utils.MainMenuInstance.SetActive(false);
        UI_Utils.LobbyMenuInstance.SetActive(true);


    }

}
