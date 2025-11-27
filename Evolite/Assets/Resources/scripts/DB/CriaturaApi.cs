using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



public class CriaturaAPI : MonoBehaviour
{
    private string baseUrl = ApiConfig.BASE_URL + "criatura/";

    // create -> callback recebe (success, rawResponse)
    public IEnumerator Create(int id_criador, string nome, Action<bool, string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id_criador", id_criador);
        form.AddField("nome", nome);
        // REMOVIDAS: level e tipo

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "create.php", form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) { callback(false, www.error); yield break; }
            string err = ApiUtils.CheckErrorLine(www.downloadHandler.text);
            if (err != null) callback(false, err);
            else callback(true, www.downloadHandler.text.Trim());
        }
    }

    [Serializable]
    public class CriaturaGetResponse
    {
        public bool success;   // Para ler o campo "success": true/false
        public string message; // Para ler o campo "message" (se houver erro)
        public Criatura criatura; // Para ler o objeto aninhado "criatura": {...}
    }

    public IEnumerator Get(int idCriatura, System.Action<Criatura, string> callback)
    {
        // --- PONTO DE DEBUG 1: Construção da URL ---
        string url = baseUrl + "get.php?id=" + idCriatura;
        Debug.Log($"DEBUG API-GET 1: Iniciando busca. URL: {url}"); // <--- DEVE APARECER

        using (UnityEngine.Networking.UnityWebRequest webRequest = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            // Define um timeout de 10 segundos, caso o servidor demore.
            webRequest.timeout = 10;

            // --- PONTO DE DEBUG 2: ENVIO DA REQUISIÇÃO ---
            Debug.Log("DEBUG API-GET 2: Enviando requisição e esperando resposta..."); // <--- DEVE APARECER

            yield return webRequest.SendWebRequest(); // <--- O CÓDIGO ESTÁ TRAVANDO AQUI

            // --- PONTO DE DEBUG 3: RECEBIMENTO DA RESPOSTA ---
            Debug.Log("DEBUG API-GET 3: Resposta recebida. Verificando resultado."); // <--- SÓ APARECE SE A REQUISIÇÃO TERMINAR

            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                // --- PONTO DE DEBUG 4: ERRO DE CONEXÃO/SERVIDOR ---
                string errorMessage = $"Erro de WebRequest ({webRequest.responseCode}): {webRequest.error}";
                Debug.LogError($"DEBUG API-GET 4: FALHA TOTAL. Mensagem: {errorMessage}"); // <--- MENSAGEM CRÍTICA
                callback(null, errorMessage);
            }
            else
            {
                // --- PONTO DE DEBUG 5: SUCESSO DE COMUNICAÇÃO ---
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"DEBUG API-GET 5: SUCESSO DE COMUNICAÇÃO. JSON Bruto: {jsonResponse}"); // <--- JSON BRUTO

                try
                {
                    CriaturaGetResponse response = JsonUtility.FromJson<CriaturaGetResponse>(jsonResponse);

                    if (response.success)
                    {
                        // Sucesso, retorna o objeto Criatura
                        Debug.Log($"DEBUG API-GET 6: Parse OK. Criatura encontrada: {response.criatura.nome}");
                        callback(response.criatura, null);
                    }
                    else
                    {
                        // Falha no servidor (Criatura não encontrada ou erro interno)
                        Debug.LogWarning($"DEBUG API-GET 6: Resposta JSON falhou. Mensagem: {response.message}");
                        callback(null, response.message);
                    }
                }
                catch (Exception e)
                {
                    // Erro ao parsear o JSON
                    Debug.LogError($"DEBUG API-GET 7: Erro de parse JSON: {e.Message}\nJSON: {jsonResponse}");
                    callback(null, "Erro de parse: " + e.Message);
                }
            }
        }
    }

    public IEnumerator List(int id_criador, Action<Criatura[]> callback)
    {
        string url = baseUrl + "read.php";
        if (id_criador > 0) url += "?id_criador=" + id_criador;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Erro na requisição: {www.error ?? www.downloadHandler.text}");
                callback(null);
                yield break;
            }
            string[] rows = ApiUtils.SplitLines(www.downloadHandler.text);
            var list = new List<Criatura>();
            foreach (var r in rows)
            {
                if (string.IsNullOrWhiteSpace(r)) continue;
                var cols = r.Split(';');
                // VERIFICAÇÃO DE COLUNAS: Anteriormente era < 5, agora deve ser < 3 (id, id_criador, nome)
                if (cols.Length < 3) continue;
                var c = new Criatura
                {
                    id = int.Parse(cols[0]),
                    id_criador = int.Parse(cols[1]),
                    nome = cols[2],
                    // REMOVIDAS: level e tipo
                };
                list.Add(c);
            }
            callback(list.ToArray());
        }
    }

    public IEnumerator Changer(int id, string nome, Action<bool, string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        form.AddField("nome", nome);

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "update.php", form))
        {
            yield return www.SendWebRequest();
            string responseText = www.downloadHandler.text;
            Debug.Log($"DEBUG Changer resposta: {responseText}");

            if (www.result != UnityWebRequest.Result.Success)
            {
                callback(false, www.error);
                yield break;
            }

            // Garantir que JSON vazio/null não quebre o parse
            if (string.IsNullOrEmpty(responseText))
            {
                callback(false, "Resposta vazia do servidor");
                yield break;
            }

            try
            {
                var resp = JsonUtility.FromJson<CriaturaGetResponse>(responseText);
                if (resp != null && resp.success) callback(true, resp.message);
                else callback(false, resp != null ? resp.message : responseText);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Falha ao parsear JSON: {ex.Message}");
                // Fallback: se contém "Atualizado" ou "OK", considera sucesso
                if (responseText.Contains("OK") || responseText.Contains("Atualizado"))
                    callback(true, responseText);
                else
                    callback(false, responseText);
            }
        }
    }
}