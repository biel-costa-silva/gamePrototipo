using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private FadeCanvas menuPrincipal;
    [SerializeField] private FadeCanvas menuOpcoes;

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

    public void AbrirOpcoes()
    {
        menuPrincipal.Esconder(() => menuOpcoes.Mostrar());
    }

    public void VoltarDoOpcoes()
    {
        menuOpcoes.Esconder(() => menuPrincipal.Mostrar());
    }
}