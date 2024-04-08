using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private Allocation allocation;
    private string joinCode; // ���� �ڵ�
    private string lobbyId;  // �κ� ���̵�
    private NetworkServer networkServer; // ��Ʈ��ũ ����

    private const int MaxConnections = 20;
    private const string GameSceneName = "Game";

    public async Task StartHostAsync()
    {
        try
        {
            // �ִ� ���� ���� 20���� �Ͽ� ����
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch(Exception ex)
        {
            // ������ ���� �α� ��� �� ��ȯ
            Debug.Log(ex);
            return;
        }

        try
        {
            // ������ �� �ִ� �ڵ� ����
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch (Exception ex)
        {
            // ������ ���� �α� ��� �� ��ȯ
            Debug.Log(ex);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // relay ���� �����͸� ���� �� ���������� udp�� �Ͽ� ����
        // ���� ���Ȼ� udp => dtls�� �������� ����
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        // ������ relay���� �����͸� ����
        transport.SetRelayServerData(relayServerData);

        // �κ� ���� ����
        try
        {
            // �κ� �ɼ� ������ ������
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            // �κ� ������� ǰ
            lobbyOptions.IsPrivate = false;
            // �κ� ������ ����
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member, // ������� ������
                        value: joinCode // �ڵ带
                    )
                }
            };
            string PlayerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "unknown Lobby");
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(
                $"{PlayerName}'s Lobby", MaxConnections, lobbyOptions);

            lobbyId = lobby.Id;

            // �κ� 15�ʸ��� Ȱ��ȭ�ϰ� �ڷ�ƾ ����
            HostSingleton.Instance.StartCoroutine(HearbeatLobby(15));
        }
        catch(LobbyServiceException ex) // �κ񼭺��� ������ �������
        {
            Debug.Log(ex);
            return;
        }

        // ��Ʈ��ũ ���� ����
        networkServer = new NetworkServer(NetworkManager.Singleton);

        UserData userData = new UserData
        {
            // UserData�� userName������ PlauerPrefs�� NameSelector.PlayerNameKey���� ������
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name")
        };

        // ���ڿ� Json������ string������ payLoad�� ����
        string payLoad = JsonUtility.ToJson(userData);
        // ���ڿ� payLoad�� Byte[]�� ��ȯ�Ͽ� ������ ����
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payLoad);

        // Byte[]�� ������ �����͸� ������ ���� ����
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        // ȣ��Ʈ ����
        NetworkManager.Singleton.StartHost();

        // �� �̵�
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    private IEnumerator HearbeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            // �ش��ϴ� �κ�ID�� ��ȣ�� ����
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
}
