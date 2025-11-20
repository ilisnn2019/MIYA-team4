// 파일명: SkyboxShaderController.cs
// 위치: Assets/... (Editor 폴더 아님)
using UnityEngine;

[ExecuteAlways] // 에디트 모드에서도 동작
[DisallowMultipleComponent]
public class SkyboxShaderController : MonoBehaviour
{
    [Header("Target Skybox Material (If null will use RenderSettings.skybox)")]
    public Material targetSkyboxMaterial; // null이면 RenderSettings.skybox에 적용

    
    [Header("Time")]
    // 0 => midnight
    // 360 => sunrise
    // 720 => noon
    // 1080 => sunset
    // 1440 => midnight
    [Range(0f, 1440f)] public float minute;

    [Header("Sun / Highlight Settings")]
    public Light Sun;
    //public Texture2D sunTexture;
    [Range(0f, 10f)] public float sunIntensity = 1f;
    public Gradient lightColor;
    //[Range(0f, 1f)] public float sunRadius = 0.02f; // smoothstep용 radius
    //public Color sunColor;
    [Range(-90f, 90f)] public float ecliptic = 23;

    [Header("Sky Color / Gradient Settings")]
    [Range(0.01f, 10f)]public float zenith;
    [Range(0.01f, 100f)] public float nadir;
    [Range(1f, 20f)] public float horizoneBlend;

    public Color groundColor;

    public Gradient skyColor;
    public Gradient horizoneColor;
    public Gradient sunsetColor;
    public Gradient sunriseColor;

    public AnimationCurve starRatio;

    [Header("Options")]
    public bool autoUseMainDirectionalLight = true; // RenderSettings.sun 사용 여부
    public Vector3 customSunDirection = new Vector3(0, 1, 0); // auto 사용 안할때

    // 쉐이더의 프로퍼티 이름들(사용하는 쉐이더에 맞게 변경)

    const string PROP_ZENITH_PARAM = "_ZenithParam";
    const string PROP_NADRI_PARAM = "_NadirParam";
    const string PROP_HORIZONE_BLEND_PARAM = "_HorizoneBlendParam";
    const string PROP_SKY_COLOR = "_SkyColor";
    const string PROP_GROUND_COLOR = "_GroundColor";
    const string PROP_HORIZONE_COLOR = "_HorizoneColor";
    const string PROP_SUNSET_COLOR = "_SunsetColor";
    const string PROP_SUNRISE_COLOR = "_SunriseColor";

    const string PROP_SUN_INTENSITY = "_SunIntensity";

    const string PROP_STAR_RATIO = "_StarRatio";

    Material SkyboxMaterial
    {
        get
        {
            if (targetSkyboxMaterial != null) return targetSkyboxMaterial;
            return RenderSettings.skybox;
        }
    }

    void OnEnable()
    {
        ApplyToMaterial(); // 활성/비활성시 즉시 적용
    }

    void OnDisable()
    {
        // 특별히 되돌리는 동작이 필요하면 여기서 처리
    }

    void Update()
    {
        // 에디트모드에서는 너무 자주 호출하지 않도록 조건부로 호출해도 좋지만
        // 간단함을 위해 매 프레임 동기화합니다. (원하면 편집시에만 OnValidate로 제한)
        if (!Application.isPlaying)
        {
            ApplyToMaterial();
        }
    }

    // 인스펙터에서 값 바꿀 때 즉시 반영되도록
    void OnValidate()
    {
        ApplyToMaterial();
    }

    // 실제로 머테리얼에 값을 씁니다.
    public void ApplyToMaterial()
    {
        var mat = SkyboxMaterial;
        if (mat == null) return;

        float timeEvaluate = minute / 1440;

        /*
        // 방향 계산: Main directional light(Sun) 사용 또는 커스텀
        Vector3 sunDir = customSunDirection.normalized;
        if (autoUseMainDirectionalLight && RenderSettings.sun != null)
        {
            // RenderSettings.sun.transform.forward는 라이트가 향하는 방향.
            // 스카이박스 셰이더에선 보통 빛이 오는 방향(또는 반대 방향)을 사용하므로 셰이더에 맞게 조정하세요.
            sunDir = -RenderSettings.sun.transform.forward.normalized; // 예: 라이트가 향하는 반대방향을 '해 위치'로 쓸 때
        }

        // 안전하게 프로퍼티 설정 (프로퍼티가 없으면 무시됨)
        if (sunTexture != null && mat.HasProperty(PROP_SUN_TEX))
            mat.SetTexture(PROP_SUN_TEX, sunTexture);

        if (mat.HasProperty(PROP_SUN_COLOR))
            mat.SetColor(PROP_SUN_COLOR, sunColor);

        if (mat.HasProperty(PROP_SUN_INTENSITY))
            mat.SetFloat(PROP_SUN_INTENSITY, sunIntensity);

        if (mat.HasProperty(PROP_SUN_RADIUS))
            mat.SetFloat(PROP_SUN_RADIUS, sunRadius);

        if (mat.HasProperty(PROP_SUN_DIR))
            mat.SetVector(PROP_SUN_DIR, new Vector4(sunDir.x, sunDir.y, sunDir.z, 0f));
        */

        float timeAngle = (minute / 4 + 270) % 360;
        Quaternion a = Quaternion.Euler(timeAngle, 0, 0);
        Quaternion b = Quaternion.Euler(0, 90, ecliptic);
        Sun.transform.rotation = b * a;

        Sun.color = lightColor.Evaluate(timeEvaluate);

        TrySetFloat(mat, PROP_SUN_INTENSITY, sunIntensity);

        TrySetFloat(mat, PROP_ZENITH_PARAM, zenith);
        TrySetFloat(mat, PROP_NADRI_PARAM, nadir);
        TrySetFloat(mat, PROP_HORIZONE_BLEND_PARAM, horizoneBlend * 10);

        TrySetColor(mat, PROP_SKY_COLOR, skyColor, timeEvaluate);
        TrySetColor(mat, PROP_GROUND_COLOR, groundColor);
        TrySetColor(mat, PROP_HORIZONE_COLOR, horizoneColor, timeEvaluate);
        TrySetColor(mat, PROP_SUNSET_COLOR, sunsetColor, timeEvaluate);
        TrySetColor(mat, PROP_SUNRISE_COLOR, sunriseColor, timeEvaluate);

        TrySetFloat(mat, PROP_STAR_RATIO, starRatio.Evaluate(timeEvaluate));

#if UNITY_EDITOR
        // 에디터에서는 변경을 저장/반영하도록 표시
        UnityEditor.EditorUtility.SetDirty(mat);
#endif
    }

    public Color GetCurrentSkyColor()
    {
        return skyColor.Evaluate(minute);
    }

    // 개발 편의: 인스펙터 버튼으로 강제 적용 가능하게 public 함수 노출
    public void ApplyNowFromEditor()
    {
        ApplyToMaterial();
    }

    private void TrySetFloat(Material mat, string propertyId, float value)
    {
        if (mat.HasProperty(propertyId))
            mat.SetFloat(propertyId, value);
    }

    private void TrySetColor(Material mat, string propertyId, Color value)
    {
        if (mat.HasProperty(propertyId))
            mat.SetColor(propertyId, value);
    }

    private void TrySetColor(Material mat, string propertyId, Gradient value, float eval)
    {
        TrySetColor(mat, propertyId, value.Evaluate(eval));
    }
}
