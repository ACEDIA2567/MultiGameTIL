using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager networkManager;
    private const string MenuSceneName = "Menu";

    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        // 연결이 끓어졌을 때 OnClientDisconnect메서드 실행
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }                                             

    private void OnClientDisconnect(ulong clientId)
    {
        // 현재 Id와 네트워크에 존재하는 Id가 다르거나 Id가 0이 아닐 경우
        if(clientId != 0 && clientId != networkManager.LocalClientId) { return; }

        // 현재 씬이 메뉴씬이 아니라면
        if(SceneManager.GetActiveScene().name != MenuSceneName)
        {
            // 메뉴 씬으로 이동
            SceneManager.LoadScene(MenuSceneName);
        }

        // 클라이언트가 네트워크와 연결이 되어 있다면
        if (networkManager.IsConnectedClient)
        {
            // 네트워크를 끓음
            networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        // 네트워크 관리자가 존재 한다면
        if (networkManager != null)
        {
            // OnClientDisconnect를 이벤트에서 제거
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}
