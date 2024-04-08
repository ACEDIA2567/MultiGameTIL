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

