using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemParent; // �κ��� �θ�
    [SerializeField] private LobbyItem lobbyItemPrefab; // �κ� Prefab

    private bool isJoining = false; // ���� ����
    private bool isRefreshing = false; // ���ΰ�ħ ����

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (isRefreshing) { return; }
        isRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25; // ������ ���� 25�� ����

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT, // GT:ũ��
                    value: "0"), // ���� 0
                    // AvailableSlots�� ���� 0���� ū �κ� ��û��
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked, // ���
                    op: QueryFilter.OpOptions.EQ, // GT:ũ��
                    value: "0")
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            // ���� �����ϴ� �κ���� ����
            foreach(Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            // ���ͷ� �����ϴ� �κ���� ����
            foreach (Lobby lobby in lobbies.Results)
            {
                // �κ���� �����Ͽ� ������ ����
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                // ������ ����(�κ�)���� Text ����
                lobbyItem.Initialise(this, lobby);
            }
        }
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex);
        }

        // ���ΰ�ħ�� �Ϸ������Ƿ� false ����
        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        // ���� ���̶�� ��ȯ
        if (isJoining) { return; }
        // ������ �߱� ������ ���� ���� true ����
        isJoining = true;

        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex);
            return;
        }

        // Ŭ���̾�Ʈ�� �̱������� joinCode�� ������ ���±� ������ ���� ���� false�� ����
        isJoining = false;
    }
}
