# 2024-04-09 TIL

```cs
[InitializeOnLoad]
public static class StartUpSceneLoader
{
    static StartUpSceneLoader()
    {
        // 게임 시작 시 바로 실행
        EditorApplication.playModeStateChanged += LoadStartupScene;
    }

    private static void LoadStartupScene(PlayModeStateChange state)
    {
        // 저장을 하지 않고 플레이 버튼을 눌렀을 때
        if(state == PlayModeStateChange.ExitingEditMode)
        {
            // 현재 씬의 편집 내용을 저장 여부 확인 후 실행
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        // 플레이 버튼을 눌렀을 때
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // EditorSceneManager의 현재 씬이 0번째가 아니라면
            if (EditorSceneManager.GetActiveScene().buildIndex != 0)
            {
                // 0번째 씬으로 이동
                EditorSceneManager.LoadScene(0);
            }
        }
    }
}
```
### InitializeOnLoad로 플레이 버튼을 누르면 바로 실행이 되게 설정
### 현재 씬의 편집 상태에 따라서 씬 이동 및 저장 구현

```cs
[Serializable]
public class UserData
{
    public string userName;   // 이름
    public string userAuthId; // 아이디
}
```
### UserData에 userAuthId를 추가

```cs
// 클라이언트 ID
private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
// 승인된 유저 정보
private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();
```
### 클라이언트와 유저 정보를 딕셔너리 변수로 저장


```cs
networkManager.OnServerStarted += OnNetworkReady;

clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
authIdToUserData[userData.userAuthId] = userData;
```
### 추가 구현으로 서버 연결 시 OnNetworkReady를 이벤트 함수에 추가 후
### 각 딕셔너리 변수에 연결 한 클라리언트의 정보로 저장

```cs
private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            // 딕셔너리 clientIdToAuth에서 clientId정보 삭제
            clientIdToAuth.Remove(clientId);
            // 딕셔너리 authIdToUserData에서 authId정보 삭제
            authIdToUserData.Remove(authId);
        }
    }
```
### OnClientDisconnectCallback를 사용하여 연결 해제 시 실행되는 이벤트 함수에 OnClientDisconnect를 추가
### 아까 저장하였던 클라이언트의 정보를 검색하여 삭제 함

```cs
public void Dispose()
    {
        if(networkManager == null) { return; }

        // 추가 했었던 이벤트 함수 제거
        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        networkManager.OnServerStarted -= OnNetworkReady;

        // 네트워크가 아직 수신을 받고 있는 상태라면
        if (networkManager.IsListening)
        {
            // 네트워크 종료
            networkManager.Shutdown();
        }
    }
```
### 인터페이스 IDisposable를 이용하여 추가했던 이벤트 함수를 제거 처리 후
### 아직 서버에 수신 중이라면 종료 함







```cs
// 해당 오브젝트가 삭제되면
    private void OnDestroy()
    {
        // GameManager 존재 여부 확인 후 실행
        GameManager?.Dispose();
    }

private void OnDestroy()
    {
        GameManager?.Dispose();
    }



```
### Host와 Client에 각 삭제 시 해당 싱글톤의 Dispose() 실행





## 주요 코드 정리
### 씬 편집 상태
```cs
[InitializeOnLoad] => 시작 시 실행을 하게 함
PlayModeStateChange.ExitingEditMode => 씬 저장을 하지 않고 플레이 버튼을 누른 상태
EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo(); => 씬 저장할 것인지 여부 선택 창 활성화
PlayModeStateChange.EnteredPlayMode => 플레이 버튼을 눌른 상태
```
 
### 인터페이스
```cs
class 클래스명 : IDisposable
public void Dispose() => 즉각 인스턴스를 삭제하게 함[없어도 삭제가 되지만 언제 삭제 될지 모르기 때문]
    {

    }
```
