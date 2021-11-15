using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private bool iCanUsingLadder;
    [SerializeField] private bool usingLadder;
    [SerializeField] private float horizontalMove;
    [SerializeField] private float verticalMove;

    [Header("Jump")]
    [SerializeField] private bool iCanJump;
    [SerializeField] public bool UpInJumping; // �����ϴ� ���� �ö󰡴°�?
    [SerializeField] public bool CheckFeet;
    [SerializeField] private float jumpPower = 1f;
    [SerializeField] private float jumpTimer = 0f;
    private const float jumpTimeLimit = 0.33f; // �ִ� ���� �ð�

    [Header("etc Field Value")]
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] public Vector3 MyFeetPosition;

    [Header("Components")]
    [SerializeField] private Rigidbody2D playerRigidbody;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        iCanUsingLadder = false;
    }

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonUp("Jump")) // ���� ��ư�� ���� ��
        {
            iCanJump = false;
            jumpTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!usingLadder) Movement(horizontalMove); // ��ٸ� ��� ���� �ƴ� �� �¿��̵� ����

        if (iCanUsingLadder) UsingLadder(verticalMove); // '��ٸ� ������Ʈ'�� Ʈ���� �� ���Ʒ� �Է����� ��ٸ� ��� ����

        CheckMyFeet(); // �� �� �˻�

        // ��ٸ� ��ȣ�ۿ� ��, ���� ������Ʈ �浹 ����
        if (usingLadder)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Railing"), LayerMask.NameToLayer("Player"), true); // �� ���̾ �浹 ����
            playerRigidbody.gravityScale = 0f; // ��ٸ� �̿� �� �߷� ������ 0
        }
        else if (!usingLadder)
        {
            // ��ٸ� �̿� ������ ��, ���� ������Ʈ �浹 ���δ� �߹� üũ�� �����Ѵ�.
            if (CheckFeet) Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Railing"), LayerMask.NameToLayer("Player"), false); // �� ���̾ �浹 ����

            if (playerRigidbody.velocity.y < 0) // �߶��ϰ� ���� ���
            {
                UpInJumping = false;
                playerRigidbody.gravityScale = 2.0f;
            }
            else if (playerRigidbody.velocity.y >= 0) // ���� ��ȭ�� ���ų� ��� ���� ���
            {
                playerRigidbody.gravityScale = 1.0f;
            }
        }
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

        //if (velocityY < 0f) playerRigidbody.gravityScale = 2.0f; // �߶� �߷� ������ ����
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

    private void UsingLadder(float moveVertical) // ��ٸ� �̿�
    {
        if (moveVertical != 0)
        {
            usingLadder = true;
            iCanJump = false; // ��ٸ� ��� ���϶��� ������ UsingLadder()���� �����Ѵ�
        }
        else if (usingLadder) goto Ladder;
        else return;

        Ladder:
        // �÷��̾�� ��, �Ʒ��θ� �̵��� �� �ִ�.
        Vector3 velocity = new Vector3(0, moveVertical, 0);
        //velocity *= speed; // ��ٸ� �ö󰡴� �ӵ��� ������ �ұ�?

        // ��ٸ� �̿� ��, Jump �Է����� ��ٸ� �̿��� ������ �� �ִ�.
        if (Input.GetButton("Jump"))
        {
            usingLadder = false;
            playerRigidbody.gravityScale = 1.0f;

            if (horizontalMove != 0)
            {
                iCanJump = true;
                return;
            }
            return;
        }

        playerRigidbody.velocity = velocity;
    }

    private void CheckMyFeet() // �÷��̾� �߹ؿ� ���� �ִ���?
    {
        //Ray2D ray = new Ray2D(transform.position,Vector2.down);
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position - new Vector3(0, 0.01f, 0), Vector2.down, 1.87f,
                (1 << LayerMask.NameToLayer("Ground")) + (1 << LayerMask.NameToLayer("Railing")));

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
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground")) // Edge �ݶ��̴��� �׶��� ������Ʈ�� Ʈ���� ��
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Railing"), LayerMask.NameToLayer("Player"), false); // �� ���̾ �浹 ����
            UpInJumping = false;
            iCanJump = true; // ���� ����
        }
        
        // �÷��̾ ��ٸ��� Ʈ���� ���� ��, bool ������ ��ٸ� ��ȣ�ۿ� ���� ���θ� üũ�Ѵ�.
        if(collision.CompareTag("Ladder")) // ��ٸ��� ������ true
        {
            iCanUsingLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Ladder")) // ��ٸ����� ����� false
        {
            iCanUsingLadder = false;
            usingLadder = false;
        }
    }
}