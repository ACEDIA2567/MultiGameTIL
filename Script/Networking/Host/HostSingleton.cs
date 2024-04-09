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
            // 싱글톤이 존재하면
            if (instance != null) { return instance; }

            // 존재하지 않다면
            // instance에 HostSingleton의 스크립트를 검색한다.
            instance = FindAnyObjectByType<HostSingleton>();
            // 스크립트다 없다면 오류 호출
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
        // 삭제 안되게 함
        DontDestroyOnLoad(gameObject);
    }

    // 호스트를 생성
    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
