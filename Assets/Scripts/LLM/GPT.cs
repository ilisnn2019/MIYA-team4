using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NAudio.CoreAudioApi;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

#region Data Models

[Serializable]
public class FunctionCall
{
    public string name;
    public Dictionary<string, object> arguments;
}

[Serializable]
public class Message
{
    public string role;
    public string content;
    public FunctionCallRaw function_call;
}

[Serializable]
public class FunctionCallRaw
{
    public string name;
    public string arguments; // JSON String
}

[Serializable]
public class FunctionDefinition
{
    public string name;
    public string description;
    public Dictionary<string, object> parameters;
}

[Serializable]
public class ChatRequest<T>
{
    public string model = "gpt-4o";
    public List<Message> messages;
    public List<T> functions;
}

[Serializable]
public class ChatResponse
{
    public List<Choice> choices;

    [Serializable]
    public class Choice
    {
        public int index;
        public Message message;
        public string finish_reason;
    }
}

[Serializable]
public class CommandFunctionPair
{
    public string command;
    public string name;

    public override string ToString()
    {
        return $"command : {command}\nname : {name}";
    }
}

#endregion

public interface IOpenAI
{
    IEnumerator SendMessage(string userMessage, Action<List<FunctionCall>> onComplete, bool isTest = false);
}

public class GPT : IOpenAI
{
    private readonly string apiKey;
    private const string LogTag = "GPT";
    //�ӽ��߰�###########
    private LookObjectSelector lookObjectSelector;
    //�ӽ��߰�###########

    private readonly IStorage storage;
    private readonly IObjectLookUp f_lookup;

    class CachPass : ChatPassExecutor<List<FunctionCall>>
    {
        private readonly IObjectLookUp f_lookup;
        private TextAsset loadPrompt;

        public CachPass(string apiKey, string pname = "prompt") : base(apiKey)
        {
            loadPrompt = Resources.Load<TextAsset>(pname);
        }

        public override IEnumerator Execute(string userInput, Action<List<FunctionCall>> onComplete)
        {
            string staticPrompt = loadPrompt.text;

            string payload = CreateFunctionCallPayload(userInput, staticPrompt);

            using var request = CreateRequest(payload);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("FunctionCallGenerationPass 실패: " + request.error);
                onComplete(new List<FunctionCall>());
                yield break;
            }

            Debug.Log(request.downloadHandler.text);

            var calls = GPTUtils.ParseFinalFunctionCalls(request.downloadHandler.text);
            onComplete(calls);
        }

        private string CreateFunctionCallPayload(string dynamicPrompt, string staticprompt)
        {
            var chatRequest = new ChatRequest<FunctionDefinition>
            {
                messages = new List<Message>
            {
                new Message { role = "system", content = staticprompt },
                new Message { role = "user", content = dynamicPrompt }
            },
            };

            return JsonConvert.SerializeObject(chatRequest);
        }
    }

    private readonly CachPass pass;

    public GPT(AccessKeySO key, string prompt_name = "prompt")
    {
        this.apiKey = key.accessKey;

        //contextPass = new(apiKey);
        //clarificationPass = new(apiKey);

        pass = new CachPass(apiKey, prompt_name);

        //임시추가###########
        /*
        this.lookObjectSelector = Camera.main.GetComponent<LookObjectSelector>();
        if (lookObjectSelector == null)
        {
            Debug.LogError("LookObjectSelector를 찾을 수 없습니다!");
        }
        */
        //임시추가###########

        storage = InterfaceContainer.Resolve<IStorage>("storage");
        f_lookup = InterfaceContainer.Resolve<IObjectLookUp>("lookup");

        //storage.PrevCmd = "그 원기둥의 크기를 2배 키우고, 색상은 초록, 오른쪽으로 30도 회전시켜줘";


    }

    public IEnumerator SendMessage(string userMessage, Action<List<FunctionCall>> onComplete, bool isTest = false)
    {
        // const only one pass
        // consider inserting a pass for pre-processing before main pass.

        string userInput = $@"
            # User's natural language command in Korean. #
            {userMessage}
            # Player Information #
            {f_lookup.GetCameraInfo()}
            # Context Information #
            Objects : {f_lookup.GetCaptureInfo()}
            # Previous user command #
            {storage.GetChanged()}
            ";

        List<FunctionCall> calls = null;

        TimerLogger.StartTimer();
        yield return pass.Execute(userInput, result =>
        {
            calls = result as List<FunctionCall>;
        });
        TimerLogger.EndTimer("gpt processing");
        onComplete?.Invoke(calls ?? new List<FunctionCall>());


        yield return null;
    }
}
