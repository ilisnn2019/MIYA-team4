using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Scenario : MonoBehaviour
{
    class Comparision
    {
        public string description;
        public Func<bool> check;

        public Comparision(string desc, Func<bool> func)
        {
            description = desc;
            check = func;
        }

        // 결과를 bool로 반환
        public bool Run()
        {
            bool result = check.Invoke();
            if (result)
            {
                Debug.Log($"[PASSED] {description}");
            }
            else
            {
                Debug.LogWarning($"[FAILED] {description}");
            }
            return result;
        }
    }

    class Plot
    {
        public string name;
        public List<Comparision> comparations;
        private Action onSuccess;

        public Plot(string name, List<Comparision> comparations, Action callback = null)
        {
            this.name = name;
            this.comparations = comparations;
            onSuccess = callback;
        }

        // 명령어 처리 후, 결과값 비교
        // 참이면 다음 callback
        // 거짓이면 무시
        public void Run()
        {
            Debug.Log($"==== Running TestCase: {name} ====");

            foreach (var cmp in comparations)
            {
                if (!cmp.Run())
                {
                    Debug.Log($"[STOP] {name} aborted due to failure.");
                    return;
                }
            }

            onSuccess?.Invoke();
        }
    }


    List<Plot> cases = new();
    public Action OnPlotSuccess;

    Func<EntityInfoAgent> getCreateTarget = () =>
    {
        var ids = InterfaceContainer.Resolve<IStorage>("storage").GetCreateds();
        return InterfaceContainer.Resolve<IRegistry>("registry").Resolve(ids[0]);
    };

    Func<EntityInfoAgent> getUpdateTarget = () =>
    {
        var ids = InterfaceContainer.Resolve<IStorage>("storage").GetUpdateds();
        return InterfaceContainer.Resolve<IRegistry>("registry").Resolve(ids[0]);
    };

    Scenario()
    {
        cases.Add(
            new Plot("Example 1",
            new()
            {
                new Comparision("Check object was created properly.",
                func: () => PositionEquals(getCreateTarget(), new Vector3(10,10,10)))
            }, OnPlotSuccess));
        cases.Add(
            new Plot("Example 2",
            new()
            {
                new Comparision("Check object was updated properly.",
                func : () => true)
            }, OnPlotSuccess)
        );
    }

    // 1. 위치 비교
    public bool PositionEquals(EntityInfoAgent target, Vector3 expected, float tolerance = 0.001f)
    {
        return Vector3.Distance(target.transform.position, expected) <= tolerance;
    }

    // 2. 각도(회전) 비교
    public bool RotationEquals(EntityInfoAgent target, Quaternion expected, float tolerance = 0.001f)
    {
        return Quaternion.Angle(target.transform.rotation, expected) <= tolerance;
    }

    // 3. 크기(Scale) 비교
    public bool ScaleEquals(EntityInfoAgent target, Vector3 expected, float tolerance = 0.001f)
    {
        return Vector3.Distance(target.transform.localScale, expected) <= tolerance;
    }

    // 4. 색상 비교
    public bool ColorEquals(EntityInfoAgent target, string expectedColor)
    {
        return string.Equals(target.Info.color, expectedColor, System.StringComparison.OrdinalIgnoreCase);
    }

    // 5. 텍스쳐 비교
    public bool TextureEquals(EntityInfoAgent target, string expectedTexture)
    {
        return string.Equals(target.Info.texture, expectedTexture, System.StringComparison.OrdinalIgnoreCase);
    }

    // 6. 무게 비교
    public bool WeightEquals(EntityInfoAgent target, float expected, float tolerance = 0.001f)
    {
        return Mathf.Abs(target.Info.weight - expected) <= tolerance;
    }
}
