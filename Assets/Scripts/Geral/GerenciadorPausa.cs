using Assets.Scripts.Gameplay.View.EntradaDados;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Geral
{
    public class GerenciadorPausa : MonoBehaviour
    {

        [SerializeField] private ControlesArqueiro controleArqueiro;
        [SerializeField] private ControlesGuerreiro controleGuerreiro;
        [SerializeField] private FadeCanvas menuPausa;

        public static bool JogoPausado { get; private set; } = false;

        private IEnumerator Start()
        {
            if (menuPausa != null)
                yield return StartCoroutine(menuPausa.PreAquecer());            
        }

        private void Update()
        {
            if (GerenciadorPartida.jogoAcabou) return;


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (JogoPausado)
                {
                    controleArqueiro.Ligar();
                    controleGuerreiro.Ligar();
                    Retomar();
                }

                else
                {
                    controleArqueiro.Desligar();
                    controleGuerreiro.Desligar();
                    Pausar();
                }
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
            SceneManager.LoadScene("menu");
        }
    }
}