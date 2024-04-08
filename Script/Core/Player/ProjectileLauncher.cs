using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet coinWallet; // �÷��̾��� ��������
    [SerializeField] private GameObject ServerProjectile; // Ŭ���̾�Ʈ�� ź ������Ʈ
    [SerializeField] private GameObject ClientProjectile; // ������ ź ������Ʈ
    [SerializeField] private Transform projectileSpawnPoint; // ź���� ���� ���� ����
    [SerializeField] private GameObject muzzleFlash;      // ���� ����Ʈ
    [SerializeField] private Collider2D playerCollider;


    [Header("Settings")]
    [SerializeField] private float ProjectileSpeed = 4.0f;
    [SerializeField] private float fireRate = 2.0f;
    [SerializeField] private float muzzleFlashDuration = 0.1f;
    [SerializeField] private int costToFire; // �߻縦 ���� ���η�

    private bool shouldFire; // ���콺 Ŭ�� ����
    private float timer; // �߻� ��� �ð�
    private float muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        // �����ڰ� �ƴ϶��
        if (!IsOwner) { return; }

        inputReader.PrimaryFireEvent += HandlePrimaryEvent;
    }

    public override void OnNetworkDespawn()
    {
        // �����ڰ� �ƴ϶��
        if (!IsOwner) { return; }
        inputReader.PrimaryFireEvent -= HandlePrimaryEvent;
    }

    private void HandlePrimaryEvent(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    private void Update()
    {
        // �÷��� �ð� 
        if (muzzleFlashTimer > 0.0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            
            // �÷��� Ÿ���� 0���� �۰ų� ���ٸ�
            if (muzzleFlashTimer <= 0.0f)
            {
                // �÷��� ��Ȱ��ȭ(����)
                muzzleFlash.SetActive(false);
            }
        }

        // �����ڰ� �ƴ϶��
        if (!IsOwner) { return; }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        // ���콺 Ŭ���� �ϰ� ���� �ʴٸ�
        if (!shouldFire) { return; }

        if (timer > 0) { return; }

        // Ŭ���̾�Ʈ�� ���������� ���η��� �߻� �ڽ�Ʈ���� ���ٸ�
        if (coinWallet.TotalCoins.Value < costToFire) { return; }

        // Ŭ���̾�Ʈ�� ������ ���� ��
        // ������ ź�� ���� ����
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

        // Ŭ���̾�Ʈ�� ź�� ���� ����
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        // ź �߻� �� �߻� �ð��� �ʱ�ȭ
        timer = 1 / fireRate;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        // Ŭ���̾�Ʈ�� ���������� ���η��� �߻� �ڽ�Ʈ���� ���ٸ�
        if (coinWallet.TotalCoins.Value < costToFire) { return; }
        // �Ҹ����� ��ŭ ����
        coinWallet.SpendCoins(costToFire);

        // ���� ź�� ����
        GameObject projectileInstantiate = Instantiate(ServerProjectile, spawnPos, Quaternion.identity);

        // ź���� ������ ����
        projectileInstantiate.transform.up = direction;

        // Ŭ���̾�Ʈ(�ڽ�)�� ������ ź�� �浹 ó���� ���� ��Ŵ
        Physics2D.IgnoreCollision(playerCollider, projectileInstantiate.GetComponent<Collider2D>());

        // �߻��� ������Ʈ�� Ŭ���̾�Ʈ Id�� ������
        if (projectileInstantiate.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamageOnContact))
        {
            dealDamageOnContact.SetOwner(OwnerClientId);
        }

        // ź �̵�
        if (projectileInstantiate.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * ProjectileSpeed;
        }

        // ������ ���� �� �ٸ� Ŭ���̾�Ʈ���� ź�� ���� �۽�
        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        // �����ڶ��
        if (IsOwner) { return; }

        // �ٸ� Ŭ���̾�Ʈ�� ź�� ����
        SpawnDummyProjectile(spawnPos, direction);
    }

    // ����ź�� ���� �Լ�
    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        // �÷��� (Ȱ��ȭ)����
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;


        // Ŭ���̾�Ʈ ź�� ����
        GameObject projectileInstantiate =  Instantiate(ClientProjectile, spawnPos, Quaternion.identity);

        // ź���� ������ ����
        projectileInstantiate.transform.up = direction;

        // Ŭ���̾�Ʈ(�ڽ�)�� ź�� �浹 ó���� ���� ��Ŵ
        Physics2D.IgnoreCollision(playerCollider, projectileInstantiate.GetComponent<Collider2D>());

        // ź �̵�
        if (projectileInstantiate.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * ProjectileSpeed;
        }
    }
}
