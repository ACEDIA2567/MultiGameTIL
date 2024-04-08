using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer
{
    private NetworkManager networkManager;

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        // ���� ��û�� �Ҷ����� ApprovalCheck�޼��带 ����
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        // request.Payload�� UTF8�� ���ڿ� ������ ��ȯ�Ͽ� ����
        string playload = System.Text.Encoding.UTF8.GetString(request.Payload);
        // playload�� Json���ڿ��� �����Ͽ� ��� �����ϰ� ������
        UserData userData = JsonUtility.FromJson<UserData>(playload);

        Debug.Log(userData.userName);

        response.Approved = true;
        // ���� ������ ������ �����Ƿ� �÷��̾� ������Ʈ�� ������
        response.CreatePlayerObject = true;
    }
}
