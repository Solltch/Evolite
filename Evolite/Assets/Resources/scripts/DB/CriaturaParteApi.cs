using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CriatParteAPI : MonoBehaviour
{
    public string baseUrl = "http://SEU_SERVIDOR/criat_parte/";

    // ==========================
    // ADD
    // ==========================
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
            form.AddField("equipadas[]", 1); // sempre 1
        }

        using UnityWebRequest www = UnityWebRequest.Post(baseUrl + "save.php", form);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        callback(www.result == UnityWebRequest.Result.Success, www.downloadHandler.text);
    }



    // ==========================
    // REMOVE
    // ==========================
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
