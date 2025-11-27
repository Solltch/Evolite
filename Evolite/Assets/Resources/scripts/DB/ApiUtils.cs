using System;
using System.Collections.Generic;

public static class ApiUtils
{
    // Verifica se resposta começa com ERR; e retorna mensagem (ou null se OK-like)
    public static string CheckErrorLine(string text)
    {
        if (string.IsNullOrEmpty(text)) return "Empty response";
        string trimmed = text.Trim();
        if (trimmed.StartsWith("ERR;")) return trimmed.Substring(4);
        return null;
    }

    // Parse "OK\nk=v\nk2=v2" -> dictionary
    public static Dictionary<string, string> ParseOkKeyValue(string text)
    {
        var dict = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(text)) return dict;
        string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0) return dict;
        // first line expected OK
        for (int i = 1; i < lines.Length; i++)
        {
            var idx = lines[i].IndexOf('=');
            if (idx <= 0) continue;
            string k = lines[i].Substring(0, idx).Trim();
            string v = lines[i].Substring(idx + 1).Trim();
            dict[k] = v;
        }
        return dict;
    }

    // Split list response with lines; each line fields split by ';'
    public static string[] SplitLines(string text)
    {
        return text.Replace("\r\n", "\n")
               .Replace('\r', '\n')
               .Split('\n');
    }
}