using Assets.Scripts.Gameplay.Controller;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens.EnumsPersonagens;
using UnityEngine;

namespace Assets.Scripts.Gameplay.View.EntradaDados
{
    public class CerebroSlime : MonoBehaviour, IComportamento
    {
        [SerializeField] private float alcanceVisao = 8f;
        [SerializeField] private float alcanceCorpoACorpo = 1.2f;
        [SerializeField] private float alcanceDistancia = 6f;
        [SerializeField] private float intervaloDisparo = 4f;

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

        public int ComandoAtaque()
        {
            ControladorJogador alvo = AlvoMaisProximo();
            if (alvo == null) return 0;

            float dist = Mathf.Abs(alvo.transform.position.x - transform.position.x);
            
            if (dist <= alcanceCorpoACorpo) return 1;
            if(dist <= alcanceDistancia && Time.time >= proximoDisparo)
            {
                proximoDisparo = Time.time + intervaloDisparo;
                return 2;
            }
            return 0;
        }

        

        
    }
}