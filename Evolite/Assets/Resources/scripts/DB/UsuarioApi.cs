using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UsuarioAPI : MonoBehaviour
{
    private string baseUrl = ApiConfig.BASE_URL + "usuario/";

    public IEnumerator Create(Usuario user, System.Action<bool, string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", user.username);
        form.AddField("senha", user.senha);

        using UnityWebRequest www = UnityWebRequest.Post(baseUrl + "create.php", form);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            callback(false, www.error);
        else
            callback(true, www.downloadHandler.text);
    }

    public IEnumerator Login(Usuario user, System.Action<bool, string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", user.username);
        form.AddField("senha", user.senha);

        using UnityWebRequest www = UnityWebRequest.Post(baseUrl + "read.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            callback(false, "Falha de conexão: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;

        if (json.Contains("\"success\":false"))
            callback(false, json); // usuário incorreto ou senha errada
        else
            callback(true, json); // login válido
    }
}
