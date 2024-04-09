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

        // ������ �������� �� OnClientDisconnect�޼��� ����
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }                                             

    private void OnClientDisconnect(ulong clientId)
    {
        // ���� Id�� ��Ʈ��ũ�� �����ϴ� Id�� �ٸ��ų� Id�� 0�� �ƴ� ���
        if(clientId != 0 && clientId != networkManager.LocalClientId) { return; }

        // ���� ���� �޴����� �ƴ϶��
        if(SceneManager.GetActiveScene().name != MenuSceneName)
        {
            // �޴� ������ �̵�
            SceneManager.LoadScene(MenuSceneName);
        }

        // Ŭ���̾�Ʈ�� ��Ʈ��ũ�� ������ �Ǿ� �ִٸ�
        if (networkManager.IsConnectedClient)
        {
            // ��Ʈ��ũ�� ����
            networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        // ��Ʈ��ũ �����ڰ� ���� �Ѵٸ�
        if (networkManager != null)
        {
            // OnClientDisconnect�� �̺�Ʈ���� ����
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}
