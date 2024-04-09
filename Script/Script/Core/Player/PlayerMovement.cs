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
        // �����ڰ� �ƴ϶��
        if (!IsOwner) { return; }

        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        // �����ڰ� �ƴ϶��
        if (!IsOwner) { return; }

        inputReader.MoveEvent -= HandleMove;
    }

    private void Update()
    {
        // �����ڰ� �ƴ϶��
        if (!IsOwner) { return; }

        // ���� = �� �Ǵ� �� * ȸ���ӵ� * ������
        float z = PreMoveInput.x * -turningRage * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, z);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }

        // �ӵ� = ���� * �� �Ǵ� �� * �̵��ӵ�
        rb.velocity = (Vector2)bodyTransform.up * PreMoveInput.y * moveSpeed;
    }

    private void HandleMove(Vector2 movementInput)
    {
        PreMoveInput = movementInput;
    }

}
