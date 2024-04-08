using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private Health health;
    [SerializeField] private Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        // 소유자가 아니라면
        if (!IsClient) { return; }

        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        // 구독 이후 변경된 경우를 대비하여 수동으로 호출
        // CurrentHealth의 값을 MaxHealth로 초기화를 하기 때문에
        HandleHealthChanged(0, health.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        // 소유자가 아니라면
        if (!IsClient) { return; }

        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        healthBarImage.fillAmount = Mathf.Clamp01((float)newHealth / health.MaxHealth);
    }
}
