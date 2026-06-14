using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GerenciadorMusicas.instancia.TocarMusica("Menu");
    }

    public void Play()
    {
        //GerenciadorMusicas.instancia.TocarMusica("Menu");
        SceneManager.LoadScene("jogo");
    }    
    public void Quit()
    {
        Application.Quit(); 
    }
}
