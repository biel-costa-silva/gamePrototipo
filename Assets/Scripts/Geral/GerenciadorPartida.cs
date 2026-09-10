using Assets.Scripts.Controller;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Geral
{
    public class GerenciadorPartida : MonoBehaviour
    {
        [SerializeField] private ControladorJogador player1;
        [SerializeField] private ControladorJogador player2;
        [SerializeField] private GameObject telaGameOver;

        private bool jogoAcabou = false;

        private void OnEnable()
        {
            player1.OnMorreu += TratarMorte;
            player2.OnMorreu += TratarMorte;
        }

        private void OnDisable()
        {
            player1.OnMorreu -= TratarMorte;
            player2.OnMorreu -= TratarMorte;
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
    }
}