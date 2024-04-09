# 2024-04-08 TIL

## 1. 이름을 입력

```cs
[SerializeField] private TMP_InputField nameField; // 이름
[SerializeField] private Button connectButton;   // 버튼 정보
[SerializeField] private int minNameLength = 1;  // 최소 길이
[SerializeField] private int maxNameLength = 12; // 최대 길이

public const string PlayerNameKey = "PlayerName";
```
### 변수 선언

```cs
private void Start()
    {
        // 전용 서버라면 별도 이름이 필요 없기 때문에 return함
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            // 현재 빌드 중인 씬의 다음 순서로 로드 시킴
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        // PlayerNameKey라는 비어있는 string형으로 저장한 뒤 Text 변수에 저장
        nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        HandleNameChanged();
    }
```
### 전용 서버 여부 따라 씬 이동 처리 후 nameField.text에 PlayerNamekey의 내용으로 초기화함
### 만약 PlayerNameKey가 존재하지 않다면 string.Empty를 사용하여 비어있게 함
### 그 후 이름 길이에 따른 버튼 활성화 메서드(HandleNameChanged)로 이동

```cs
public void HandleNameChanged()
    {
        // name의 글씨가 1자리 이상 12자리 이하라면 활성화 그 외는 비활성화
        connectButton.interactable = 
            nameField.text.Length >= minNameLength &&
            nameField.text.Length <= maxNameLength;
    }
```
### 버튼의 기능으로서 nameField의 Text 길이가 최소와 최대 길이 조건에 맞으면 활성화 / 그 외는 비활성화

```cs
public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);

        // 현재 빌드 중인 씬의 다음 순서로 로드 시킴
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
```
### 버튼의 기능으로서 PlayerNameKey를 nameField.text로 변수 저장하여 이름을 저장
### 현재의 씬 다음에 존재하는 씬으로 이동

```cs
public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        // 연결 요청을 할때마다 ApprovalCheck메서드를 실행
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }
```
### ConnectionApprovalCallback를 사용하여 연결할 때마다 ApprovalCheck를 실행하게 함

```cs
private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        // request.Payload를 UTF8의 문자열 변수로 변환하여 저장
        string playload = System.Text.Encoding.UTF8.GetString(request.Payload);
        // playload를 Json문자열로 변경하여 사용 가능하게 저장함
        UserData userData = JsonUtility.FromJson<UserData>(playload);

        Debug.Log(userData.userName);

        response.Approved = true;
        // 연결 승인을 재정의 했으므로 플레이어 오브젝트를 생성함
        response.CreatePlayerObject = true;
    }
```
### 요청한 클라이언트의 Payload를 Json문자열로 변경하여 userData에 저장함

```cs
[Serializable]
public class UserData
{
    public string userName;
}
```
### 저장한 데이터 즉 클라이언트의 userName을 로그하여 이름을 출력함

```cs
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
```
### Client, Host의 GameManager에서 각자 서버에 자신의 이름 userName에 저장 후
### 문자열 Json파일을 Bytes로 변환하여 서버로 전달

```cs
string PlayerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "unknown Lobby");
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(
                $"{PlayerName}'s Lobby", MaxConnections, lobbyOptions);
```
### 후 호스트에서는 자신의 이름을 로비의 명에 추가로 하여 해당 로비의 호스트를 알리게 함


## 주요 코드
### 버튼
```cs
connectButton.interactable = 
            nameField.text.Length >= minNameLength &&
            nameField.text.Length <= maxNameLength; => 해당 조건에 만족 시 버튼 활성화 / 아니라면 비활성화
```

### 씬
```cs
SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); => 현재 사용 중인 씬을 다음 순서의 씬으로 로드
```

```cs
nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty); => nameField.text에 PlayerNameKey에 저장된 value값으로 저장 / 만약 value가 없다면 string.Empty로 저장
PlayerPrefs.SetString(PlayerNameKey, nameField.text); => PlayerNameKey의 변수에 nameField.text를 value로 저장
```
