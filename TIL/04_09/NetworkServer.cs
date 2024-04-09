using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    // 클라이언트 ID
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    // 승인된 유저 정보
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        // 연결 요청을 할때마다 ApprovalCheck메서드를 실행
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        // request.Payload를 UTF8의 문자열 변수로 변환하여 저장
        string playload = System.Text.Encoding.UTF8.GetString(request.Payload);
        // playload를 Json문자열로 변경하여 사용 가능하게 저장함
        UserData userData = JsonUtility.FromJson<UserData>(playload);

        // 클라이언트의 정보 딕셔너리에 각각 저장
        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;
        Debug.Log(userData.userName);

        response.Approved = true;
        // 연결 승인을 재정의 했으므로 플레이어 오브젝트를 생성함
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
            // 딕셔너리 clientIdToAuth에서 clientId정보 삭제
            clientIdToAuth.Remove(clientId);
            // 딕셔너리 authIdToUserData에서 authId정보 삭제
            authIdToUserData.Remove(authId);
        }
    }

    public void Dispose()
    {
        if(networkManager == null) { return; }

        // 추가 했었던 이벤트 함수 제거
        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        networkManager.OnServerStarted -= OnNetworkReady;

        // 네트워크가 아직 수신을 받고 있는 상태라면
        if (networkManager.IsListening)
        {
            // 네트워크 종료
            networkManager.Shutdown();
        }
    }
}
