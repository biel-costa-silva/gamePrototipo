using Assets.Scripts.Geral;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class CursorMenu : MonoBehaviour
{
    public static CursorMenu instancia;

    [Tooltip("Marque na cena do menu principal. Na cena do jogo, deixe desmarcado.")]
    [SerializeField] private bool sempreAtivo = false;

    [Tooltip("Arraste aqui painéis de menu extras (ex.: Painel do Menu da Fogueira) para o cursor aparecer automaticamente quando eles abrirem.")]
    [SerializeField] private GameObject[] menusExtras;

    [Header("Visual do Cursor")]
    [SerializeField] private Texture2D texturaCursor;
    [SerializeField] private Vector2 pontaDoCursor = Vector2.zero;   // pixel da ponta, a partir do canto superior esquerdo
    [Tooltip("Se o cursor ainda não aparecer no .exe mesmo com Texture Type = Cursor, marque esta opção.")]
    [SerializeField] private bool forcarCursorSoftware = false;

    [Header("Navegação por Teclado")]
    [Tooltip("Desmarcado evita bugs de posição/DPI no .exe ao usar setas/WASD junto com o mouse.")]
    [SerializeField] private bool moverMouseComTeclado = false;
    [SerializeField] private float velocidadeSlide = 14f;

    private bool ativo;
    private bool deslizando;
    private bool pedidoSincronizar;
    private Vector2 ultimaPosicaoDefinida;
    private GameObject ultimoSelecionado;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RegistrarEventoCena()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private static void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        // Garante que cenas sem CursorMenu (ex.: Cutscene, PosMorte) sempre escondam o cursor imediatamente
        if (FindFirstObjectByType<CursorMenu>() == null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
    }

    private void Awake()
    {
        instancia = this;
        Cursor.lockState = CursorLockMode.None;
        ativo = sempreAtivo;
        AplicarTexturaCursor();
        Cursor.visible = ativo;
    }

    public void Sincronizar()
    {
        if (moverMouseComTeclado)
            pedidoSincronizar = true;
    }

    private void OnApplicationFocus(bool temFoco)
    {
        if (!temFoco) return;
        AplicarTexturaCursor();
        Cursor.visible = ativo;   // o Windows pode reexibir/resetar o cursor ao dar Alt+Tab
    }

    private void AplicarTexturaCursor()
    {
        if (texturaCursor == null) return;
        CursorMode modo = forcarCursorSoftware ? CursorMode.ForceSoftware : CursorMode.Auto;
        Cursor.SetCursor(texturaCursor, pontaDoCursor, modo);
    }

    private bool AlgumMenuExtraAberto()
    {
        if (menusExtras == null) return false;

        foreach (GameObject menu in menusExtras)
        {
            if (menu == null || !menu.activeInHierarchy) continue;

            CanvasGroup cg = menu.GetComponent<CanvasGroup>();
            if (cg == null || (cg.alpha > 0.1f && cg.interactable))
                return true;
        }
        return false;
    }

    private void Update()
    {
        bool menuPausaAberto = GerenciadorPausa.JogoPausado && !GerenciadorTutorial.aberto;
        bool deve = sempreAtivo || menuPausaAberto || GerenciadorInteracoes.menuAberto || AlgumMenuExtraAberto();

        if (deve != ativo)
        {
            ativo = deve;
            deslizando = false;
            if (ativo) AplicarTexturaCursor();
        }

        if (Cursor.visible != ativo)
            Cursor.visible = ativo;

        if (!ativo || !moverMouseComTeclado || Mouse.current == null || EventSystem.current == null)
        {
            pedidoSincronizar = false;
            return;
        }

        EventSystem es = EventSystem.current;
        GameObject atual = es.currentSelectedGameObject;
        if (atual != null) ultimoSelecionado = atual;

        Vector2 posReal = Mouse.current.position.ReadValue();

        // o jogador mexeu o mouse de verdade durante o deslize: o mouse assume
        if (deslizando && (posReal - ultimaPosicaoDefinida).sqrMagnitude > 25f)
            deslizando = false;

        bool comecar = pedidoSincronizar;
        pedidoSincronizar = false;

        if (SetaApertada())
        {
            if (atual == null && ultimoSelecionado != null)
                es.SetSelectedGameObject(ultimoSelecionado);
            comecar = true;
        }

        if (comecar)
        {
            if (!deslizando) ultimaPosicaoDefinida = posReal;
            deslizando = true;
        }

        if (!deslizando) return;

        GameObject alvo = es.currentSelectedGameObject;
        if (alvo == null) return;

        RectTransform rt = alvo.GetComponent<RectTransform>();
        if (rt == null) return;

        Vector2 destino = PosicaoNaTela(rt);
        Vector2 nova = Vector2.Lerp(posReal, destino, velocidadeSlide * Time.unscaledDeltaTime);

        if ((destino - nova).sqrMagnitude < 4f)
        {
            nova = destino;
            deslizando = false;
        }

        Definir(nova);
    }

    private bool SetaApertada()
    {
        return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)
            || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D);
    }

    private void Definir(Vector2 pos)
    {
        Mouse.current.WarpCursorPosition(pos);
        InputState.Change(Mouse.current.position, pos);
        ultimaPosicaoDefinida = pos;
    }

    private Vector2 PosicaoNaTela(RectTransform rt)
    {
        Canvas canvas = rt.GetComponentInParent<Canvas>();
        Camera cam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;

        Vector3 centro = rt.TransformPoint(rt.rect.center);
        return RectTransformUtility.WorldToScreenPoint(cam, centro);
    }
}