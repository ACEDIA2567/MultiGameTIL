using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionButton : MonoBehaviour
{
    // ȣ��Ʈ ���� ��ư
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    // ���� ���� ��ư
    public void StartClientButton()
    {
        // ��Ʈ��ũ �Ŵ����� Ŭ���̾�Ʈ ����
        NetworkManager.Singleton.StartClient();
    }
}
