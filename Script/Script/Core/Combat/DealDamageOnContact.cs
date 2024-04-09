using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private ulong ownerClientId; // Ŭ���̾�Ʈ�� Id

    // Ŭ���̾�Ʈ Id Ȯ���� ���� ����
    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �浹ü�� Rigidbody�� ������ ����
        if (collision.attachedRigidbody == null) { return; }

        // �浹ü�� 
        if (collision.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            // Ŭ���̾�Ʈ�� Id�� ������ ����
            if( ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }

        // �ڽ��� Id�� �ƴϹǷ� �浹�� �ٸ� Ŭ���̾�Ʈ���� ������ ����
        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
