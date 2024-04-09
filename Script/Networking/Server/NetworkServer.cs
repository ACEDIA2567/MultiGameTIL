using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    // Ŭ���̾�Ʈ ID
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    // ���ε� ���� ����
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        // ���� ��û�� �Ҷ����� ApprovalCheck�޼��带 ����
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        // request.Payload�� UTF8�� ���ڿ� ������ ��ȯ�Ͽ� ����
        string playload = System.Text.Encoding.UTF8.GetString(request.Payload);
        // playload�� Json���ڿ��� �����Ͽ� ��� �����ϰ� ������
        UserData userData = JsonUtility.FromJson<UserData>(playload);

        // Ŭ���̾�Ʈ�� ���� ��ųʸ��� ���� ����
        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;
        Debug.Log(userData.userName);

        response.Approved = true;
        // ���� ������ ������ �����Ƿ� �÷��̾� ������Ʈ�� ������
        response.CreatePlayerObject = true;
    }

    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            // ��ųʸ� clientIdToAuth���� clientId���� ����
            clientIdToAuth.Remove(clientId);
            // ��ųʸ� authIdToUserData���� authId���� ����
            authIdToUserData.Remove(authId);
        }
    }

    public void Dispose()
    {
        if(networkManager == null) { return; }

        // �߰� �߾��� �̺�Ʈ �Լ� ����
        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        networkManager.OnServerStarted -= OnNetworkReady;

        // ��Ʈ��ũ�� ���� ������ �ް� �ִ� ���¶��
        if (networkManager.IsListening)
        {
            // ��Ʈ��ũ ����
            networkManager.Shutdown();
        }
    }
}
