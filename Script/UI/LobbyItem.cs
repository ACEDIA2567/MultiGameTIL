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

    // �κ� UI�� Text Ȱ��ȭ
    public void Initialise(LobbyList lobbyList, Lobby lobby)
    {
        // ������Ʈ�Ǿ� �ִ� LobbyList�� ������ �޾ƿ�
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
