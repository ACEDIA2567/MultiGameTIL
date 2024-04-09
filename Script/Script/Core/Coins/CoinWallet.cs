using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌체의 컴포넌트에 Coin여부 확인
        if (!collision.TryGetComponent<Coin>(out Coin coin)) { return; }

        int coinValue = coin.Collect();

        if (!IsServer) { return; }

        TotalCoins.Value += coinValue;
    }

    public void SpendCoins(int costCoin)
    {
        TotalCoins.Value -= costCoin;
    }
}
