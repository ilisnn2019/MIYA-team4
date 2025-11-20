using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using Newtonsoft.Json;
using UnityEngine;

public interface IObjectLookUp
{
    string GetCameraInfo();
    string GetCaptureInfo();
}

public class ObjectsLookup : MonoBehaviour, IObjectLookUp
{
    private IRegistry registry;
    private List<EntityInfoAgent> capturedObject = new();
    public float viewDegree = 30;

    void Awake()
    {
        InterfaceContainer.Register<IObjectLookUp>("lookup", this);
    }
    [ContextMenu("check info captured")]
    public void CheckCaptureInfo()
    {
        Debug.Log(GetCaptureInfo());
    }

    // 카메라 시야 내 물체들에 대한 정보 + 카메라까지 거리
    public string GetCaptureInfo()
    {
        registry ??= InterfaceContainer.Resolve<IRegistry>("registry");

        Vector3 cameraPos = transform.position;
        Vector3 cameraForward = transform.forward;

        // 속성 이름 정의
        string[] attributes = new string[]
        {
        "id", "type", "position", "rotation", "factor",
        "color", "texture", "weight", "distance", "facingCosine"
        };

        List<List<object>> values = new List<List<object>>();

        foreach (var reference in registry.GetAllAgents())
        {
            GameObject furniture = reference.gameObject;
            Vector3 toFurniture = reference.transform.position - cameraPos;
            float dist = toFurniture.sqrMagnitude;
            float facingCos = Vector3.Dot(cameraForward, toFurniture.normalized);

            EntityInfo info = reference.Info; // SerializeToJson 대신 직접 접근

            List<object> row = new List<object>
        {
            info.id,
            info.type,
            new float[] { info.position.x, info.position.y, info.position.z },
            info.rotation,
            info.factor,
            info.color,
            info.texture,
            info.weight,
            dist,
            facingCos
        };

            values.Add(row);
        }

        var captureInfo = new Dictionary<string, object>
    {
        { "attributes", attributes },
        { "values", values }
    };

        // JSON 직렬화 (가독성 없는 compact 모드)
        string json = JsonConvert.SerializeObject(captureInfo, Formatting.None);

        return json;
    }


    [ContextMenu("test - camera info")]
    public string GetCameraInfo()
    {
        string str = $"Camera Position : {transform.position}, Camera forward direction (normalized) : {transform.forward}";
        Debug.Log(str);
        return str;
    }

    [Header("For Debug")]
    public bool useCameraFOV = true;
    private Camera targetCamera;

    /*
    void OnDrawGizmos()
    {
        if (targetCamera == null) targetCamera = transform.GetChild(0).GetComponent<Camera>();
        if (targetCamera == null) return;

        float fov = useCameraFOV ? targetCamera.fieldOfView : viewDegree;
        float aspect = targetCamera.aspect;

        Gizmos.color = new Color(1,0,0,1);

        // 기즈모 좌표를 카메라 로컬 기준으로 두고 그립니다.
        Matrix4x4 prev = Gizmos.matrix;
        Gizmos.matrix = targetCamera.transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, fov, 0.1f, 100f, aspect);
        Gizmos.matrix = prev;
    }
    */
}
