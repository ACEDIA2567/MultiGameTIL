using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet coinWallet; // 플레이어의 코인지갑
    [SerializeField] private GameObject ServerProjectile; // 클라이언트의 탄 오브젝트
    [SerializeField] private GameObject ClientProjectile; // 서버의 탄 오브젝트
    [SerializeField] private Transform projectileSpawnPoint; // 탄알의 스폰 지점 정보
    [SerializeField] private GameObject muzzleFlash;      // 포구 임펙트
    [SerializeField] private Collider2D playerCollider;


    [Header("Settings")]
    [SerializeField] private float ProjectileSpeed = 4.0f;
    [SerializeField] private float fireRate = 2.0f;
    [SerializeField] private float muzzleFlashDuration = 0.1f;
    [SerializeField] private int costToFire; // 발사를 위한 코인량

    private bool shouldFire; // 마우스 클릭 여부
    private float timer; // 발사 대기 시간
    private float muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        // 소유자가 아니라면
        if (!IsOwner) { return; }

        inputReader.PrimaryFireEvent += HandlePrimaryEvent;
    }

    public override void OnNetworkDespawn()
    {
        // 소유자가 아니라면
        if (!IsOwner) { return; }
        inputReader.PrimaryFireEvent -= HandlePrimaryEvent;
    }

    private void HandlePrimaryEvent(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    private void Update()
    {
        // 플래쉬 시간 
        if (muzzleFlashTimer > 0.0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            
            // 플래쉬 타임이 0보다 작거나 같다면
            if (muzzleFlashTimer <= 0.0f)
            {
                // 플래쉬 비활성화(삭제)
                muzzleFlash.SetActive(false);
            }
        }

        // 소유자가 아니라면
        if (!IsOwner) { return; }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        // 마우스 클릭을 하고 있지 않다면
        if (!shouldFire) { return; }

        if (timer > 0) { return; }

        // 클라이언트의 코인지갑의 코인량이 발사 코스트보다 적다면
        if (coinWallet.TotalCoins.Value < costToFire) { return; }

        // 클라이언트가 공격을 했을 때
        // 서버로 탄알 생성 지시
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

        // 클라이언트의 탄알 더미 생성
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        // 탄 발사 후 발사 시간을 초기화
        timer = 1 / fireRate;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        // 클라이언트의 코인지갑의 코인량이 발사 코스트보다 적다면
        if (coinWallet.TotalCoins.Value < costToFire) { return; }
        // 소모코인 만큼 감소
        coinWallet.SpendCoins(costToFire);

        // 서버 탄알 생성
        GameObject projectileInstantiate = Instantiate(ServerProjectile, spawnPos, Quaternion.identity);

        // 탄알의 방향을 변경
        projectileInstantiate.transform.up = direction;

        // 클라이언트(자신)과 서버의 탄의 충돌 처리를 무시 시킴
        Physics2D.IgnoreCollision(playerCollider, projectileInstantiate.GetComponent<Collider2D>());

        // 발사한 오브젝트의 클라이언트 Id를 가져옴
        if (projectileInstantiate.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamageOnContact))
        {
            dealDamageOnContact.SetOwner(OwnerClientId);
        }

        // 탄 이동
        if (projectileInstantiate.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * ProjectileSpeed;
        }

        // 서버에 전달 후 다른 클라이언트에게 탄의 정보 송신
        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        // 소유자라면
        if (IsOwner) { return; }

        // 다른 클라이언트의 탄알 생성
        SpawnDummyProjectile(spawnPos, direction);
    }

    // 더미탄알 생성 함수
    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        // 플래쉬 (활성화)생성
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;


        // 클라이언트 탄알 생성
        GameObject projectileInstantiate =  Instantiate(ClientProjectile, spawnPos, Quaternion.identity);

        // 탄알의 방향을 변경
        projectileInstantiate.transform.up = direction;

        // 클라이언트(자신)와 탄의 충돌 처리를 무시 시킴
        Physics2D.IgnoreCollision(playerCollider, projectileInstantiate.GetComponent<Collider2D>());

        // 탄 이동
        if (projectileInstantiate.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * ProjectileSpeed;
        }
    }
}
