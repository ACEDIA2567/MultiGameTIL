using System;
using UnityEditor;
using UnityEditor.SceneManagement;

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
