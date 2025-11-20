using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("�̵� ����")]
    [SerializeField] float moveSpeed = 5f;              // �⺻ �̵� �ӵ�
    [SerializeField] float gravity = -9.81f;            // �߷� ���ӵ�

    [Header("���콺 ����")]
    [SerializeField] float lookSensitivity = 2f;        // ���콺 ����
    [SerializeField] float maxLookAngle = 90f;          // ���� ���� ����

    CharacterController cc;
    Transform cam;                                       // ī�޶� Ʈ������
    float verticalVelocity;
    float pitch = 0f;                                    // ���� ī�޶� ȸ�� ��

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>().transform;
        // ���콺 Ŀ�� ����/����
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    // ���콺 �����ӿ� ���� �ü� ó��
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // �¿� ȸ�� (�÷��̾� ��ü Y��)
        transform.Rotate(Vector3.up * mouseX);

        // ���� ȸ�� (ī�޶� ��ġ)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);
        cam.localEulerAngles = Vector3.right * pitch;
    }

    // Ű���� �Է¿� ���� �̵� �� �߷� ó��
    void HandleMovement()
    {
        // ���� �̵� �Է�
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        move *= moveSpeed;

        // �߷� ����
        if (cc.isGrounded)
        {
            verticalVelocity = -1f;  // �ٴڿ� �پ� �ְ� �ణ�� ����
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        move.y = verticalVelocity;

        // ���� �̵�
        cc.Move(move * Time.deltaTime);
    }
}
