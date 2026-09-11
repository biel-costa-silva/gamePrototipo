using Assets.Scripts.Controller;
using Assets.Scripts.Model.Entidades.Objetos;
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
        [SerializeField] private GameObject telaGameOver;

        private bool jogoAcabou = false;

        private Vector3 checkpoint1;
        private Vector3 checkpoint2;

        private void Awake()
        {
            instancia = this;
        }

        private void OnEnable()
        {
            player1.OnMorreu += TratarMorte;
            player2.OnMorreu += TratarMorte;
            totem.OnAtivado += AoTotemAtivado;
        }

        private void OnDisable()
        {
            player1.OnMorreu -= TratarMorte;
            player2.OnMorreu -= TratarMorte;
            totem.OnAtivado -= AoTotemAtivado;
        }

        private void AoTotemAtivado()
        {
            SalvarCheckpoint();
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

            if(telaGameOver != null)
            {
                telaGameOver.SetActive(true);
            }
        }

        public void ReiniciarAposGameOver()
        {
            jogoAcabou = false;
            telaGameOver.SetActive(false);

            player1.transform.position = checkpoint1;
            player1.transform.position = checkpoint2;

            player1.Reviver();
            player2.Reviver();
        }
    }
}