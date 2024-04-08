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
    private string joinCode; // 참여 코드
    private string lobbyId;  // 로비 아이디
    private NetworkServer networkServer; // 네트워크 서버

    private const int MaxConnections = 20;
    private const string GameSceneName = "Game";

    public async Task StartHostAsync()
    {
        try
        {
            // 최대 연결 수는 20으로 하여 생성
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch(Exception ex)
        {
            // 문제에 대한 로그 출력 후 반환
            Debug.Log(ex);
            return;
        }

        try
        {
            // 참가할 수 있는 코드 생성
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch (Exception ex)
        {
            // 문제에 대한 로그 출력 후 반환
            Debug.Log(ex);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // relay 서버 데이터를 생성 후 프로토콜을 udp로 하여 생성
        // 안전 보안상 udp => dtls로 프로토콜 변경
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        // 생성한 relay서버 데이터를 설정
        transport.SetRelayServerData(relayServerData);

        // 로비 생성 로직
        try
        {
            // 로비 옵션 정보를 생성함
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            // 로비를 비공개를 품
            lobbyOptions.IsPrivate = false;
            // 로비 데이터 설정
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member, // 멤버에게 보여줌
                        value: joinCode // 코드를
                    )
                }
            };
            string PlayerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "unknown Lobby");
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(
                $"{PlayerName}'s Lobby", MaxConnections, lobbyOptions);

            lobbyId = lobby.Id;

            // 로비가 15초마다 활성화하게 코루틴 실행
            HostSingleton.Instance.StartCoroutine(HearbeatLobby(15));
        }
        catch(LobbyServiceException ex) // 로비서비스의 오류를 출력해줌
        {
            Debug.Log(ex);
            return;
        }

        // 네트워크 서버 생성
        networkServer = new NetworkServer(NetworkManager.Singleton);

        UserData userData = new UserData
        {
            // UserData의 userName변수를 PlauerPrefs의 NameSelector.PlayerNameKey에서 가져옴
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name")
        };

        // 문자열 Json파일을 string변수로 payLoad에 저장
        string payLoad = JsonUtility.ToJson(userData);
        // 문자열 payLoad를 Byte[]로 변환하여 변수로 저장
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payLoad);

        // Byte[]로 저장한 데이터를 서버로 정보 전송
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        // 호스트 시작
        NetworkManager.Singleton.StartHost();

        // 씬 이동
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    private IEnumerator HearbeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            // 해당하는 로비ID의 신호를 전달
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
}
