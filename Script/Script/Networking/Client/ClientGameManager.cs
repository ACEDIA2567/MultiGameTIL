using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private JoinAllocation allocation;

    private NetworkClient networkClient;

    private const string MenuSceneName = "Menu";

    // Task: ��ȯ ���� / ���Ӱ����� �ʱ�ȭ
    public async Task<bool> InitAsync()
    {
        // �÷��̾� ����
        await UnityServices.InitializeAsync();

        // Ŭ���̾�Ʈ�� ��Ʈ��ũ ���� ������ ������ ����
        networkClient = new NetworkClient(NetworkManager.Singleton);

        // ���¸� ��ȯ
        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // relay ���� �����͸� ���� �� ���������� udp�� �Ͽ� ����
        // ���� ���Ȼ� udp => dtls�� �������� ����
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        // ������ relay���� �����͸� ����
        transport.SetRelayServerData(relayServerData);

        UserData userData = new UserData
        {
            // UserData�� userName������ PlauerPrefs�� NameSelector.PlayerNameKey���� ������
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        // ���ڿ� Json������ string������ payLoad�� ����
        string payLoad = JsonUtility.ToJson(userData);
        // ���ڿ� payLoad�� Byte[]�� ��ȯ�Ͽ� ������ ����
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payLoad);

        // Byte[]�� ������ �����͸� ������ ���� ����
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
    }

    // �޴� �̵�
    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public void Dispose()
    {
        // networkClient�� ���� ���� Ȯ�� �� ����
        networkClient?.Dispose();
    }
}
