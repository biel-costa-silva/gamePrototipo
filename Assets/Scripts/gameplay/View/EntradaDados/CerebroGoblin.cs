using Assets.Scripts.Gameplay.Controller;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens.EnumsPersonagens;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.View.EntradaDados
{
    public class CerebroGoblin : MonoBehaviour, IComportamento
    {
        [Header("Alcances")]
        [SerializeField] private float alcanceVisao = 8f;
        [Tooltip("Distância para iniciar o corte. Lembre: ele arranca para frente antes de cortar.")]
        [SerializeField] private float alcanceCorpoACorpo = 1.8f;
        [SerializeField] private float alcanceDistancia = 6f;
        [SerializeField] private float intervaloFaca = 4f;

        [Header("Esquiva")]
        [Tooltip("Se o guerreiro estiver atacando e a esta distância (ou menos), o goblin esquiva.")]
        [SerializeField] private float alcanceEsquiva = 2f;

        private ControladorJogador[] jogadores;
        private Vector2 ultimaPosicaoAlvo;
        private float proximoDisparo = 0f;

        private void Start()
        {
            jogadores = FindObjectsByType<ControladorJogador>(FindObjectsSortMode.None);
            ultimaPosicaoAlvo = transform.position;
        }

        private ControladorJogador AlvoMaisProximo()
        {
            ControladorJogador melhor = null;
            float menor = float.MaxValue;

            foreach (ControladorJogador j in jogadores)
            {
                if (j == null || j.estadoAtual == EstadoJogador.Morto) continue;

                float d = Mathf.Abs(j.transform.position.x - transform.position.x);
                if (d < menor) { menor = d; melhor = j; }
            }
            return melhor;
        }

        public Vector2 PosicaoAlvo()
        {
            ControladorJogador alvo = AlvoMaisProximo();
            if (alvo != null) ultimaPosicaoAlvo = alvo.transform.position;
            return ultimaPosicaoAlvo;
        }

        public float ComandoMovimento()
        {
            ControladorJogador alvo = AlvoMaisProximo();
            if (alvo == null) return 0f;

            float dx = alvo.transform.position.x - transform.position.x;
            float dist = Mathf.Abs(dx);

            if (dist > alcanceVisao || dist <= alcanceCorpoACorpo) return 0f;
            return Mathf.Sign(dx);
        }

        // 0 = nada | 1 = corte (combo: corte -> recuo -> faca) | 2 = só a faca
        public int ComandoAtaque()
        {
            ControladorJogador alvo = AlvoMaisProximo();
            if (alvo == null) return 0;

            float dist = Mathf.Abs(alvo.transform.position.x - transform.position.x);

            if (dist <= alcanceCorpoACorpo) return 1;
            if (dist <= alcanceDistancia && Time.time >= proximoDisparo)
            {
                proximoDisparo = Time.time + intervaloFaca;
                return 2;
            }
            return 0;
        }

        // O guerreiro está perto E no meio de um ataque? Então o goblin deve esquivar.
        public bool JogadorVaiAtacar()
        {
            ControladorJogador alvo = AlvoMaisProximo();
            if (alvo == null) return false;

            float dist = Mathf.Abs(alvo.transform.position.x - transform.position.x);
            return dist <= alcanceEsquiva && alvo.estaAtacando;
        }

        // Chamado pelo controlador depois de jogar uma faca (inclusive a do combo)
        public void ReiniciarIntervaloFaca()
        {
            proximoDisparo = Time.time + intervaloFaca;
        }

    }
}