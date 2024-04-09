using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;
    }

    protected override void Update()
    {
        CanCommitToTransform = IsOwner;
        base.Update();

        // 1. 네트워크 관리자가 null인 경우
        if (NetworkManager == null)
        {
            // 2. 네트워크 관리자가 클라이언트에 연결 여부
            if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
            {
                // 서버에 오브젝트의 정보를 커밋한다.
                if (CanCommitToTransform)
                {
                    // 오브젝트의 정보를 서버에 커밋한다.
                    TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                    // NetworkManager.LocalTime.Time = 
                    // 각 프레임이 항상 같은 시간을 차지하지 않기 때문에
                    // 각 변환된 업데이트 사이의 시간이 얼마나 되었는지 알아야 함
                }

            }
        }
    }

    // 서버 권한 여부
    protected override bool OnIsServerAuthoritative()
    {
        // 서버에 권한이 없다고 false를 리턴
        return false;
    }
}
