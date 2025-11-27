// Assets/Scripts/Api/HabilidadeAPI.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HabilidadeAPI : MonoBehaviour
{
    private string baseUrl = ApiConfig.BASE_URL + "habilidade/";

    // list all
    public IEnumerator List(Action<Habilidade[]> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(baseUrl + "read.php"))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) { Debug.LogError(www.error); callback(null); yield break; }
            string[] rows = ApiUtils.SplitLines(www.downloadHandler.text);
            var outList = new List<Habilidade>();
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
                outList.Add(h);
            }
            callback(outList.ToArray());
        }
    }
}
