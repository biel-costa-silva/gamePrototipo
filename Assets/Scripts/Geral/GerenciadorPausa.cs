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
        public static GerenciadorPausa instancia;

        private void Awake()
        {
            instancia = this;
        }

        private IEnumerator Start()
        {
            if (menuPausa != null)
                yield return StartCoroutine(menuPausa.PreAquecer());            
        }

        private void Update()
        {
            if (GerenciadorPartida.jogoAcabou) return;
            if (GerenciadorTutorial.aberto) return;   // ESC não abre o menu de pausa durante o tutorial

            if (Input.GetKeyDown(KeyCode.Escape))
            {                
                if (JogoPausado) Retomar();        

                else Pausar();
            }
        }

        public void Pausar()
        {
            GerenciadorAudio.instancia.TocarAudio2D("entrarMenu");
            GerenciadorAudio.instancia.Abafar(true);

            controleArqueiro.Desligar();
            controleGuerreiro.Desligar();
            JogoPausado = true;
            Time.timeScale = 0f;
            menuPausa.Mostrar();
        }

        public void Retomar()
        {
            GerenciadorAudio.instancia.TocarAudio2D("sairMenu");
            GerenciadorAudio.instancia.Abafar(false);

            controleArqueiro.Ligar();
            controleGuerreiro.Ligar();
            JogoPausado = false;
            menuPausa.Esconder();
            Time.timeScale = 1f;
        }

        // ---- Pausa usada pelo tutorial (congela tudo, mas sem abrir o menu de pausa) ----
        public void PausarParaTutorial()
        {
            GerenciadorAudio.instancia.Abafar(true);
            controleArqueiro.Desligar();
            controleGuerreiro.Desligar();
            JogoPausado = true;
            Time.timeScale = 0f;
        }

        public void RetomarDoTutorial()
        {
            GerenciadorAudio.instancia.Abafar(false);
            controleArqueiro.Ligar();
            controleGuerreiro.Ligar();
            JogoPausado = false;
            Time.timeScale = 1f;
        }

        public void SairParaMenu()
        {
            JogoPausado = false; // reseta o flag antes de trocar de cena
            GerenciadorNivel.instancia.IrPara("Menu");
        }
    }
}