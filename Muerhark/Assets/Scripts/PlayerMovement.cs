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
    [SerializeField] public bool UpInJumping; // �����ϴ� ���� �ö󰡴°�?
    [SerializeField] public bool CheckFeet;
    [SerializeField] private float jumpPower = 1f;
    [SerializeField] private float jumpTimer = 0f;
    private const float jumpTimeLimit = 0.33f; // �ִ� ���� �ð�

    [Header("Common")]
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] public Vector3 MyFeetPosition;

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

        if (playerRigidbody.velocity.y < 0)
        {
            UpInJumping = false;
        }

        //Debug.Log(playerRigidbody.velocity.y);
    }

    private void FixedUpdate()
    {
        Movement(horizontalMove);
        CheckMyFeet(); // �� �� �˻�
    }

    private void Movement(float moveVelocity) // �÷��̾� �̵��� ����
    {
        float velocityY;

        Vector3 velocity = new Vector3(moveVelocity, 0, 0);
        velocity *= speed;

        if (iCanJump && Input.GetButton("Jump")) // Jump ��ư �Է� �� Jump() �޼��� ȣ��
            velocityY = JumpVelocityY();
        else
            velocityY = playerRigidbody.velocity.y;

        if (velocityY < 0f) playerRigidbody.gravityScale = 2.0f; // �߶� �߷� ������ ����
        if (velocityY < -6.0f) velocityY = -6.0f; // �ִ� �߶� �ӵ� -6.0f

        velocity.y = velocityY;

        playerRigidbody.velocity = velocity;
    }

    private float JumpVelocityY() // �÷��̾� ���� Y ��
    {
        jumpTimer += Time.deltaTime; // ���� Ÿ�̸� ����

        switch (jumpTimer)
        {
            case <= 0.11f:
                UpInJumping = true;
                return 1.3f * jumpPower;

            case <= 0.22f:
                UpInJumping = true;
                return 1.3f * jumpPower;

            case < jumpTimeLimit:
                UpInJumping = true;
                return 1.3f * jumpPower;

            default:
                UpInJumping = true;
                iCanJump = false;
                jumpTimer = 0;
                return playerRigidbody.velocity.y;
        }

        //return Mathf.Sin(1.902f * jumpTimer) * jumpPower; // �ﰢ�Լ� Sin ������ ó�� ������
    }

    public Vector2 JumpRay() // ���� 2021-11-02
    {
        Ray2D ray = new Ray2D();

        ray.origin = transform.position;
        ray.direction = Vector2.down;

        return ray.GetPoint(100f);
    }

    private void CheckMyFeet() // �÷��̾� �� �ؿ� ���� �ִ���?
    {
        //Ray2D ray = new Ray2D(transform.position,Vector2.down);
        RaycastHit2D hit;

        
        hit = Physics2D.Raycast(transform.position - new Vector3(0, 0.01f, 0), Vector2.down, 1.87f/*, LayerMask.NameToLayer("Ground")*/);

        if (!hit.collider)
        {
            CheckFeet = false;
        }
        else
        {
            MyFeetPosition = hit.point;
            CheckFeet = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - new Vector3(0, 0.01f, 0), transform.position + (Vector3.down *1.87f));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //collision.IsTouchingLayers(LayerMask.NameToLayer("Player"));
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            UpInJumping = false;
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