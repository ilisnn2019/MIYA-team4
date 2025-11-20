using UnityEngine;
using System.Text;

public static class RandomStringUtil
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    // 길이 10 고정 함수
    public static string Generate10()
    {
        return Generate(10);
    }

    public static string Generate(int length)
    {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            // Range: min inclusive, max exclusive
            int idx = UnityEngine.Random.Range(0, chars.Length);
            sb.Append(chars[idx]);
        }
        return sb.ToString();
    }
}
