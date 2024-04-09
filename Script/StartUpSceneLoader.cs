using System;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class StartUpSceneLoader
{
    static StartUpSceneLoader()
    {
        // ���� ���� �� �ٷ� ����
        EditorApplication.playModeStateChanged += LoadStartupScene;
    }

    private static void LoadStartupScene(PlayModeStateChange state)
    {
        // ������ ���� �ʰ� �÷��� ��ư�� ������ ��
        if(state == PlayModeStateChange.ExitingEditMode)
        {
            // ���� ���� ���� ������ ���� ���� Ȯ�� �� ����
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        // �÷��� ��ư�� ������ ��
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // EditorSceneManager�� ���� ���� 0��°�� �ƴ϶��
            if (EditorSceneManager.GetActiveScene().buildIndex != 0)
            {
                // 0��° ������ �̵�
                EditorSceneManager.LoadScene(0);
            }
        }
    }
}
