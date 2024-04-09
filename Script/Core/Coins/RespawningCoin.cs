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
        // 서버가 아니라면
        if (!IsServer) 
        {
            Show(false); 
            return 0;
        }

        // 이미 획득했다면
        if (alreadyCollected) { return 0; }

        // 획득을 하지 못한 상태에서 획득을 했기 때문에
        alreadyCollected = true;

        onCollected?.Invoke(this);

        return coinValue;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
