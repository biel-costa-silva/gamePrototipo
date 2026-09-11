using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Geral
{
    public class GameOverInput : MonoBehaviour
    {
        private float tempoDeEspera = 4f;
        private float tempoAtivo;

        private void OnEnable()
        {
            tempoAtivo = 0f;
        }

        private void Update()
        {
            tempoAtivo += Time.deltaTime;

            if (tempoAtivo < tempoDeEspera) return;

            if (Input.anyKeyDown)
            {
                GerenciadorPartida.instancia.ReiniciarAposGameOver();
            }
        }
    }
}