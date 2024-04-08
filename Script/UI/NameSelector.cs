using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectButton;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 12;

    public const string PlayerNameKey = "PlayerName";

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


    public void HandleNameChanged()
    {
        // name의 글씨가 1자리 이상 12자리 이하라면 활성화 그 외는 비활성화
        connectButton.interactable = 
            nameField.text.Length >= minNameLength &&
            nameField.text.Length <= maxNameLength;

    }

    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);

        // 현재 빌드 중인 씬의 다음 순서로 로드 시킴
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
