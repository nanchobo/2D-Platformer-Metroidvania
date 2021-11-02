using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Move Time")]
    [SerializeField] private float SmoothTime = 0.2f;
    [SerializeField] private float lastZoomSpeed;
    [SerializeField] private float targetZoomSize = 3.0f;

    private Vector3 lastMovingVelocity;
    private Vector3 targetPosition;
    [SerializeField] private float cameraFollowTime; // 추락하는 시간을 체크

    [Header("Character When It Falls")]                 // 플레이어가
    [SerializeField] private float characterGoingUp;    // 상승 중일때 카메라 구도
    [SerializeField] private float characterFallingOff; // 추락 중일때 카메라 구도

    private const float roundReadyZoomSize = 5.5f;
    private const float readyShotZoomSize = 5.0f;
    private const float trackingZoomSize = 3.0f;

    [Header ("Component")]
    [SerializeField] private GameObject target;
    [SerializeField] private PlayerMovement targetMovement;
    [SerializeField] private Camera camera;

    public enum State
    {
        Idle, Ready, Tracking
    }

    private State state
    {
        set
        {
            switch (value)
            {
                case State.Idle:
                    targetZoomSize = roundReadyZoomSize;
                    break;

                case State.Ready:
                    targetZoomSize = readyShotZoomSize;
                    break;

                case State.Tracking:
                    targetZoomSize = trackingZoomSize;
                    break;
            }
        }
    }

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        state = State.Idle;
        target = GameObject.Find("Muerhark");
        targetMovement = target.GetComponent<PlayerMovement>();
    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Move();
            Zoom();
        }
    }

    private void Move()
    {
        targetPosition = target.transform.position;

        Vector3 smoothPosition;

        if(targetMovement.CheckFeet) // 플레이어 발 밑에 물체가 있다면,
        {
            // 플레이어가 점프를 할 때, 카메라는 플레이어 X축만 추적한다.
            // 1. 플레이어는 항시 발 밑에 ray를 쏜다.
            // 2. ray 가 "Ground" 에 닿는다면 카메라의 Y축은 플레이어 쫓지 않는다.

            cameraFollowTime = 0; // 추락 안할 때는 시간 0으로

            smoothPosition = Vector3.SmoothDamp(transform.position, targetMovement.MyFeetPosition, ref lastMovingVelocity, SmoothTime);
            transform.position = smoothPosition;
        }
        else if(!targetMovement.CheckFeet) // 플레이어 발 밑에 물체가 없다면,
        {
            // 플레이어가 점프를 할 때, 땅이 없는 곳을 지나갈 수도 있다.
            // 1. 플레이어 발 밑으로 항시 ray를 쏜다. <ray의 길이는 플레이어 점프 높이>
            // 2. ray에 오브젝트가 안닿으면, 카메라가 플레이어를 쫓아간다.

            // <플레이어가 공중에서 착지할 때, 발 밑에 땅이 없는 경우>
            // 1. 공중에서 올라가고 있는 경우(rigidbody.velocity.y >= 0)는 <플레이어 기준 약간 높은 카메라 구도>
            // 2. 공중에서 추락하고 있는 경우(rigidbody.velocity.y < 0)는 <플레이어 기준 낮은 카메라 구도>

            cameraFollowTime += Time.deltaTime; // 추락 할 때는 시간 체크

            if (target.GetComponent<Rigidbody2D>().velocity.y >= 0) // 상승 중
            {
                SmoothTime = 0.2f;

                Vector3 cameraFollow = new Vector3(0, characterGoingUp, 0);
                smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition + cameraFollow, ref lastMovingVelocity, SmoothTime);

                transform.position = smoothPosition;
            }
            else if (target.GetComponent<Rigidbody2D>().velocity.y < 0 && // 추락 중
                     cameraFollowTime <= 1.5f)
            {
                SmoothTime = 0.4f;

                Vector3 cameraFollow = new Vector3(0, characterFallingOff, 0);
                smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition + cameraFollow, ref lastMovingVelocity, SmoothTime);

                transform.position = smoothPosition;
            }
            else if (cameraFollowTime > 1.5f) // 오랫동안 추락 중
            {
                SmoothTime = 0.4f;

                Vector3 cameraFollow = new Vector3(0, -5.5f, 0);
                smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition + cameraFollow, ref lastMovingVelocity, SmoothTime);

                transform.position = smoothPosition;
            }
        }

        state = State.Tracking;
    }

    private void Zoom()
    {
        float smoothZoomSize = Mathf.SmoothDamp(camera.orthographicSize,
                                targetZoomSize, ref lastZoomSpeed, SmoothTime);

        camera.orthographicSize = smoothZoomSize;
    }
}
