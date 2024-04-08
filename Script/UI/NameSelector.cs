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
        // ���� ������� ���� �̸��� �ʿ� ���� ������ return��
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            // ���� ���� ���� ���� ���� ������ �ε� ��Ŵ
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        // PlayerNameKey��� ����ִ� string������ ������ �� Text ������ ����
        nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        HandleNameChanged();
    }


    public void HandleNameChanged()
    {
        // name�� �۾��� 1�ڸ� �̻� 12�ڸ� ���϶�� Ȱ��ȭ �� �ܴ� ��Ȱ��ȭ
        connectButton.interactable = 
            nameField.text.Length >= minNameLength &&
            nameField.text.Length <= maxNameLength;

    }

    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);

        // ���� ���� ���� ���� ���� ������ �ε� ��Ŵ
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
