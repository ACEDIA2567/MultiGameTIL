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
        // ������Ʈ�� �ı����� �ʰ� ��
        DontDestroyOnLoad(gameObject);

        // ���� ������ �׷��� ����� �ʿ����� �ʱ� ������
        // true��� ���뼭�� / false��� ���뼭���� �ƴ�
        await LaunchInmode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    // ���� �������� ���� Ȯ���� ���� �Լ�
    private async Task LaunchInmode(bool isDedicatedServer)
    {
        // ���� �������
        if (isDedicatedServer)
        {

        }
        // ���뼭���� �ƴ϶��
        else
        {
            // ȣ��Ʈ �̱��� ����
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();

            // Ŭ���̾�Ʈ �̱��� ����
            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();            

            if (authenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }

}
