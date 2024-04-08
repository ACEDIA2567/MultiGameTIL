using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin coinPrefab;
    
    [SerializeField] private int maxCoins = 50;
    [SerializeField] private int coinValue = 10;
    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 ySpawnRange;
    [SerializeField] private LayerMask layerMask;
    
    private Collider2D[] coinBuffer = new Collider2D[1];

    private float coinRadius;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        // 최대 코인 개수만큼 코인을 스폰함
        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(
            coinPrefab, 
            GetSpawnPoint(), 
            Quaternion.identity);

        // coin 점수 설정
        coinInstance.SetValue(coinValue);
        // coin을 생성
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.onCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;

        // layerMask의 영역에 coinRadius거리에 존재하는 coin이 없을 때 까지 반복
        while (true)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPoint = new Vector2(x, y);

            // spawnPoint의 좌표에 coinRadius(거리)에 있으며
            // layerMask의 영역에 존재하는 충돌체의 개수를 알아내려 함
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);

            // 영역에 존재하는 충돌체 즉, Coin이 없으면 해당 스폰 지점을 리턴 함
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }

    }
}
