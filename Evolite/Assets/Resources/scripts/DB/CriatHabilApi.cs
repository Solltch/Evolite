// Assets/Scripts/Api/CriatHabilAPI.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CriatHabilAPI : MonoBehaviour
{
    private string baseUrl = ApiConfig.BASE_URL + "criat_habil/";

    public IEnumerator Add(int idCriatura, int idHabilidade, Action<bool, string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id_criatura", idCriatura);
        form.AddField("id_habilidade", idHabilidade);
        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "add.php", form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) { callback(false, www.error); yield break; }
            string err = ApiUtils.CheckErrorLine(www.downloadHandler.text);
            if (err != null) callback(false, err); else callback(true, "OK");
        }
    }

    public IEnumerator Remove(int idCriatura, int idHabilidade, Action<bool, string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id_criatura", idCriatura);
        form.AddField("id_habilidade", idHabilidade);
        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "remove.php", form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) { callback(false, www.error); yield break; }
            string err = ApiUtils.CheckErrorLine(www.downloadHandler.text);
            if (err != null) callback(false, err); else callback(true, "OK");
        }
    }

    public IEnumerator ListByCriatura(int idCriatura, Action<Habilidade[]> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id_criatura", idCriatura);
        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "list.php", form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) { Debug.LogError(www.error); callback(null); yield break; }
            string[] rows = ApiUtils.SplitLines(www.downloadHandler.text);
            var list = new List<Habilidade>();
            foreach (var r in rows)
            {
                if (string.IsNullOrWhiteSpace(r)) continue;
                var cols = r.Split(';');
                if (cols.Length < 4) continue;
                var h = new Habilidade
                {
                    id = int.Parse(cols[0]),
                    nome = cols[1],
                    descricao = cols[2],
                    custo_DNA = int.Parse(cols[3])
                };
                list.Add(h);
            }
            callback(list.ToArray());
        }
    }
}
