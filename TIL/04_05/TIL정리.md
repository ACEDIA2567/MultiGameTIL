# 요약
## 호스트가 게임을 참가를 하면 그 방의 정보(방명, 인원 수)와 참가 버튼을 만들어 게임에 참가하게 구현함

```cs
[SerializeField] private TMP_Text LobbyNameText;  // 방 제목
[SerializeField] private TMP_Text LobbyPlayerText; // 인원 수
```
## 방의 제목과 인원 수의 정보를 가져옴

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
## 

```cs
public void join()
    {
        lobbyList.JoinAsync(lobby);
    }
```
