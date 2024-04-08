using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private ulong ownerClientId; // 클라이언트의 Id

    // 클라이언트 Id 확인을 위한 변수
    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌체에 Rigidbody가 없으면 리턴
        if (collision.attachedRigidbody == null) { return; }

        // 충돌체의 
        if (collision.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            // 클라이언트의 Id가 같으면 리턴
            if( ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }

        // 자신의 Id가 아니므로 충돌한 다른 클라이언트에게 데미지 적용
        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
