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

    public GameObject saveButtonContainer;
    private const string SAVE_BUTTON_PREFAB_PATH = "prefab/Menus/SaveButton";
    private GameObject saveButtonPrefab;
    public GameObject savesMenu;
    private Criatura[] savedCreatures;

    public void Start()
    {
        if (usuarioApi == null) usuarioApi = FindObjectOfType<UsuarioAPI>();
        if (criaturaApi == null) criaturaApi = FindObjectOfType<CriaturaAPI>();
        if (criatParteApi == null) criatParteApi = FindObjectOfType<CriatParteAPI>();

        if (skillTree != null)
            SMpos = skillTree.GetComponent<RectTransform>().anchoredPosition;
        if(customMenu != null )
            CMpos = customMenu.GetComponent<RectTransform>().anchoredPosition;

        saveButtonPrefab = Resources.Load<GameObject>(SAVE_BUTTON_PREFAB_PATH);
        if (saveButtonPrefab == null)
        {
            Debug.LogError($"Erro ao carregar o prefab: {SAVE_BUTTON_PREFAB_PATH}. Verifique o caminho!");
        }

        if (isIngame)
        {
            loggedUser = SessionManager.Instance.CurrentUser;
            isAbleToPause = true;
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(false);
            skillTree.SetActive(false);
            if (SessionManager.Instance.CurrentCreature.id != 0)
            {
                LoadCreature(SessionManager.Instance.CurrentCreature.id);
                playerGeneral.idCriatura = SessionManager.Instance.CurrentCreature.id;
            }
            
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Update()
    {
        if (isIngame)
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
    }

    public void OpenTree()
    {
        skillTree.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        skillTree.SetActive(true);
        Time.timeScale = 0f;
        isInSkillTree = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTree()
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

    public void OpenSavesMenu()
    {
        if (!SessionManager.Instance.IsLoggedIn || loggedUser == null)
        {
            Debug.Log("É preciso estar logado para ver os salvamentos!");
            return;
        }

        if (savesMenu == null || saveButtonContainer == null)
        {
            Debug.Log("Erro: Componentes do Menu de Salvamento não configurados!");
            return;
        }

        savesMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Inicia a busca pelas criaturas
        StartCoroutine(LoadCreaturesRoutine(loggedUser.id));
    }

    private IEnumerator LoadCreaturesRoutine(int creatorId)
    {
        Debug.Log("Buscando criaturas salvas...");

        Criatura[] list = null;
        // Chama a função List do CriaturaAPI
        yield return criaturaApi.List(creatorId, (arr) => list = arr);

        savedCreatures = list;

        if (savedCreatures == null || savedCreatures.Length == 0)
        {
            Debug.Log("Nenhuma criatura salva encontrada para este usuário.");
            yield break;
        }

        SetConsoleMessage($"Criaturas encontradas: {savedCreatures.Length}. Criando botões...", false);
        CreateSaveButtons(savedCreatures);
    }

    private void CreateSaveButtons(Criatura[] creatures)
    {
        if (saveButtonPrefab == null)
        {
            Debug.Log("Erro: Prefab do botão de salvamento não carregado!");
            return;
        }

        foreach (var creature in creatures)
        {
            // 1. Instancia o botão
            GameObject buttonObj = Instantiate(saveButtonPrefab, saveButtonContainer.transform);

            // 2. Tenta pegar o componente Button e o Text (assumindo que o texto é um filho)
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                // 3. Define o nome da criatura no texto do botão
                buttonText.text = creature.nome;
            }
            else
            {
                Debug.LogWarning($"Button Text (TextMeshProUGUI) não encontrado no filho do prefab {SAVE_BUTTON_PREFAB_PATH}.");
            }

            if (button != null)
            {
                // 4. Configura a ação do botão (Carregar Criatura)
                // Usamos uma expressão lambda para capturar o ID da criatura (closure)
                button.onClick.AddListener(() => StartGame(creature.id));
            }
        }
        this.gameObject.SetActive(false);
    }

    private void StartGame(int creatureId)
    {
        SessionManager.Instance.CurrentCreature.id = creatureId;
        SetConsoleMessage($"Tentando carregar a criatura com ID: {creatureId}", false);
        PlayGame();
    }

    public void LoadCreature(int creatureId)
    {
        Debug.Log($"Tentando carregar a criatura com ID: {creatureId}");

        // Garante que o Player_General está disponível
        if (playerGeneral == null) playerGeneral = FindObjectOfType<Player_General>();

        if (playerGeneral == null)
        {
            Debug.Log("Erro: Componente Player_General não encontrado.");
            return;
        }

        // Inicia a rotina de carregamento
        StartCoroutine(LoadCreatureRoutine(creatureId));
    }

    private IEnumerator LoadCreatureRoutine(int creatureId)
    {
        Debug.Log($"Buscando dados da criatura e partes para o ID: {creatureId}...");

        List<CriaturaParte> partes = null;
        string error = null;

        // NOVO PASSO 1: Busca os dados da criatura principal (onde o nome está)
        yield return criaturaApi.Get(creatureId, (c, err) =>
        {
            criaturaAtual = c;
            error = err;
        });

        if (error != null)
        {
            Debug.Log($"Falha ao carregar dados da criatura: {error}");
            yield break;
        }

        if (criaturaAtual == null)
        {
            Debug.Log("Criatura principal não encontrada.");
            yield break;
        }

        // NOVO PASSO 2: Salva o ID e o NOME no SessionManager
        if (SessionManager.Instance.CurrentCreature != null)
        {
            SessionManager.Instance.CurrentCreature.id = criaturaAtual.id;
            SessionManager.Instance.CurrentCreature.nome = criaturaAtual.nome;
            Debug.Log($"Dados da criatura carregados: ID={criaturaAtual.id}, Nome='{criaturaAtual.nome}'");
        }

        // 1. Chama a API para obter as partes
        yield return criatParteApi.GetByCriatura(creatureId, (list, err) =>
        {
            partes = list;
            error = err;
        });

        if (error != null)
        {
            Debug.Log($"Falha ao carregar partes: {error}");
            // Se houver falha, não continua, mas também não chama PlayGame()
            yield break;
        }

        if (partes == null || partes.Count == 0)
        {
            Debug.Log("Nenhuma parte encontrada para esta criatura. Iniciando com padrão.");
        }
        else
        {
            Debug.Log($"Partes carregadas: {partes.Count}. Aplicando ao jogador...");
            ApplyCreatureParts(partes);
            Debug.Log("Partes aplicadas com sucesso!");
        }
    }

    private void ApplyCreatureParts(List<CriaturaParte> partes)
    {
        if (playerGeneral == null) return;

        foreach (var parte in partes)
        {
            switch (parte.tipo_parte)
            {
                case 0:
                    playerGeneral.headIndex = parte.id_parte;
                    break;
                case 1: // Head
                    playerGeneral.eyeIndex = parte.id_parte;
                    break;
                case 2: // Eye
                    playerGeneral.pupilIndex = parte.id_parte;
                    break;
                case 3: // Pupil
                    playerGeneral.FaceIndex = parte.id_parte;
                    break;
                case 4: // Face (Skin/Body)
                    playerGeneral.headAcessoriesIndex = parte.id_parte;
                    break;
                case 5: // Head Accessory
                    playerGeneral.bodyAcessoriesIndex = parte.id_parte;
                    break;
                default:
                    Debug.LogWarning($"Tipo de parte desconhecido: {parte.tipo_parte}. Ignorando.");
                    break;
            }
        }

        // Você pode precisar de uma chamada para atualizar visualmente a criatura 
        // após definir todos os IDs (ex: playerGeneral.UpdateAppearance()).
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
            Debug.Log("RESPOSTA DO SERVIDOR: " + res); // Log para ver a resposta

            if (!ok)
            {
                SetConsoleMessage("Falha de conexão:\n" + res, true);
                return;
            }

            string[] dados = res.Split('|');

            if (dados.Length == 0)
            {
                SetConsoleMessage("Resposta inválida do servidor.", true);
                return;
            }

            if (dados[0] == "ERRO")
            {
                SetConsoleMessage("Login falhou:\n" + (dados.Length > 1 ? dados[1] : "Motivo desconhecido"), true);
                return;
            }

            if (dados[0] == "OK" && dados.Length >= 3)
            {
                SetConsoleMessage("Login bem sucedido!", false);

                // 1. Preenche o objeto 'user' com os dados do servidor
                user.id = int.Parse(dados[1]);
                user.username = dados[2];

                loggedUser = user;

                // 2. ATRIBUI AO SESSION MANAGER (O ÚNICO LUGAR DE VERDADE)
                SessionManager.Instance.SetCurrentUser(loggedUser);

                // 3. ATUALIZA A VARIÁVEL LOCAL 'loggedUser' (IMPORTANTE!)
                // Use a referência do Session Manager para garantir que é o mesmo objeto.
                loggedUser = SessionManager.Instance.CurrentUser;

                OpenSavesMenu();

                return;
            }

            SetConsoleMessage("Resposta não reconhecida:\n" + res, true);
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
        if (criatura.id != 0)
        {
            yield return criaturaApi.Get(criatura.id, (c, err) => { criaturaExistente = c; error = err; });
            if (error != null) { Debug.Log("Erro ao localizar criatura!"); yield break; }
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

        SessionManager.Instance.CurrentCreature.id = criatura.id;
        SessionManager.Instance.CurrentCreature.nome = criatura.nome;
        
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

        Debug.Log("Parte12: Chamando SaveAll para substituir/atualizar todas as partes.");

        yield return criatParteApi.SaveAll(criatura.id, idsParaSalvar, tiposParaSalvar, (success, res) =>
        {
            Debug.LogError("RESPOSTA CRUA DO PHP: " + res);

            if (success)
            {
                // O servidor respondeu com sucesso HTTP. Verifica o resultado lógico.
                if (res.ToLower().Contains("salvas"))
                {
                    SetConsoleMessage("Salvamento de Partes: Sucesso total! " + res, false);
                }
                else
                {
                    // Pode ser um erro de validação do PHP
                    SetConsoleMessage("Falha/Aviso ao salvar partes: " + res, true);
                }
            }
            else
            {
                // Falha na requisição HTTP (conexão, timeout, etc.)
                SetConsoleMessage("Erro de conexão ao salvar partes: " + res, true);
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
