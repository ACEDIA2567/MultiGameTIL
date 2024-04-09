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
            // �̱����� �����ϸ�
            if (instance != null) { return instance; }

            // �������� �ʴٸ�
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

    // Ŭ���̾�Ʈ�� ����
    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();

        // �Ʒ��� �Լ��� ���� ������ ���
        return await GameManager.InitAsync();
    }

    // �ش� ������Ʈ�� �����Ǹ�
    private void OnDestroy()
    {
        // GameManager ���� ���� Ȯ�� �� ����
        GameManager?.Dispose();
    }
}
