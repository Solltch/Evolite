using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;

public class MenuFunctions : MonoBehaviour
{
    public Usuario user;
    public Usuario loggedUser;

    public Criatura criatura;
    public Criatura criaturaAtual;

    public UsuarioAPI usuarioApi;
    public CriaturaAPI criaturaApi;
    public CriatParteAPI criatParteApi;

    public TMP_InputField usernameInputField;
    public TMP_InputField senhaInputField;
    public TextMeshProUGUI console;
    public Player_General playerGeneral;

    public Material playerSkin;
    public Material playerSkin2;
    public Material playerSkin3;
    public Material playerEye;
    public Material playerPupil;
    public GameObject pauseMenu;
    public GameObject customMenu;
    public GameObject UsableMenus;
    public GameObject skillTree;
    private Vector3 CMpos;
    private Vector3 SMpos;
    public KeyCode pauseButton;
    public KeyCode skillTreeButton = KeyCode.K;
    public bool isAbleToPause = true;
    public bool isPaused;
    public bool isInSkillTree;
    public bool isIngame;
    public Color pickedColor;
    public Button autoclick;

    public void Start()
    {
        if (usuarioApi == null) usuarioApi = FindObjectOfType<UsuarioAPI>();
        if (criaturaApi == null) criaturaApi = FindObjectOfType<CriaturaAPI>();
        if (criatParteApi == null) criatParteApi = FindObjectOfType<CriatParteAPI>();

        if (skillTree != null)
            SMpos = skillTree.GetComponent<RectTransform>().anchoredPosition;
        if(customMenu != null )
            CMpos = customMenu.GetComponent<RectTransform>().anchoredPosition;
        isAbleToPause = true;
        pauseMenu.SetActive(false);
        skillTree.SetActive(false);

        if (isIngame)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            loggedUser = SessionManager.Instance.CurrentUser;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(pauseButton))
        {
            if (isAbleToPause)
            {
                if (!isPaused) PauseGame();
                else ResumeGame();
            }
        }

        if (Input.GetKeyDown(skillTreeButton))
        {
            if (!isInSkillTree) OpenTree();
            else CloseTree();
        }
    }

    private void OpenTree()
    {
        skillTree.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        skillTree.SetActive(true);
        Time.timeScale = 0f;
        isInSkillTree = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseTree()
    {
        skillTree.GetComponent<RectTransform>().anchoredPosition = SMpos;
        skillTree.SetActive(false);
        Time.timeScale = 1f;
        isInSkillTree = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PlayGame()
    {
        if (SessionManager.Instance.IsLoggedIn)
        {
            if (Input.GetKey(KeyCode.LeftShift)) SceneManager.LoadScene(1);
            else SceneManager.LoadScene(2);
        }
    }

    public void QuitGame() => Application.Quit();

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PauseGame()
    {
        if (!isPaused)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void CadastrarUsuario()
    {
        Debug.Log("Parte1");

        user.username = usernameInputField.text;
        user.senha = senhaInputField.text;

        Debug.Log("Parte2");

        StartCoroutine(usuarioApi.Create(user, (ok, res) =>
        {
            Debug.Log("Parte3");
            if (ok) SetConsoleMessage("Usuário criado!", false);
            else SetConsoleMessage($"Erro {res}!", true);
            Debug.Log("Parte4");
        }));
    }

    public void LogIn()
    {
        user.username = usernameInputField.text;
        user.senha = senhaInputField.text;

        StartCoroutine(usuarioApi.Login(user, (ok, res) =>
        {
            Debug.Log("RESPOSTA DO SERVIDOR" + res); 

            if (!ok)
            {
                console.text = "Falha de conexão:\n" + res;
                return;
            }

            string[] dados = res.Split('|');

            if (dados.Length == 0)
            {
                console.text = "Resposta inválida do servidor.";
                return;
            }

            if (dados[0] == "ERRO")
            {
                console.text = "Login falhou:\n" + (dados.Length > 1 ? dados[1] : "Motivo desconhecido");
                return;
            }

            if (dados[0] == "OK" && dados.Length >= 3)
            {
                console.text = "Login bem sucedido!";

                user.id = int.Parse(dados[1]);
                user.username = dados[2];

                SessionManager.Instance.SetCurrentUser(user);
                PlayGame();
                return;
            }

            console.text = "Resposta não reconhecida:\n" + res;
        }));
    }


    private void SetConsoleMessage(string message, bool isError)
    {
        if (console != null)
        {
            console.text = message;
            console.color = isError ? Color.red : Color.white;
        }
    }

    public void SalvarCriaturaPublic()
    {
        if (SessionManager.Instance == null)
        {
            SetConsoleMessage("Erro Crítico: SessionManager não está ativo.", true);
            return;
        }

        Debug.Log("Parte1");

        if (!SessionManager.Instance.IsLoggedIn)
        {
            SetConsoleMessage("Erro: Nenhum usuário logado.", true);
            return;
        }

        Debug.Log("Parte2");

        if (playerGeneral == null) playerGeneral = FindObjectOfType<Player_General>();
        if (criaturaApi == null) criaturaApi = FindObjectOfType<CriaturaAPI>();
        if (criatParteApi == null) criatParteApi = FindObjectOfType<CriatParteAPI>();


        criaturaAtual.id = playerGeneral.idCriatura;
        criaturaAtual.id_criador = loggedUser.id;
        criaturaAtual.nome = playerGeneral.nomeInput.text;
        List<Parte>PartesAtuais = new List<Parte>();


        Debug.Log("Parte3");

        if (playerGeneral == null || criaturaApi == null || criatParteApi == null)
        {
            SetConsoleMessage("Erro: Componentes essenciais faltando.", true);
            return;
        }

        Debug.Log("Parte4");

        SetConsoleMessage("Iniciando salvamento da criatura...", false);
        StartCoroutine(SalvarCriaturaCoroutine());
    }

    IEnumerator SalvarCriaturaCoroutine()
    {
        Debug.Log("Parte5: Preparando dados.");

        // 1. Garante que o ID do criador e o nome da criatura estão definidos
        criatura.id_criador = SessionManager.Instance.GetCurrentUser().id;
        // CORREÇÃO AQUI: Atribua o nome ANTES da verificação
        criatura.nome = playerGeneral.nomeInput.text;
        criatura.id = playerGeneral.idCriatura;

        // 2. Agora, verifique se o nome está vazio
        if (string.IsNullOrWhiteSpace(criatura.nome))
        {
            SetConsoleMessage("Erro: O nome da criatura não pode estar vazio!", true);
            Debug.LogWarning("Falha ao salvar: Nome da criatura vazio.");
            yield break; // Interrompe a Coroutine
        }

        Debug.Log("Parte6: Variáveis definidas.");

        Criatura criaturaExistente = null;
        string error = null;

        SetConsoleMessage("Procurando criatura existente...", false);

        Debug.Log("Parte7: Checando existência.");

        // --- Lógica de Criação ou Busca da Criatura (MANTIDA) ---
        if (criatura.id > 0)
        {
            yield return criaturaApi.Get(criatura.id, (c, err) => { criaturaExistente = c; error = err; });
            if (error != null) { SetConsoleMessage("Erro ao localizar criatura!", true); yield break; }
            Debug.Log("Parte8.1: Criatura existente localizada.");
        }
        else
        {
            Criatura[] lista = null;
            yield return criaturaApi.List(criatura.id_criador, (arr) => lista = arr);
            if (lista != null && lista.Length > 0)
            {
                criaturaExistente = lista[0];
                criatura.id = criaturaExistente.id;
                playerGeneral.idCriatura = criatura.id;
            }
            Debug.Log("Parte8.2: Checagem por criador concluída.");
        }
        // --------------------------------------------------------

        Debug.Log("Parte9: Processando status da criatura.");

        bool success = false;
        string apiResult = "";

        // --- Cria ou Atualiza a Criatura (MANTIDA) ---
        if (criaturaExistente != null)
        {
            yield return criaturaApi.Changer(criatura.id, criatura.nome, (s, r) => { success = s; apiResult = r; });
            if (!success) { SetConsoleMessage("Falha ao atualizar criatura.", true); yield break; }
            SetConsoleMessage("Criatura atualizada!", false);
            Debug.Log("Parte10.1: Criatura atualizada.");
        }
        else
        {
            yield return criaturaApi.Create(criatura.id_criador, criatura.nome, (s, r) => { success = s; apiResult = r; });
            if (!success || !int.TryParse(apiResult, out criatura.id))
            {
                SetConsoleMessage($"Erro ao cadastrar criatura! {apiResult}", true);
                yield break;
            }
            playerGeneral.idCriatura = criatura.id;
            SetConsoleMessage($"Criatura criada! ID = {criatura.id}", false);
            Debug.Log("Parte10.2: Criatura criada.");
        }
        // ---------------------------------------------

        if (criatura.id <= 0)
        {
            SetConsoleMessage("Erro: ID de criatura inválido após criação/busca.", true);
            yield break;
        }

        // --- PARTE DE SALVAMENTO DE PARTES (CORRIGIDA E SEM FISCALIZAÇÃO) ---

        Debug.Log("Parte11: Iniciando salvamento das partes.");

        // 1. Prepara as listas contendo DADOS DE TODAS AS 6 PARTES
        List<int> idsParaSalvar = new List<int>
    {
        playerGeneral.headBD.id, playerGeneral.eyeBD.id, playerGeneral.pupilBD.id,
        playerGeneral.faceBD.id, playerGeneral.headAcessBD.id, playerGeneral.bodyAcessBD.id,
    };

        List<int> tiposParaSalvar = new List<int>
    {
        playerGeneral.headBD.tipo, playerGeneral.eyeBD.tipo, playerGeneral.pupilBD.tipo,
        playerGeneral.faceBD.tipo, playerGeneral.headAcessBD.tipo, playerGeneral.bodyAcessBD.tipo,
    };

        List<int> equipadasParaSalvar = new List<int> { 1, 1, 1, 1, 1, 1 };

        // A verificação de ids <= 0 foi removida, permitindo qualquer número.

        // 2. Garante que CriatParteAPI está disponível
        if (criatParteApi == null) criatParteApi = FindObjectOfType<CriatParteAPI>();
        if (criatParteApi == null)
        {
            SetConsoleMessage("Erro: CriatParteAPI não encontrado.", true);
            yield break;
        }

        SetConsoleMessage("Salvando 6 partes da criatura...", false);

        Debug.Log("Parte12: Chamando AddPartes uma única vez.");

        // 3. Chama a API UMA ÚNICA VEZ com todas as listas
        yield return criatParteApi.AddPartes(criatura.id, idsParaSalvar, tiposParaSalvar, equipadasParaSalvar, (res) =>
        {

            bool salvou = res.ToLower().Contains("salvas");

            bool sucessoCompleto = salvou && res.ToLower().Contains("0 falhas");

            if (sucessoCompleto)
            {
                SetConsoleMessage("Salvamento de Partes: Sucesso total! ", false);
            }
            else
            {
                // Trata qualquer outra resposta como aviso/falha
                SetConsoleMessage("Falha/Aviso ao salvar partes. Verifique a resposta do servidor: " + res, true);
            }
        });

        Debug.Log("Parte16: Processo de salvamento concluído.");
    }

    public void PickPlayerColor(Button buttonClicked)
    {
        pickedColor = buttonClicked.colors.normalColor;
        playerSkin.SetColor("_Color", pickedColor);
    }

    public void PickPlayerColor2(Button buttonClicked)
    {
        pickedColor = buttonClicked.colors.normalColor;
        playerSkin2.SetColor("_Color", pickedColor);
    }

    public void PickPlayerColor3(Button buttonClicked)
    {
        pickedColor = buttonClicked.colors.normalColor;
        playerSkin3.SetColor("_Color", pickedColor);
    }

    public void PickEye(Button buttonClicked)
    {
        pickedColor = buttonClicked.colors.normalColor;
        playerEye.SetColor("_Color", pickedColor);
    }

    public void PickPupil(Button buttonClicked)
    {
        pickedColor = buttonClicked.colors.normalColor;
        playerPupil.SetColor("_Color", pickedColor);
    }

    public void LeaveCustumization()
    {
        customMenu.GetComponent<RectTransform>().anchoredPosition = CMpos;
        GameObject.Find("Player Collider").GetComponent<Player_Movement>().isAbleToMove = true;
        UsableMenus.SetActive(true);
        GameObject.Find("Player Sprite").GetComponent<Player_General>().isCustomizing = false;
        Time.timeScale = 1f;

        CinemachineRotationComposer cameraRotate = GameObject.Find("FreeLook Camera").GetComponent<CinemachineRotationComposer>();
        cameraRotate.Damping = Vector2.zero * 0.5f;
        cameraRotate.TargetOffset = Vector3.zero;

        isPaused = false;
        isAbleToPause = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !isPaused && isIngame)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
