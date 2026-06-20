using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private FadeCanvas menuPrincipal;
    [SerializeField] private FadeCanvas menuOpcoes;
    [SerializeField] private FundoEscuroController fundoEscuro;

    private void Start()
    {
        GerenciadorMusicas.instancia.TocarMusica("Menu");
    }

    public void Play()
    {
        SceneManager.LoadScene("jogo");
    }

    public void Quit()
    {
        Application.Quit();
    }

    // botaoClicado: arraste, no Inspector do OnClick(), o próprio botão que
    // está sendo clicado (o mesmo objeto que tem o BotaoAnimado).
    public void AbrirOpcoes(BotaoAnimado botaoClicado)
    {
        StartCoroutine(TrocarTelaAposAnimacao(botaoClicado, menuPrincipal, menuOpcoes, fundoEscuro.IrParaOpcoes));
    }

    public void VoltarDoOpcoes(BotaoAnimado botaoClicado)
    {
        StartCoroutine(TrocarTelaAposAnimacao(botaoClicado, menuOpcoes, menuPrincipal, fundoEscuro.IrParaPrincipal));
    }

    private IEnumerator TrocarTelaAposAnimacao(BotaoAnimado botaoClicado, FadeCanvas esconder, FadeCanvas mostrar, System.Action moverFundo)
    {
        Animator anim = botaoClicado.Animator;

        // Espera o Animator do botão terminar qualquer transição em
        // andamento e chegar num estado de repouso (Selected ou Normal).
        while (anim.IsInTransition(0) ||
               !(anim.GetCurrentAnimatorStateInfo(0).IsName("Selected") ||
                 anim.GetCurrentAnimatorStateInfo(0).IsName("Normal")))
        {
            yield return null;
        }

        // Dispara os dois ao mesmo tempo: o fade dos textos e o deslizar
        // do fundo escuro acontecem simultaneamente.
        moverFundo();
        esconder.Esconder(() => mostrar.Mostrar());
    }
}