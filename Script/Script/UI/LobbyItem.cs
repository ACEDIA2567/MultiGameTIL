using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text LobbyNameText;
    [SerializeField] private TMP_Text LobbyPlayerText;

    private LobbyList lobbyList;
    private Lobby lobby;

    // 로비 UI의 Text 활성화
    public void Initialise(LobbyList lobbyList, Lobby lobby)
    {
        // 컴포넌트되어 있는 LobbyList의 정보를 받아옴
        this.lobbyList = lobbyList;
        this.lobby = lobby;

        LobbyNameText.text = lobby.Name;
        LobbyPlayerText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void join()
    {
        lobbyList.JoinAsync(lobby);
    }
}
