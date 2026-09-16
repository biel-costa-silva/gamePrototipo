using Assets.Scripts.Controller;
using Assets.Scripts.Model.Entidades.Objetos;
using Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Geral
{
    public class GerenciadorPartida : MonoBehaviour
    {
        public static GerenciadorPartida instancia;

        [SerializeField] private ControladorJogador player1;
        [SerializeField] private ControladorJogador player2;
        [SerializeField] private Totem totem;           
        [SerializeField] private Camera cameraController;
        [SerializeField] private TMPro.TextMeshProUGUI textoTempoSobrevivido;

        [SerializeField] private GameObject telaGameOver;       

        private float tempoInicioCombate;

        public static bool jogoAcabou { get; private set; } = false;

        private Vector3 checkpoint1;
        private Vector3 checkpoint2;

        [SerializeField] private VidaUIController hud1;
        [SerializeField] private VidaUIController hud2;

        private void Awake()
        {
            instancia = this;
        }

        private void OnEnable()
        {
            player1.OnMorreu += TratarMorte;
            player2.OnMorreu += TratarMorte;
            totem.OnAtivado += AoTotemAtivado;

            player1.OnEstadoAlterado += TratarMudancaEstado;
            player2.OnEstadoAlterado += TratarMudancaEstado;
        }

        private void OnDisable()
        {
            player1.OnMorreu -= TratarMorte;
            player2.OnMorreu -= TratarMorte;
            totem.OnAtivado -= AoTotemAtivado;

            player1.OnEstadoAlterado -= TratarMudancaEstado;
            player2.OnEstadoAlterado -= TratarMudancaEstado;
        }

        private void TratarMudancaEstado(ControladorJogador quemMudou, EstadoJogador novoEstado)
        {
            ControladorJogador outro = (quemMudou == player1) ? player2 : player1;

            if (novoEstado == EstadoJogador.ModoAtaque)
            {
                outro.AlertarEntrarEmModoAtaque();
                hud1.DefinirVisivel(true);
                hud2.DefinirVisivel(true);

            }else if (novoEstado == EstadoJogador.Parado || novoEstado == EstadoJogador.Andando)
            {
                outro.AlertarEDesarmar();
                hud1.DefinirVisivel(false);
                hud2.DefinirVisivel(false);
            }            
        }

        private void AoTotemAtivado()
        {
            SalvarCheckpoint();
            cameraController.TravarCamera();
            tempoInicioCombate = Time.time;
        }

        public void SalvarCheckpoint()
        {
            checkpoint1 = player1.transform.position;
            checkpoint2 = player2.transform.position;
        }
        
        private void TratarMorte(ControladorJogador quemMorreu)
        {
            if (jogoAcabou) return;
            jogoAcabou = true;

            ControladorJogador outroJogador = (quemMorreu == player1) ? player2 : player1;
            outroJogador.MorrerForcado();

            float tempoSobrevivido = Time.time - tempoInicioCombate;
            ExibirTempoNaTelaGameOver(tempoSobrevivido);

            if (telaGameOver != null)
            {
                telaGameOver.SetActive(true);                
            }
        }

        private void ExibirTempoNaTelaGameOver(float segundos)
        {
            int minutos = Mathf.FloorToInt(segundos / 60f);
            int segundosRestantes = Mathf.FloorToInt(segundos % 60f);

            textoTempoSobrevivido.text = $"tempo de sincronização {minutos:00}:{segundosRestantes:00}";
        }

        public void ReiniciarAposGameOver()
        {
            jogoAcabou = false;

            telaGameOver.SetActive(false);

            player1.transform.position = checkpoint1;
            player2.transform.position = checkpoint2;

            player1.Reviver();
            player2.Reviver();

            cameraController.DestravarCamera();
            totem.Resetar();
        }
    }
}