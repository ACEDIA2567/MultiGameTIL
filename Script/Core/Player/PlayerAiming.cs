using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform turretTransform;

    private void LateUpdate()
    {
        // 소유자가 아니라면
        if (!IsOwner) { return; }

        Vector2 aimScreenPosition = inputReader.AimPos;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y);
            
    }
}
