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
    [SerializeField] private const float jumpTimeLimit = 0.33f; // 최대 점프 시간
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

        if (Input.GetButtonUp("Jump")) // 점프 버튼을 땠을 때
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

    private void Movement(float moveVelocity) // 플레이어 이동과 점프
    {
        float velocityY;

        Vector3 velocity = new Vector3(moveVelocity, 0, 0);
        velocity *= speed;

        if (iCanJump && Input.GetButton("Jump")) // Jump 버튼 입력 시 Jump() 메서드 호출
            velocityY = JumpVelocity();
        else
            velocityY = playerRigidbody.velocity.y;

        if (velocityY < 0f) playerRigidbody.gravityScale = 2.0f; // 추락 중력 스케일 조정
        if (velocityY < -6.0f) velocityY = -6.0f; // 최대 추락 속도 -6.0f

        velocity.y = velocityY;

        playerRigidbody.velocity = velocity;
    }

    private float JumpVelocity() // 플레이어 점프 Y 값
    {
        jumpTimer += Time.deltaTime; // 점프 타이머 증가

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

        //return Mathf.Sin(1.902f * jumpTimer) * jumpPower; // 삼각함수 Sin 포물선 처럼 점프함
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //collision.IsTouchingLayers(LayerMask.NameToLayer("Player"));
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
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