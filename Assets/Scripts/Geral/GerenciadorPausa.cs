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
                if (JogoPausado) Retomar();        

                else Pausar();
            }
        }

        public void Pausar()
        {
            GerenciadorAudio.instancia.TocarAudio2D("entrarMenu");

            controleArqueiro.Desligar();
            controleGuerreiro.Desligar();
            JogoPausado = true;
            Time.timeScale = 0f;
            menuPausa.Mostrar();
        }

        public void Retomar()
        {
            GerenciadorAudio.instancia.TocarAudio2D("sairMenu");

            controleArqueiro.Ligar();
            controleGuerreiro.Ligar();
            JogoPausado = false;
            menuPausa.Esconder();
            Time.timeScale = 1f;
        }

        public void SairParaMenu()
        {
            JogoPausado = false; // reseta o flag antes de trocar de cena
            GerenciadorNivel.instancia.IrPara("Menu");
        }
    }
}