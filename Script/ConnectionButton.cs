using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionButton : MonoBehaviour
{
    // 호스트 시작 버튼
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    // 서버 참가 버튼
    public void StartClientButton()
    {
        // 네트워크 매니저의 클라이언트 참가
        NetworkManager.Singleton.StartClient();
    }
}
