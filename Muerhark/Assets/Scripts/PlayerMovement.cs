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
    [SerializeField] private float jumpTimeLimit = 0.33f; // 최대 점프 시간
    [SerializeField] private float jumpTimer = 0f;

    [Header("Components")]
    [SerializeField] private Rigidbody2D playerRigidbody;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        this.horizontalMove = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonUp("Jump")) // 점프 버튼을 땠을 때
        {
            iCanJump = false;
            jumpTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        Movement(this.horizontalMove);
    }

    private void Movement(float moveVelocity) // 플레이어 이동과 점프
    {
        float velocityY;

        Vector3 velocity = new Vector3(moveVelocity, 0, 0);
        velocity *= speed;

        if (iCanJump && Input.GetButton("Jump")) // Jump 버튼 입력 시 Jump() 메서드 호출
        {
            velocityY = Jump();
        }
        else
        {
            velocityY = playerRigidbody.velocity.y;
        }

        if (velocityY < 0) playerRigidbody.gravityScale = 2;

        if (velocityY < -9.81f) velocityY = -9.81f;

        velocity.y = velocityY;

        playerRigidbody.velocity = velocity;
    }

    private float Jump() // 플레이어 점프
    {
        jumpTimer += Time.deltaTime; // 점프 타이머 증가

        if (jumpTimer >= jumpTimeLimit) // Jump 버튼을 jumpTimeLimit 보다 오래 누르고 있을 시,
        {
            iCanJump = false; // 점프 불가
            jumpTimer = 0;
            Debug.Log("jump Time Limit!!");
            return playerRigidbody.velocity.y;
        }

        return Mathf.Sin(1.902f * jumpTimer) * jumpPower; // 삼각함수 Sin 포물선 처럼 점프함
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        iCanJump = true; // 점프 가능
        playerRigidbody.gravityScale = 1;
        //isJumping = true;
    }

    void TempMove() // 플레이어 움직임
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
    void TempJump() // 플레이어 점프
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