using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class GPTUtils
{
    public static string ExtractContentFromGPTResponse(string json)
    {
        try
        {
            JObject parsed = JObject.Parse(json);
            var content = parsed["choices"]?[0]?["message"]?["content"]?.ToString();
            return content ?? "No content found";
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse GPT response: " + e.Message);
            return "Error";
        }
    }


    public static List<CommandFunctionPair> ParseCommandFunctionPairs(string content)
    {
        try
        {
            var match = Regex.Match(content, @"```(?:json)?\s*(.*?)\s*```", RegexOptions.Singleline);
            string extracted = match.Success ? match.Groups[1].Value : content;
            string unescaped = Regex.Unescape(extracted);

            if (unescaped.TrimStart().StartsWith("["))
            {
                return JsonConvert.DeserializeObject<List<CommandFunctionPair>>(unescaped);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("ParseCommandFunctionPairs 실패: " + ex.Message);
        }

        return new List<CommandFunctionPair>();
    }

    public static List<FunctionCall> ParseFinalFunctionCalls(string response)
    {
        try
        {
            var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(response);
            var message = chatResponse?.choices?[0]?.message;

            if (message == null)
                return new List<FunctionCall>();

            if (message.function_call != null)
            {
                var args = JsonConvert.DeserializeObject<Dictionary<string, object>>(message.function_call.arguments);
                return new List<FunctionCall> { new FunctionCall { name = message.function_call.name, arguments = args } };
            }

            if (!string.IsNullOrEmpty(message.content))
            {
                string cleaned = ExtractJsonBlock(message.content);

                if (cleaned.TrimStart().StartsWith("["))
                    return JsonConvert.DeserializeObject<List<FunctionCall>>(cleaned);
                else
                    return new List<FunctionCall> { JsonConvert.DeserializeObject<FunctionCall>(cleaned) };
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ParseFinalFunctionCalls 실패: " + e.Message);
        }

        return new List<FunctionCall>();
    }

    public static string ExtractJsonBlock(string content)
    {
        var match = Regex.Match(content, @"```json\s*(.*?)\s*```", RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value : content;
    }
}
