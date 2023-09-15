using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ThirdPersonOrbitCam : MonoBehaviour
{
    public Transform playerTr; // 플레이어 트랜스폼 
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f); // 플레이어 캐릭터의 피봇. 플레이어 캐릭터의 피봇은 땅에 있기 때문에 1f 만큼 위로 카메라가 바라보게 한다.
                                                                // 만약 멀리 보는 줌 인 효과 같은 경우에도 사용한다.
    public Vector3 camOffset = new Vector3(0.4f, 0.5f, -2.0f);  // 위치 오프셋 벡터. 카메라와 플레이어의 거리. 어느 정도 떨어져 있는가.

    public float smooth = 10f;                  // 카메라 반응속도. 이동을 얼마나 부드럽게 할 것인가.
    public float horizontalAimingSpeed = 6.0f;  // 수평 회전 스피드
    public float verticalAimingSpeed = 6.0f;    // 수직 회전 스피드
    public float maxVerticalAngle = 30.0f;      // 최대 수직 각도 (얼마나 카메라가 위를 볼 것인지)
    public float minVerticalAngle = -60.0f;     // 최소 수직 각도 (얼마나 카메라가 아래를 볼 것인지)

    public float recoilAngleBounce = 5.0f;      // 반동 각도 힘
    private float angleH = 0.0f;                // 마우스가 수평으로 얼마나 움직였는지
    private float angleV = 0.0f;                // 마우스가 수직으로 얼마나 움직였는지
    private Transform cameraTransform;          // 카메라 트랜스폼 (캐싱)
    private Camera myCamera;                    // 카메라 (캐싱)
    private Vector3 relCameraPos;               // 카메라 좌표와 플레이어와의 상대적 위치
    private float relCameraPosMag;              // 카메라 좌표와 플레이어와의 상대적 위치의 거리 (스칼라)
    private Vector3 smoothPivotOffset;          // 카메라가 화전할때 각 보간 위치 (부드럽게)
    private Vector3 smoothCamOffset;            // 
    private Vector3 targetPivotOffset;          // 
    private Vector3 targetCamOffset;            // 
    private float defaultFOV;                   // 기본 FOV 값
    private float targetFOV;                    // 변할 FOV 값 (달리기 할때)
    private float targetMaxVerticalAngle;       // 
    private float recoilAngle = 0f;             // 반동 각도

    public float GetH
    {
        get => angleH;
    }

    private void Awake()
    {
        cameraTransform = transform;
        myCamera = cameraTransform.GetComponent<Camera>();

        cameraTransform.position = playerTr.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
        cameraTransform.rotation = Quaternion.identity;

        relCameraPos = cameraTransform.position - playerTr.position;
        relCameraPosMag = relCameraPos.magnitude - 0.5f; // 왜 0.5f를 했지?

        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = myCamera.fieldOfView;
        angleH = playerTr.eulerAngles.y;

        ResetTargetOffsets();
        ResetFOV();
        ResetMaxVerticalAngle();
    }

    // 타겟 오프셋 리셋
    // 그렇다면 타겟 오프셋은 무엇일까? 
    // 마우스를 움직이면 카메라는 이동을 하게 된다.
    // 하지만, 일정한 캐릭터의 피봇 (대충 머리)를 계속 지켜봐야 하며, 이동도 부드럽게 이동해야 한다.
    // 타겟 오프셋은 카메라가 이동할 마지막 위치 이며, 그사이를 보간하여 카메라를 부드럽게 이동한다.
    public void ResetTargetOffsets()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
    }

    public void ResetFOV()
    {
        this.targetFOV = defaultFOV;
    }
    public void ResetMaxVerticalAngle()
    {
        targetMaxVerticalAngle = maxVerticalAngle;
    }

    public void BounceVertical(float degree)
    {
        recoilAngle = degree;
    }

    public void SetTargetOffset(Vector3 newPivotOffset, Vector3 newCamOffset)
    {
        targetPivotOffset = newPivotOffset;
        targetCamOffset = newCamOffset;
    }

    public void SetFOV(float customFOV)
    {
        this.targetFOV = customFOV;
    }

    // 현재 위치 체커
    // 만약 카메라가 이동 할때 카메라와 플레이어의 사이에 벽이 있는지 체킹
    bool ViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight)
    {
        Vector3 target = playerTr.position + (Vector3.up * deltaPlayerHeight);
        if (Physics.SphereCast(checkPos, 0.2f, target - checkPos, out RaycastHit hit, relCameraPosMag))
        {
            if (hit.transform != playerTr && !hit.transform.GetComponent<Collider>().isTrigger) // Cast 맞은 hit가 플레이어가 아니고 콜라이더가 트리거가 아닐 경우, 무언가의 콜라이더의 경우
            {
                return false;
            }
        }
        return true;
    }

    // 반대로 위치 체커
    bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight, float maxDistance)
    {
        Vector3 origin = playerTr.position + (Vector3.up * deltaPlayerHeight);
        if (Physics.SphereCast(origin, 0.2f, checkPos - origin, out RaycastHit hit, maxDistance))
        {
            if (hit.transform != playerTr && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }

    // 위 두 함수를 이용하여 더블 체킹
    bool DoubleViewingPosCheck(Vector3 checkPos, float offset)
    {
        float playerFocusHeight = playerTr.GetComponent<CapsuleCollider>().height * 0.75f;
        return ViewingPosCheck(checkPos, playerFocusHeight) && ReverseViewingPosCheck(checkPos, playerFocusHeight, offset);
    }

    private void Update()
    {
        // 마우스 이동 값/
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f) * horizontalAimingSpeed;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1f, 1f) * verticalAimingSpeed;

        // 수직 이동 제한
        angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);
        

        Quaternion camYRotation = Quaternion.Euler(0.0f, angleH, 0.0f);
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0.0f);
        cameraTransform.rotation = aimRotation;
        
        // set FOV
        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, targetFOV, Time.deltaTime);
        Vector3 baseTempPosition = playerTr.position + camYRotation * targetPivotOffset;
        Vector3 noCollisionOffset = targetCamOffset; // 조준할때 카메라의 오프셋값 

        for (float zOffset = targetCamOffset.z; zOffset <= 0f; zOffset += 0.5f)
        {
            noCollisionOffset.z = zOffset;
            if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset,
                Mathf.Abs(zOffset)) || zOffset == 0f)
            {
                break;
            }
        }

        // Repostion Camera
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, noCollisionOffset, smooth * Time.deltaTime);

        cameraTransform.position = playerTr.position + camYRotation * smoothPivotOffset +
            aimRotation * smoothCamOffset;
    }

    public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
    {
        return Mathf.Abs((finalPivotOffset - smoothPivotOffset).magnitude);
    }
}
