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
    [SerializeField] private float cameraFollowTime; // �߶��ϴ� �ð��� üũ

    [Header("Character When It Falls")]                 // �÷��̾
    [SerializeField] private float characterGoingUp;    // ��� ���϶� ī�޶� ����
    [SerializeField] private float characterFallingOff; // �߶� ���϶� ī�޶� ����

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

        if(targetMovement.CheckFeet) // �÷��̾� �� �ؿ� ��ü�� �ִٸ�,
        {
            // �÷��̾ ������ �� ��, ī�޶�� �÷��̾� X�ุ �����Ѵ�.
            // 1. �÷��̾�� �׽� �� �ؿ� ray�� ���.
            // 2. ray �� "Ground" �� ��´ٸ� ī�޶��� Y���� �÷��̾� ���� �ʴ´�.

            cameraFollowTime = 0; // �߶� ���� ���� �ð� 0����

            smoothPosition = Vector3.SmoothDamp(transform.position, targetMovement.MyFeetPosition, ref lastMovingVelocity, SmoothTime);
            transform.position = smoothPosition;
        }
        else if(!targetMovement.CheckFeet) // �÷��̾� �� �ؿ� ��ü�� ���ٸ�,
        {
            // �÷��̾ ������ �� ��, ���� ���� ���� ������ ���� �ִ�.
            // 1. �÷��̾� �� ������ �׽� ray�� ���. <ray�� ���̴� �÷��̾� ���� ����>
            // 2. ray�� ������Ʈ�� �ȴ�����, ī�޶� �÷��̾ �Ѿư���.

            // <�÷��̾ ���߿��� ������ ��, �� �ؿ� ���� ���� ���>
            // 1. ���߿��� �ö󰡰� �ִ� ���(rigidbody.velocity.y >= 0)�� <�÷��̾� ���� �ణ ���� ī�޶� ����>
            // 2. ���߿��� �߶��ϰ� �ִ� ���(rigidbody.velocity.y < 0)�� <�÷��̾� ���� ���� ī�޶� ����>

            cameraFollowTime += Time.deltaTime; // �߶� �� ���� �ð� üũ

            if (target.GetComponent<Rigidbody2D>().velocity.y >= 0) // ��� ��
            {
                SmoothTime = 0.2f;

                Vector3 cameraFollow = new Vector3(0, characterGoingUp, 0);
                smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition + cameraFollow, ref lastMovingVelocity, SmoothTime);

                transform.position = smoothPosition;
            }
            else if (target.GetComponent<Rigidbody2D>().velocity.y < 0 && // �߶� ��
                     cameraFollowTime <= 1.5f)
            {
                SmoothTime = 0.4f;

                Vector3 cameraFollow = new Vector3(0, characterFallingOff, 0);
                smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition + cameraFollow, ref lastMovingVelocity, SmoothTime);

                transform.position = smoothPosition;
            }
            else if (cameraFollowTime > 1.5f) // �������� �߶� ��
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
