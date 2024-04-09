using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxRetries = 5)
    {
        // ������ �� ���¶��
        if (AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }

        // ���� ���� ���̶��
        if (AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating!");
            await Authenticating();
            return AuthState;
        }

        // ���� �õ�
        await SignInAnonymouslyAsync(maxRetries);

        // ������ �������Ƿ� ���¸� ��ȯ����
        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {
        // ���¸� ���� ������ ����
        AuthState = AuthState.Authenticating;

        int retries = 0;
        while (AuthState == AuthState.Authenticating && retries < maxRetries)
        {
            try
            {
                // �͸����� ���� (����, ����, ���� �������ε� ����)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                // �α����� �Ǿ��ų� ������ �޾Ҵٸ�
                if (AuthenticationService.Instance.IsSignedIn &&
                    AuthenticationService.Instance.IsAuthorized)
                {
                    // ���� ���� ���¸� ������ ���·� ����
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            // ���� ����
            catch(AuthenticationException authException)
            {
                Debug.LogError(authException);
                AuthState = AuthState.Error;
            }
            // ��û ����
            catch(RequestFailedException requestException)
            {
                Debug.LogError(requestException);
                AuthState = AuthState.Error;
            }

            // ���࿡ ���ߴٸ� �õ� Ƚ�� 1�� ����
            retries++;

            // ������ �ð��� �� (1000 = 1��)
            await Task.Delay(1000);
        }
        
        // ���и� ���� �ʾ����� �ð��� ������ ���
        if (AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successful after{retries} retries");
            AuthState = AuthState.TimeOut;
        }
    }

}

public enum AuthState
{
    NotAuthenticated, // ���� X
    Authenticating,   // ���� ��
    Authenticated,    // ���� O
    Error,            // ����
    TimeOut           // �ð� ����
}
