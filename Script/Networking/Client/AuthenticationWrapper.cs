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
        // 인증이 된 상태라면
        if (AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }

        // 연결 상태 중이라면
        if (AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating!");
            await Authenticating();
            return AuthState;
        }

        // 인증 시도
        await SignInAnonymouslyAsync(maxRetries);

        // 인증이 끝났으므로 상태를 반환해줌
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
        // 상태를 연결 중으로 변경
        AuthState = AuthState.Authenticating;

        int retries = 0;
        while (AuthState == AuthState.Authenticating && retries < maxRetries)
        {
            try
            {
                // 익명으로 설정 (스팀, 구글, 여러 계정으로도 가능)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                // 로그인이 되었거나 인정을 받았다면
                if (AuthenticationService.Instance.IsSignedIn &&
                    AuthenticationService.Instance.IsAuthorized)
                {
                    // 인증 중인 상태를 인증된 상태로 변경
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            // 인증 예외
            catch(AuthenticationException authException)
            {
                Debug.LogError(authException);
                AuthState = AuthState.Error;
            }
            // 요청 실패
            catch(RequestFailedException requestException)
            {
                Debug.LogError(requestException);
                AuthState = AuthState.Error;
            }

            // 만약에 못했다면 시도 횟수 1씩 증가
            retries++;

            // 딜레이 시간을 줌 (1000 = 1초)
            await Task.Delay(1000);
        }
        
        // 실패를 하지 않았지만 시간이 지났을 경우
        if (AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successful after{retries} retries");
            AuthState = AuthState.TimeOut;
        }
    }

}

public enum AuthState
{
    NotAuthenticated, // 인증 X
    Authenticating,   // 인증 중
    Authenticated,    // 인증 O
    Error,            // 에러
    TimeOut           // 시간 지남
}
