using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float horizontalMove;

    [Header("Jump")]
    [SerializeField] private bool iCanJump;
    [SerializeField] private float jumpPower = 1f;
    [SerializeField] private const float jumpTimeLimit = 0.33f; // �ִ� ���� �ð�
    [SerializeField] private float jumpTimer = 0f;

    [Header("Components")]
    [SerializeField] private Rigidbody2D playerRigidbody;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonUp("Jump")) // ���� ��ư�� ���� ��
        {
            iCanJump = false;
            jumpTimer = 0;
        }

        //Debug.Log(playerRigidbody.velocity.y);
    }

    private void FixedUpdate()
    {
        Movement(horizontalMove);
    }

    private void Movement(float moveVelocity) // �÷��̾� �̵��� ����
    {
        float velocityY;

        Vector3 velocity = new Vector3(moveVelocity, 0, 0);
        velocity *= speed;

        if (iCanJump && Input.GetButton("Jump")) // Jump ��ư �Է� �� Jump() �޼��� ȣ��
            velocityY = JumpVelocity();
        else
            velocityY = playerRigidbody.velocity.y;

        if (velocityY < 0f) playerRigidbody.gravityScale = 2.0f; // �߶� �߷� ������ ����
        if (velocityY < -6.0f) velocityY = -6.0f; // �ִ� �߶� �ӵ� -6.0f

        velocity.y = velocityY;

        playerRigidbody.velocity = velocity;
    }

    private float JumpVelocity() // �÷��̾� ���� Y ��
    {
        jumpTimer += Time.deltaTime; // ���� Ÿ�̸� ����

        switch (jumpTimer)
        {
            case <= 0.11f:
                return 1.0f * jumpPower;

            case <= 0.22f:
                return 1.5f * jumpPower;

            case < jumpTimeLimit:
                return 2.0f * jumpPower;

            default:
                iCanJump = false;
                jumpTimer = 0;
                return playerRigidbody.velocity.y;
        }

        //return Mathf.Sin(1.902f * jumpTimer) * jumpPower; // �ﰢ�Լ� Sin ������ ó�� ������
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //collision.IsTouchingLayers(LayerMask.NameToLayer("Player"));
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            iCanJump = true; // ���� ����
            playerRigidbody.gravityScale = 1;
        }
    }

    void TempMove() // ���� �÷��̾� ������
    {
        float fallSpeed = playerRigidbody.velocity.y;

        Vector3 velocity = new Vector3(horizontalMove, 0, 0);

        velocity *= speed;

        Debug.Log("Move �Լ�: " + fallSpeed);

        velocity.y = fallSpeed;
        /* velocity�� ���� ������Ʈ��
         * �ӵ��� �ٷ� ������.
         * �����ؾ� �� ����,
         * �߶��� ���� �ӵ��̴�.
         * ó�� �߶��� �� �ӵ���
         * �������� �ִ� ���� �߿��ϴ�. */
        playerRigidbody.velocity = velocity;
    }
    void TempJump() // ���� �÷��̾� ����
    {
        jumpTimer += Time.deltaTime; // ���� Ÿ�̸� ����

        if (jumpTimer >= jumpTimeLimit)
        {
            iCanJump = false; // ���� �Ұ�
            jumpTimer = 0;
            //Debug.Log("���� �Ұ�| iCanJump: " + iCanJump + "| jumpTimer: " + jumpTimer);
            playerRigidbody.gravityScale += 2;
            return;
        }


        if (jumpTimer <= 0.11f && jumpTimer >= 0)
        {
            playerRigidbody.AddForce(Vector2.up * jumpPower * 0.7f, ForceMode2D.Impulse);
        }
        if (jumpTimer <= 0.22f && jumpTimer > 0.11f)
        {
            playerRigidbody.AddForce(Vector2.up * jumpPower * 1f, ForceMode2D.Impulse);
        }
        /* ForceMode.Impulse �� ������ �ٵ� ������Ʈ ��� �߿�
         * Mass(����)�� ������ ������ ª�� ������ ���� ���ϴ� ����̴�. */
    }
}