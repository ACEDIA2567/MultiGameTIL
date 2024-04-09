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

        // 1. ��Ʈ��ũ �����ڰ� null�� ���
        if (NetworkManager == null)
        {
            // 2. ��Ʈ��ũ �����ڰ� Ŭ���̾�Ʈ�� ���� ����
            if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
            {
                // ������ ������Ʈ�� ������ Ŀ���Ѵ�.
                if (CanCommitToTransform)
                {
                    // ������Ʈ�� ������ ������ Ŀ���Ѵ�.
                    TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                    // NetworkManager.LocalTime.Time = 
                    // �� �������� �׻� ���� �ð��� �������� �ʱ� ������
                    // �� ��ȯ�� ������Ʈ ������ �ð��� �󸶳� �Ǿ����� �˾ƾ� ��
                }

            }
        }
    }

    // ���� ���� ����
    protected override bool OnIsServerAuthoritative()
    {
        // ������ ������ ���ٰ� false�� ����
        return false;
    }
}
