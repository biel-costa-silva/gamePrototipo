using Assets.Scripts.Geral;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class CursorMenu : MonoBehaviour
{
    public static CursorMenu instancia;

    [Tooltip("Marque na cena do menu principal. Na cena do jogo, deixe desmarcado.")]
    [SerializeField] private bool sempreAtivo = false;
    [SerializeField] private float velocidadeSlide = 14f;

    [SerializeField] private Texture2D texturaCursor;
    [SerializeField] private Vector2 pontaDoCursor = Vector2.zero;   // pixel da ponta, contado a partir do canto superior esquerdo


    private bool ativo;
    private bool deslizando;
    private bool pedidoSincronizar;
    private Vector2 ultimaPosicaoDefinida;
    private GameObject ultimoSelecionado;


    private void Awake()
    {
        instancia = this;
        ativo = sempreAtivo;
        Cursor.visible = ativo;
        Cursor.SetCursor(texturaCursor, pontaDoCursor, CursorMode.Auto);
    }

    public void Sincronizar()
    {
        pedidoSincronizar = true;
    }

    private void OnApplicationFocus(bool temFoco)
    {
        if (temFoco) Cursor.visible = ativo;   // o Windows/Editor pode reexibir o cursor ao trocar de janela
    }

    private void Update()
    {
        bool deve = sempreAtivo || GerenciadorPausa.JogoPausado || GerenciadorInteracoes.menuAberto;
        if (deve != ativo)
        {
            ativo = deve;
            deslizando = false;
        }

        if (Cursor.visible != ativo) Cursor.visible = ativo;

        if (!ativo || Mouse.current == null || EventSystem.current == null)
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

        // o que inicia um deslize: pedido do menu ou seta apertada
        bool comecar = pedidoSincronizar;
        pedidoSincronizar = false;

        if (SetaApertada())
        {
            if (atual == null && ultimoSelecionado != null)       // clicou no vazio e perdeu a seleção
                es.SetSelectedGameObject(ultimoSelecionado);
            comecar = true;
        }

        if (comecar)
        {
            if (!deslizando) ultimaPosicaoDefinida = posReal;      // ponto de partida correto
            deslizando = true;
        }

        if (!deslizando) return;

        GameObject alvo = es.currentSelectedGameObject;
        if (alvo == null) return;

        Vector2 destino = PosicaoNaTela(alvo.GetComponent<RectTransform>());
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