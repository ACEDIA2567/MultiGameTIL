using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> onCollected;

    private Vector3 previousPosition;

    private void Update()
    {
        if (previousPosition != transform.position)
        {
            Show(true);
        }
        previousPosition = transform.position;
    }

    public override int Collect()
    {
        // ������ �ƴ϶��
        if (!IsServer) 
        {
            Show(false); 
            return 0;
        }

        // �̹� ȹ���ߴٸ�
        if (alreadyCollected) { return 0; }

        // ȹ���� ���� ���� ���¿��� ȹ���� �߱� ������
        alreadyCollected = true;

        onCollected?.Invoke(this);

        return coinValue;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
