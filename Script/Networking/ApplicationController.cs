using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;

    private async void Start()
    {
        // 오브젝트가 파괴되지 않게 함
        DontDestroyOnLoad(gameObject);

        // 전용 서버는 그래픽 출력이 필요하지 않기 때문에
        // true라면 전용서버 / false라면 전용서버가 아님
        await LaunchInmode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    // 전용 서버인지 여부 확인을 위한 함수
    private async Task LaunchInmode(bool isDedicatedServer)
    {
        // 전용 서버라면
        if (isDedicatedServer)
        {

        }
        // 전용서버가 아니라면
        else
        {
            // 호스트 싱글톤 생성
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();

            // 클라이언트 싱글톤 생성
            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();            

            if (authenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }

}
