using Assets.Scripts.Geral;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class CursorMenu : MonoBehaviour
{
    [Tooltip("Marque na cena do menu principal. Na cena do jogo, deixe desmarcado.")]
    [SerializeField] private bool sempreAtivo = false;
    [SerializeField] private float velocidadeSlide = 14f;

    private bool ativo;
    private bool deslizando;
    private Vector2 ultimaPosicaoDefinida;
    private GameObject ultimoSelecionado;

    public static CursorMenu instancia;


    private void Awake()
    {
        instancia = this;
        ativo = sempreAtivo;
        Cursor.visible = ativo;
    }

    public void Sincronizar()
    {
        deslizando = true;
    }

    private void Update()
    {
        // ativo no menu principal, no pause e na confirmação do totem
        bool deve = sempreAtivo || GerenciadorPausa.JogoPausado || GerenciadorInteracoes.menuAberto;
        if (deve != ativo)
        {
            ativo = deve;
            deslizando = false;
        }
        if (Cursor.visible != ativo) Cursor.visible = ativo;

        if (!ativo || Mouse.current == null || EventSystem.current == null) return;

        EventSystem es = EventSystem.current;
        GameObject atual = es.currentSelectedGameObject;
        if (atual != null) ultimoSelecionado = atual;

        // o jogador mexeu o mouse de verdade: o mouse assume o controle
        Vector2 posReal = Mouse.current.position.ReadValue();
        if (deslizando && (posReal - ultimaPosicaoDefinida).sqrMagnitude > 25f)
            deslizando = false;

        // apertou uma seta: o cursor desliza até o botão selecionado
        if (SetaApertada())
        {
            if (atual == null && ultimoSelecionado != null)       // clicou no vazio e perdeu a seleção
                es.SetSelectedGameObject(ultimoSelecionado);
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
    private void OnApplicationFocus(bool temFoco)
    {
        if (temFoco) Cursor.visible = ativo;   // o Windows/Editor pode reexibir o cursor ao trocar de janela
    }

    private bool SetaApertada()
    {
        return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)
            || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);
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