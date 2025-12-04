using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }
    public bool IsLoggedIn => CurrentUser.id != -1;
    public Usuario CurrentUser;
    public Criatura CurrentCreature;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentUser(Usuario user)
    {
        CurrentUser = user;
        Debug.Log($"Sessão iniciada para o ID: {CurrentUser.id}, Usuário: {CurrentUser.username}");
    }

    public Usuario GetCurrentUser()
    {
        return CurrentUser;
    }
}
