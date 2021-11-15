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
    [SerializeField] public bool UpInJumping; // 점프하는 동안 올라가는가?
    [SerializeField] public bool CheckFeet;
    [SerializeField] private float jumpPower = 1f;
    [SerializeField] private float jumpTimer = 0f;
    private const float jumpTimeLimit = 0.33f; // 최대 점프 시간

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

        if (Input.GetButtonUp("Jump")) // 점프 버튼을 땠을 때
        {
            iCanJump = false;
            jumpTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!usingLadder) Movement(horizontalMove); // 사다리 사용 중이 아닐 때 좌우이동 가능

        if (iCanUsingLadder) UsingLadder(verticalMove); // '사다리 오브젝트'에 트리거 시 위아래 입력으로 사다리 사용 가능

        CheckMyFeet(); // 발 밑 검사

        // 사다리 상호작용 시, 난간 오브젝트 충돌 무시
        if (usingLadder)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Railing"), LayerMask.NameToLayer("Player"), true); // 두 레이어간 충돌 꺼짐
            playerRigidbody.gravityScale = 0f; // 사다리 이용 시 중력 스케일 0
        }
        else if (!usingLadder)
        {
            // 사다리 이용 중지할 때, 난간 오브젝트 충돌 여부는 발밑 체크로 제어한다.
            if (CheckFeet) Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Railing"), LayerMask.NameToLayer("Player"), false); // 두 레이어간 충돌 켜짐

            if (playerRigidbody.velocity.y < 0) // 추락하고 있을 경우
            {
                UpInJumping = false;
                playerRigidbody.gravityScale = 2.0f;
            }
            else if (playerRigidbody.velocity.y >= 0) // 높이 변화가 없거나 상승 중인 경우
            {
                playerRigidbody.gravityScale = 1.0f;
            }
        }
    }

    private void Movement(float moveVelocity) // 플레이어 이동과 점프
    {
        float velocityY;

        Vector3 velocity = new Vector3(moveVelocity, 0, 0);
        velocity *= speed;

        if (iCanJump && Input.GetButton("Jump")) // Jump 버튼 입력 시 Jump() 메서드 호출
            velocityY = JumpVelocityY();
        else
            velocityY = playerRigidbody.velocity.y;

        //if (velocityY < 0f) playerRigidbody.gravityScale = 2.0f; // 추락 중력 스케일 조정
        if (velocityY < -6.0f) velocityY = -6.0f; // 최대 추락 속도 -6.0f

        velocity.y = velocityY;

        playerRigidbody.velocity = velocity;
    }

    private float JumpVelocityY() // 플레이어 점프 Y 값
    {
        jumpTimer += Time.deltaTime; // 점프 타이머 증가

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

        //return Mathf.Sin(1.902f * jumpTimer) * jumpPower; // 삼각함수 Sin 포물선 처럼 점프함
    }

    private void UsingLadder(float moveVertical) // 사다리 이용
    {
        if (moveVertical != 0)
        {
            usingLadder = true;
            iCanJump = false; // 사다리 사용 중일때는 점프를 UsingLadder()에서 제어한다
        }
        else if (usingLadder) goto Ladder;
        else return;

        Ladder:
        // 플레이어는 위, 아래로만 이동할 수 있다.
        Vector3 velocity = new Vector3(0, moveVertical, 0);
        //velocity *= speed; // 사다리 올라가는 속도를 높여야 할까?

        // 사다리 이용 중, Jump 입력으로 사다리 이용을 중지할 수 있다.
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

    private void CheckMyFeet() // 플레이어 발밑에 땅이 있는지?
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
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground")) // Edge 콜라이더가 그라운드 오브젝트와 트리거 시
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Railing"), LayerMask.NameToLayer("Player"), false); // 두 레이어간 충돌 켜짐
            UpInJumping = false;
            iCanJump = true; // 점프 가능
        }
        
        // 플레이어가 사다리에 트리거 됐을 때, bool 변수로 사다리 상호작용 가능 여부를 체크한다.
        if(collision.CompareTag("Ladder")) // 사다리에 들어오면 true
        {
            iCanUsingLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Ladder")) // 사다리에서 벗어나면 false
        {
            iCanUsingLadder = false;
            usingLadder = false;
        }
    }
}