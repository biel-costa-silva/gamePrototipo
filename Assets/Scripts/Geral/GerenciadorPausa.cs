using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Geral
{
    public class GerenciadorPausa : MonoBehaviour
    {
        public static bool JogoPausado { get; private set; } = false;

        [SerializeField] private FadeCanvas menuPausa;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (JogoPausado) Retomar();
                else Pausar();
            }
        }

        public void Pausar()
        {
            JogoPausado = true;
            Time.timeScale = 0f;
            menuPausa.Mostrar();
        }

        public void Retomar()
        {
            JogoPausado = false;
            menuPausa.Esconder();
            Time.timeScale = 1f;
        }

        public void SairParaMenu()
        {
            JogoPausado = false; // reseta o flag antes de trocar de cena
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}