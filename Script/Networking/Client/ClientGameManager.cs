using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private JoinAllocation allocation;

    private const string MenuSceneName = "Menu";

    // Task: 반환 유형 / 게임관리자 초기화
    public async Task<bool> InitAsync()
    {
        // 플레이어 인증
        await UnityServices.InitializeAsync();

        // 상태를 반환
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

        // relay 서버 데이터를 생성 후 프로토콜을 udp로 하여 생성
        // 안전 보안상 udp => dtls로 프로토콜 변경
        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        // 생성한 relay서버 데이터를 설정
        transport.SetRelayServerData(relayServerData);

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

        NetworkManager.Singleton.StartClient();
    }

    // 메뉴 이동
    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }
}
