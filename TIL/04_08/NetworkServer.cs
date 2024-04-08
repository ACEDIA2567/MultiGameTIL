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

        // 연결 요청을 할때마다 ApprovalCheck메서드를 실행
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        // request.Payload를 UTF8의 문자열 변수로 변환하여 저장
        string playload = System.Text.Encoding.UTF8.GetString(request.Payload);
        // playload를 Json문자열로 변경하여 사용 가능하게 저장함
        UserData userData = JsonUtility.FromJson<UserData>(playload);

        Debug.Log(userData.userName);

        response.Approved = true;
        // 연결 승인을 재정의 했으므로 플레이어 오브젝트를 생성함
        response.CreatePlayerObject = true;
    }
}

