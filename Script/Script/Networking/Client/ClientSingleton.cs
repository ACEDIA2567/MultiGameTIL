using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;
    public ClientGameManager GameManager { get; private set; }

    public static ClientSingleton Instance
    {
        get
        {
            // 싱글톤이 존재하면
            if (instance != null) { return instance; }

            // 존재하지 않다면
            instance = FindAnyObjectByType<ClientSingleton>();
            if (instance == null)
            {
                Debug.LogError("No ClientSingleton in the Scene!");
                return null;
            }
            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // 클라이언트를 생성
    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();

        // 아래의 함수가 끝날 때까지 대기
        return await GameManager.InitAsync();
    }

    // 해당 오브젝트가 삭제되면
    private void OnDestroy()
    {
        // GameManager 존재 여부 확인 후 실행
        GameManager?.Dispose();
    }
}
