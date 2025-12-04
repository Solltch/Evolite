using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CriatParteAPI : MonoBehaviour
{
    public string baseUrl = "http://SEU_SERVIDOR/criat_parte/";

    public IEnumerator AddPartes(int idCriatura, List<int> idPartes, List<int> tiposPartes, List<int> equipadas, Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id_criatura", idCriatura);

        foreach (int v in idPartes) form.AddField("id_partes[]", v);
        foreach (int v in tiposPartes) form.AddField("tipos_partes[]", v);
        foreach (int v in equipadas) form.AddField("equipadas[]", v);

        using UnityWebRequest www = UnityWebRequest.Post(baseUrl + "add.php", form);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            callback("Erro de conexão: " + www.error);
        }
        else
        {
            // Retorna o texto bruto do PHP, ex: "RESULTADO: 5 partes salvas, 1 falha"
            callback(www.downloadHandler.text);
        }
    }


    public IEnumerator SaveAll(int idCriatura, List<int> idPartes, List<int> tiposPartes, Action<bool, string> callback)
    {
        if (idPartes.Count != 6 || tiposPartes.Count != 6)
        {
            callback(false, "Precisam ser exatamente 6 partes");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("id_criatura", idCriatura);

        for (int i = 0; i < 6; i++)
        {
            form.AddField("id_partes[]", idPartes[i]);
            form.AddField("tipos_partes[]", tiposPartes[i]);
            form.AddField("equipadas[]", 1); // <--- Hardcoded aqui
        }

        using UnityWebRequest www = UnityWebRequest.Post(baseUrl + "save.php", form); // <--- Aponta para save.php
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        callback(www.result == UnityWebRequest.Result.Success, www.downloadHandler.text);
    }

    public IEnumerator GetByCriatura(int idCriatura, Action<List<CriaturaParte>, string> callback)
    {
        // Constrói a URL para a requisição GET
        string url = baseUrl + $"read.php?id_criatura={idCriatura}";

        using UnityWebRequest www = UnityWebRequest.Get(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            callback(null, "Erro de conexão: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;

            // Verifica se o resultado é uma resposta de erro (não JSON)
            if (json.StartsWith("ERRO"))
            {
                callback(null, json);
                yield break;
            }

            try
            {
                // O Unity precisa de um 'wrapper' para desserializar um array de JSON
                string jsonWrapper = "{\"partes\":" + json + "}";
                PartesWrapper wrapper = JsonUtility.FromJson<PartesWrapper>(jsonWrapper);

                if (wrapper.partes == null)
                {
                    callback(new List<CriaturaParte>(), "Nenhuma parte encontrada ou JSON vazio.");
                    yield break;
                }

                callback(wrapper.partes, null);
            }
            catch (Exception e)
            {
                callback(null, "Erro ao processar JSON: " + e.Message + "\nJSON Recebido: " + json);
            }
        }
    }

    public class PartesWrapper
    {
        public List<CriaturaParte> partes;
    }

    public IEnumerator Remove(int idCriatura, int tipoParte, Action<bool, string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id_criatura", idCriatura);
        form.AddField("tipo_parte", tipoParte);

        using UnityWebRequest www = UnityWebRequest.Post(baseUrl + "remove.php", form);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        callback(www.result == UnityWebRequest.Result.Success, www.downloadHandler.text);
    }
}
