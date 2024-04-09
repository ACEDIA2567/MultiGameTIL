using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform   bodyTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Setting")]
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private float turningRage = 30f;

    private Vector2 PreMoveInput;

    public override void OnNetworkSpawn()
    {
        // 소유자가 아니라면
        if (!IsOwner) { return; }

        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        // 소유자가 아니라면
        if (!IsOwner) { return; }

        inputReader.MoveEvent -= HandleMove;
    }

    private void Update()
    {
        // 소유자가 아니라면
        if (!IsOwner) { return; }

        // 각도 = 왼 또는 우 * 회전속도 * 프레임
        float z = PreMoveInput.x * -turningRage * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, z);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }

        // 속도 = 방향 * 앞 또는 뒤 * 이동속도
        rb.velocity = (Vector2)bodyTransform.up * PreMoveInput.y * moveSpeed;
    }

    private void HandleMove(Vector2 movementInput)
    {
        PreMoveInput = movementInput;
    }

}
