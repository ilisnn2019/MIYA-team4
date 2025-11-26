#if UNITY_EDITOR    
using UnityEditor;
using UnityEngine;

using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

public class PromptCacheTesterWindow : EditorWindow
{
    private string openAIKey = "";
    private string model = "gpt-4o";

    private string staticPrompt =
        "You are a context-aware language interpreter for a VR environment.\n" +
        "Follow rules strictly and output only JSON.";

    private string userInput = "큐브를 만들어줘";
    private string prevCmd = "공을 크게 해줘";

    private string resultLog = "";
    private Vector2 scroll;
    private bool isRunning;

    private Vector2 staticPromptScroll; // 스크롤 위치 저장

    [MenuItem("Tools/Prompt Cache Tester")]
    public static void ShowWindow()
    {
        GetWindow<PromptCacheTesterWindow>("Prompt Cache Tester");
    }

    private void OnGUI()
    {
        GUILayout.Label("[API Key]", EditorStyles.boldLabel);
        openAIKey = EditorGUILayout.TextField("API Key", openAIKey);

        GUILayout.Space(6);
        GUILayout.Label("[Model]", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(model);

        GUILayout.Space(6);
        GUILayout.Label("[Static Prompt] (캐싱 대상)", EditorStyles.boldLabel);

        if (GUILayout.Button("Load Prompt"))
        {
            string loadPrompt = Resources.Load<TextAsset>("prompt").text;
            staticPrompt = loadPrompt;
        }

        staticPromptScroll = EditorGUILayout.BeginScrollView(staticPromptScroll, GUILayout.Height(250)); // 높이 조정 가능
        staticPrompt = EditorGUILayout.TextArea(staticPrompt, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        GUILayout.Space(6);
        GUILayout.Label("[Dynamic Prompt Inputs]", EditorStyles.boldLabel);
        userInput = EditorGUILayout.TextField("User Command", userInput);
        prevCmd = EditorGUILayout.TextField("Previous Command", prevCmd);

        GUILayout.Space(10);

        EditorGUI.BeginDisabledGroup(isRunning);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Run Test"))  // 실제 테스트 실행
        {
            if (ValidateKey()) _ = RunPromptTestAsync(userInput, prevCmd);
        }

        if (GUILayout.Button("Check Tokens"))  // Dry-run: static prompt 토큰만 확인
        {
            if (ValidateKey()) _ = RunDryRunAsync();
        }
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(14);
        GUILayout.Label("[Result]", EditorStyles.boldLabel);

        // 결과 초기화 버튼 추가
        if (GUILayout.Button("Clear Result"))
        {
            resultLog = "";
        }

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(230));
        EditorGUILayout.TextArea(resultLog, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    private bool ValidateKey()
    {
        if (string.IsNullOrEmpty(openAIKey))
        {
            EditorUtility.DisplayDialog("Error", "API Key를 입력하세요.", "OK");
            return false;
        }
        return true;
    }

    private string BuildPayload(string userInput, string prevCmd)
    {
        var dynamicPrompt = $"User command: {userInput}\nPrevious command: {prevCmd}";
        var payload = new JObject
        {
            ["model"] = model,
            ["messages"] = new JArray
            {
                new JObject { ["role"] = "system", ["content"] = staticPrompt },
                new JObject { ["role"] = "user", ["content"] = dynamicPrompt }
            }
        };
        return payload.ToString();
    }

    private async Task RunPromptTestAsync(string userInput, string prevCmd)
    {
        isRunning = true;
        try
        {
            string url = "https://api.openai.com/v1/chat/completions";
            string payload = BuildPayload(userInput, prevCmd);

            using var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + openAIKey);

            var op = request.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                AppendLog($"[Error] {request.error}");
                return;
            }

            string json = request.downloadHandler.text;
            JObject res = JObject.Parse(json);

            int promptTokens = res["usage"]?["prompt_tokens"]?.Value<int>() ?? -1;
            int completionTokens = res["usage"]?["completion_tokens"]?.Value<int>() ?? -1;
            int totalTokens = res["usage"]?["total_tokens"]?.Value<int>() ?? -1;
            int cahcedTokens = res["usage"]?["prompt_tokens_details"]?["cached_tokens"]?.Value<int>() ?? -1;
            string answer = res["choices"]?[0]?["message"]?["content"]?.ToString();

            AppendLog(
                "[Run Test Completed]\n" +
                $"PromptTokens: {promptTokens}\n" +
                $"CompletionTokens: {completionTokens}\n" +
                $"TotalTokens: {totalTokens}\n" +
                $"CachedTokens: {cahcedTokens}\n" +
                "[Answer]\n" + answer + "\n"
            );
        }
        catch (System.Exception ex)
        {
            AppendLog("[Exception] " + ex.Message);
        }
        finally
        {
            isRunning = false;
        }
    }

    // Dry-run 실행: static prompt만 전달
    private async Task RunDryRunAsync()
    {
        isRunning = true;
        try
        {
            string url = "https://api.openai.com/v1/chat/completions";

            // User 메시지는 비워두고 system 메시지만 포함
            var payload = new JObject
            {
                ["model"] = model,
                ["messages"] = new JArray
                {
                    new JObject { ["role"] = "system", ["content"] = staticPrompt },
                    new JObject { ["role"] = "user", ["content"] = "" }
                },
                ["max_tokens"] = 1  // 응답은 최소화
            };

            using var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(payload.ToString());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + openAIKey);

            var op = request.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                AppendLog($"[Error] Dry-run 실패: {request.error}");
                return;
            }

            string json = request.downloadHandler.text;
            JObject res = JObject.Parse(json);

            int promptTokens = res["usage"]?["prompt_tokens"]?.Value<int>() ?? -1;

            AppendLog(
                "[Dry-Run Completed]\n" +
                $"Static Prompt Tokens: {promptTokens}\n" +
                "(이 값으로 캐싱 조건 충족 여부 확인 가능)"
            );
        }
        catch (System.Exception ex)
        {
            AppendLog("[Exception in Dry-Run] " + ex.Message);
        }
        finally
        {
            isRunning = false;
        }
    }

    private void AppendLog(string text)
    {
        resultLog = (resultLog.Length > 0 ? resultLog + "\n" : "") + text;
        Repaint();
    }
}
#endif