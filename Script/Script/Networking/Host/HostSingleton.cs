using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    public HostGameManager GameManager { get; private set; }

    public static HostSingleton Instance
    {
        get
        {
            // �̱����� �����ϸ�
            if (instance != null) { return instance; }

            // �������� �ʴٸ�
            // instance�� HostSingleton�� ��ũ��Ʈ�� �˻��Ѵ�.
            instance = FindAnyObjectByType<HostSingleton>();
            // ��ũ��Ʈ�� ���ٸ� ���� ȣ��
            if (instance == null)
            {
                Debug.LogError("!");
                return null;
            }
            return instance;
        }
    }

    private void Start()
    {
        // ���� �ȵǰ� ��
        DontDestroyOnLoad(gameObject);
    }

    // ȣ��Ʈ�� ����
    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
