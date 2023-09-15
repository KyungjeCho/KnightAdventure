using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ThirdPersonOrbitCam : MonoBehaviour
{
    public Transform playerTr; // �÷��̾� Ʈ������ 
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f); // �÷��̾� ĳ������ �Ǻ�. �÷��̾� ĳ������ �Ǻ��� ���� �ֱ� ������ 1f ��ŭ ���� ī�޶� �ٶ󺸰� �Ѵ�.
                                                                // ���� �ָ� ���� �� �� ȿ�� ���� ��쿡�� ����Ѵ�.
    public Vector3 camOffset = new Vector3(0.4f, 0.5f, -2.0f);  // ��ġ ������ ����. ī�޶�� �÷��̾��� �Ÿ�. ��� ���� ������ �ִ°�.

    public float smooth = 10f;                  // ī�޶� �����ӵ�. �̵��� �󸶳� �ε巴�� �� ���ΰ�.
    public float horizontalAimingSpeed = 6.0f;  // ���� ȸ�� ���ǵ�
    public float verticalAimingSpeed = 6.0f;    // ���� ȸ�� ���ǵ�
    public float maxVerticalAngle = 30.0f;      // �ִ� ���� ���� (�󸶳� ī�޶� ���� �� ������)
    public float minVerticalAngle = -60.0f;     // �ּ� ���� ���� (�󸶳� ī�޶� �Ʒ��� �� ������)

    public float recoilAngleBounce = 5.0f;      // �ݵ� ���� ��
    private float angleH = 0.0f;                // ���콺�� �������� �󸶳� ����������
    private float angleV = 0.0f;                // ���콺�� �������� �󸶳� ����������
    private Transform cameraTransform;          // ī�޶� Ʈ������ (ĳ��)
    private Camera myCamera;                    // ī�޶� (ĳ��)
    private Vector3 relCameraPos;               // ī�޶� ��ǥ�� �÷��̾���� ����� ��ġ
    private float relCameraPosMag;              // ī�޶� ��ǥ�� �÷��̾���� ����� ��ġ�� �Ÿ� (��Į��)
    private Vector3 smoothPivotOffset;          // ī�޶� ȭ���Ҷ� �� ���� ��ġ (�ε巴��)
    private Vector3 smoothCamOffset;            // 
    private Vector3 targetPivotOffset;          // 
    private Vector3 targetCamOffset;            // 
    private float defaultFOV;                   // �⺻ FOV ��
    private float targetFOV;                    // ���� FOV �� (�޸��� �Ҷ�)
    private float targetMaxVerticalAngle;       // 
    private float recoilAngle = 0f;             // �ݵ� ����

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
        relCameraPosMag = relCameraPos.magnitude - 0.5f; // �� 0.5f�� ����?

        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = myCamera.fieldOfView;
        angleH = playerTr.eulerAngles.y;

        ResetTargetOffsets();
        ResetFOV();
        ResetMaxVerticalAngle();
    }

    // Ÿ�� ������ ����
    // �׷��ٸ� Ÿ�� �������� �����ϱ�? 
    // ���콺�� �����̸� ī�޶�� �̵��� �ϰ� �ȴ�.
    // ������, ������ ĳ������ �Ǻ� (���� �Ӹ�)�� ��� ���Ѻ��� �ϸ�, �̵��� �ε巴�� �̵��ؾ� �Ѵ�.
    // Ÿ�� �������� ī�޶� �̵��� ������ ��ġ �̸�, �׻��̸� �����Ͽ� ī�޶� �ε巴�� �̵��Ѵ�.
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

    // ���� ��ġ üĿ
    // ���� ī�޶� �̵� �Ҷ� ī�޶�� �÷��̾��� ���̿� ���� �ִ��� üŷ
    bool ViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight)
    {
        Vector3 target = playerTr.position + (Vector3.up * deltaPlayerHeight);
        if (Physics.SphereCast(checkPos, 0.2f, target - checkPos, out RaycastHit hit, relCameraPosMag))
        {
            if (hit.transform != playerTr && !hit.transform.GetComponent<Collider>().isTrigger) // Cast ���� hit�� �÷��̾ �ƴϰ� �ݶ��̴��� Ʈ���Ű� �ƴ� ���, ������ �ݶ��̴��� ���
            {
                return false;
            }
        }
        return true;
    }

    // �ݴ�� ��ġ üĿ
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

    // �� �� �Լ��� �̿��Ͽ� ���� üŷ
    bool DoubleViewingPosCheck(Vector3 checkPos, float offset)
    {
        float playerFocusHeight = playerTr.GetComponent<CapsuleCollider>().height * 0.75f;
        return ViewingPosCheck(checkPos, playerFocusHeight) && ReverseViewingPosCheck(checkPos, playerFocusHeight, offset);
    }

    private void Update()
    {
        // ���콺 �̵� ��/
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f) * horizontalAimingSpeed;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1f, 1f) * verticalAimingSpeed;

        // ���� �̵� ����
        angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);
        

        Quaternion camYRotation = Quaternion.Euler(0.0f, angleH, 0.0f);
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0.0f);
        cameraTransform.rotation = aimRotation;
        
        // set FOV
        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, targetFOV, Time.deltaTime);
        Vector3 baseTempPosition = playerTr.position + camYRotation * targetPivotOffset;
        Vector3 noCollisionOffset = targetCamOffset; // �����Ҷ� ī�޶��� �����°� 

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
