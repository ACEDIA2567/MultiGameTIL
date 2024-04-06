# 2024-04-05 TIL
## 1. 로비씬에 버튼을 만들어 로비창을 활성화 여부를 결정    <br/>2. 참가한 플레이어가 있는 방이 있으면 로비창에 나오게 함    <br/>3. 다른 플레이어가 볼 수 있게 방의 이름과 인원 수가 나오게 하고 버튼으로 게임 참가 하게 함

```cs
[SerializeField] private TMP_Text LobbyNameText;  // 방 제목
[SerializeField] private TMP_Text LobbyPlayerText; // 인원 수

private LobbyList lobbyList; // 로비창 정보
private Lobby lobby;         // 로비의 정보
```
### 변수 선언

```cs
// 로비 UI의 Text 활성화
    public void Initialise(LobbyList lobbyList, Lobby lobby)
    {
        // 컴포넌트되어 있는 LobbyList의 정보를 받아옴
        this.lobbyList = lobbyList;
        this.lobby = lobby;

        LobbyNameText.text = lobby.Name;
        LobbyPlayerText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }
```
### 로비의 이름과 인원수와 최대 인원수를 나타내는 변수의 text정보로 초기화하여 출력하게 함

```cs
public void join()
    {
        lobbyList.JoinAsync(lobby);
    }
```
### 버튼과 연결 할 함수로 해당 lobby로 참가하게 함

```cs
[SerializeField] private Transform lobbyItemParent; // 로비의 부모
[SerializeField] private LobbyItem lobbyItemPrefab; // 로비 Prefab

private bool isJoining = false;    // 참가 여부
private bool isRefreshing = false; // 새로고침 여부
```
### 변수 선언

```cs
private void OnEnable()
    {
        RefreshList();
    }
```
### 해당 스크립트를 컴포넌트가 되어있는 오브젝트가 활성화가 되면 RefreshList()를 실행 함

```cs
public async void RefreshList()
    {
        if (isRefreshing) { return; }
        isRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25; // 필터의 상위 25개 설정

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT, // GT:크다
                    value: "0"), // 값이 0
                    // AvailableSlots의 값이 0보다 큰 로비를 요청함
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked, // 잠김
                    op: QueryFilter.OpOptions.EQ, // GT:크다
                    value: "0")
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
```
### 새로고침을 했기 때문에 isRefreshing을 true변경 후
### 옵션의 정보를 변수로 받아와서 옵션의 필터 속성을 설정 한 다음
### 해당 필터에 속한 로비들의 정보를 QueryResponse lobbies에 변수로 저장

```cs
            // 현재 존재하는 로비들을 삭제
            foreach(Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            // 필터로 존재하는 로비들을 생성
            foreach (Lobby lobby in lobbies.Results)
            {
                // 로비들을 생성하여 변수로 저장
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                // 저장한 변수(로비)들의 Text 갱신
                lobbyItem.Initialise(this, lobby);
            }
        }
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex);
        }

        // 새로고침을 완료했으므로 false 변경
        isRefreshing = false;
    }
```
### 존재했던 로비들을 다 삭제 후 저장했던 lobbies에서
### foreach문으로 생성을 하고 Initialise를 사용하여 Text갱신 함
### 그 후 새로고침을 완료하였기 때문에 isRefreshing를 false로 변경

```cs
public async void JoinAsync(Lobby lobby)
    {
        // 접속 중이라면 반환
        if (isJoining) { return; }
        // 접속을 했기 때문에 참가 여부 true 변경
        isJoining = true;

        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex);
            return;
        }

        // 클라이언트의 싱글톤으로 joinCode로 참여를 끝냈기 때문에 참가 여부 false로 변경
        isJoining = false;
    }
```
### isJoining로 접속 여부로 반환을 하고 false라면 true로 변경함
### join()버튼을 클릭 시 해당 로비의 정보가 들어와서
### 로비에 대한 정보를 변수로 저장하여 참여코드를 string joinCode로 저장 한 뒤
### ClientSingleton의 GameManager의 참여코드 매개변수로 joinCode를 추가하여 게임에 참가한다.
### 이후 참여를 했으므로 isJoining를 false로 변경


## 주요 코드 정리
### 로비
```cs
QueryLobbiesOptions options = new QueryLobbiesOptions(); => 로비 필터 옵션를 변수로 저장
options.Count = 25; => 필터에 나올 개수
QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options); => options의 필터로 나온 List들을 lobbies에 저장
```

### 접속
```cs
Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id); => lobby.Id의 값을 가진 Lobby 정보를 joiningLobby에 변수로 저장
string joinCode = joiningLobby.Data["JoinCode"].Value; => joiningLobby의 딕셔너리 "JoinCode"의 값을 joinCode로 저장
ClientSingleton.Instance.GameManager.StartClientAsync(joinCode); => joinCode를 매개변수로 하여 StartClientAsync 실행
```
