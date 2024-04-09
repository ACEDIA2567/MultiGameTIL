using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    // 최대 체력
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    // 서버에서만 건들 수 있는 체력
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    // 죽음 여부
    private bool isDead;

    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        // 서버가 아니라면
        if (!IsServer) { return; }

        CurrentHealth.Value = MaxHealth;

    }

    public override void OnNetworkDespawn()
    {
        // 서버가 아니라면
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
        // 죽었다면
        if (isDead) { return; }

        // 체력 증가 및 감소
        int newHealth = CurrentHealth.Value + value;
        // 현재 체력을 0과 MaxHealth사이 값으로 고정하게 함
        CurrentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        // 체력이 0이 되었을 때
        if (CurrentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            isDead = true;
        }
    }
}
