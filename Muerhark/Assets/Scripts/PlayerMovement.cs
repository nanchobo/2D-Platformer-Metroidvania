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
    [SerializeField] public bool UpInJumping; // 점프하는 동안 올라가는가?
    [SerializeField] public bool CheckFeet;
    [SerializeField] private float jumpPower = 1f;
    [SerializeField] private float jumpTimer = 0f;
    private const float jumpTimeLimit = 0.33f; // 최대 점프 시간

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

        if (Input.GetButtonUp("Jump")) // 점프 버튼을 땠을 때
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
        CheckMyFeet(); // 발 밑 검사
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

        if (velocityY < 0f) playerRigidbody.gravityScale = 2.0f; // 추락 중력 스케일 조정
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

    public Vector2 JumpRay() // 보류 2021-11-02
    {
        Ray2D ray = new Ray2D();

        ray.origin = transform.position;
        ray.direction = Vector2.down;

        return ray.GetPoint(100f);
    }

    private void CheckMyFeet() // 플레이어 발 밑에 땅이 있는지?
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
            iCanJump = true; // 점프 가능
            playerRigidbody.gravityScale = 1;
        }
    }

    void TempMove() // 예전 플레이어 움직임
    {
        float fallSpeed = playerRigidbody.velocity.y;

        Vector3 velocity = new Vector3(horizontalMove, 0, 0);

        velocity *= speed;

        Debug.Log("Move 함수: " + fallSpeed);

        velocity.y = fallSpeed;
        /* velocity는 게임 오브젝트의
         * 속도를 바로 덮어씌운다.
         * 주의해야 할 점은,
         * 추락할 때의 속도이다.
         * 처음 추락할 때 속도를
         * 유지시켜 주는 것이 중요하다. */
        playerRigidbody.velocity = velocity;
    }
    void TempJump() // 예전 플레이어 점프
    {
        jumpTimer += Time.deltaTime; // 점프 타이머 증가

        if (jumpTimer >= jumpTimeLimit)
        {
            iCanJump = false; // 점프 불가
            jumpTimer = 0;
            //Debug.Log("점프 불가| iCanJump: " + iCanJump + "| jumpTimer: " + jumpTimer);
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
        /* ForceMode.Impulse 는 리지드 바디 컴포턴트 요소 중에
         * Mass(질량)의 영향을 받으며 짧은 순간에 힘을 가하는 모드이다. */
    }
}