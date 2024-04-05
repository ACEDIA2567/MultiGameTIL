```cs
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemParent; // 로비의 부모
    [SerializeField] private LobbyItem lobbyItemPrefab; // 로비 Prefab

    private bool isJoining = false; // 참가 여부
    private bool isRefreshing = false; // 새로고침 여부

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
            options.Count = 25; // 필터의 상위 25개 설정

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT, // GT:크다
                    value: "0"), // 값이 0
                    // AvailableSlots의 값이 0보다 큰 로비를 요청함
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked, // 잠김
                    op: QueryFilter.OpOptions.EQ, // GT:크다
                    value: "0")
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            // 현재 존재하는 로비들을 삭제
            foreach(Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            // 필터로 존재하는 로비들을 생성
            foreach (Lobby lobby in lobbies.Results)
            {
                // 로비들을 생성하여 변수로 저장
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                // 저장한 변수(로비)들의 Text 갱신
                lobbyItem.Initialise(this, lobby);
            }
        }
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex);
        }

        // 새로고침을 완료했으므로 false 변경
        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        // 접속 중이라면 반환
        if (isJoining) { return; }
        // 접속을 했기 때문에 참가 여부 true 변경
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

        // 클라이언트의 싱글톤으로 joinCode로 참여를 끝냈기 때문에 참가 여부 false로 변경
        isJoining = false;
    }
}

```
