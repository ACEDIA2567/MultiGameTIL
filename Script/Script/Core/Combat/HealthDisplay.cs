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
        // �����ڰ� �ƴ϶��
        if (!IsClient) { return; }

        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        // ���� ���� ����� ��츦 ����Ͽ� �������� ȣ��
        // CurrentHealth�� ���� MaxHealth�� �ʱ�ȭ�� �ϱ� ������
        HandleHealthChanged(0, health.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        // �����ڰ� �ƴ϶��
        if (!IsClient) { return; }

        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        healthBarImage.fillAmount = Mathf.Clamp01((float)newHealth / health.MaxHealth);
    }
}
