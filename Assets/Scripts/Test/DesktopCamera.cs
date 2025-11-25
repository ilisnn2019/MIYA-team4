using UnityEngine;

// 이동 없이 카메라 회전만 담당하는 스크립트
public class DesktopCameraController : MonoBehaviour
{
    [Header("마우스 설정")]
    [Tooltip("마우스 회전 민감도")]
    [SerializeField] float lookSensitivity = 2f;

    [Tooltip("위아래 회전 제한 각도 (90도면 수직까지 가능)")]
    [SerializeField] float maxLookAngle = 90f;

    // 내부 회전 값 저장 변수
    private float yaw = 0f;   // Y축 (좌우)
    private float pitch = 0f; // X축 (상하)

    void Awake()
    {
        // 시작 시 현재 카메라의 각도를 가져와서 초기값 설정 (화면 튐 방지)
        Vector3 currentAngles = transform.localEulerAngles;
        yaw = currentAngles.y;
        pitch = (currentAngles.x > 180) ? currentAngles.x - 360 : currentAngles.x; // 0~360도를 -180~180도로 보정

        // 마우스 커서 숨김 및 고정 (필요 시 주석 해제)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
    }

    // 마우스 입력에 따른 카메라 회전 처리
    void HandleMouseLook()
    {
        // 마우스 입력 감지 (프레임 간 이동 거리)
        // 유니티 Input 설정에서 "Mouse X", "Mouse Y" 축이 정의되어 있어야 함
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // 좌우 회전 (Yaw) - Y축 기준 누적
        yaw += mouseX;

        // 상하 회전 (Pitch) - X축 기준 누적 (마우스를 올리면 시선이 올라가야 하므로 뺌)
        pitch -= mouseY;

        // 상하 회전 각도 제한 (목 꺾임 방지)
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        // 회전 적용 (쿼터니언 변환 대신 오일러 각을 사용하여 직관적으로 적용)
        // Z축은 0으로 고정하여 화면이 옆으로 기울어지는 것(Roll) 방지
        transform.localEulerAngles = new Vector3(pitch, yaw, 0f);
    }
}