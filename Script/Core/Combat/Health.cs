using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    // �ִ� ü��
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    // ���������� �ǵ� �� �ִ� ü��
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    // ���� ����
    private bool isDead;

    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        // ������ �ƴ϶��
        if (!IsServer) { return; }

        CurrentHealth.Value = MaxHealth;

    }

    public override void OnNetworkDespawn()
    {
        // ������ �ƴ϶��
        if (!IsServer) { return; }

    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    private void ModifyHealth(int value)
    {
        // �׾��ٸ�
        if (isDead) { return; }

        // ü�� ���� �� ����
        int newHealth = CurrentHealth.Value + value;
        // ���� ü���� 0�� MaxHealth���� ������ �����ϰ� ��
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        // ü���� 0�� �Ǿ��� ��
        if (CurrentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            isDead = true;
        }
    }
}
