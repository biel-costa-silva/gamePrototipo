using Assets.Scripts.Entidades.Enums;
using Assets.Scripts.Jogabilidade.Mundo;
using Assets.Scripts.Nucleo.Interfaces;
using Assets.Scripts.Visoes.Animacoes;
using Assets.Scripts.Visual.Animacoes.Personagens;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Personagens.Guerreiro
{
    public class Guerreiro : Jogador
    {
        private AnimGuerreiro animacaoGuerreiro;
        private ComandosGuerreiro controleGuerreiro;
        [SerializeField] private GameObject[] prefabsVFX;
        [SerializeField] private Transform posicaoGuerreiro;


        private void Awake()
        {
            base.Awake();
            animacaoGuerreiro = animacao as AnimGuerreiro;
            controleGuerreiro = controle as ComandosGuerreiro;
            //forca a buscar de forma especifica -> ComandosGuerreiro e AnimGuerreiro


        }
        void Start()
        {
            estadoAtual = base.estadoAtual;


            nome = "Guerreiro";
            vida = 6;
            dano = 2;
            defesa = 2;
            velocidade = 5;
            velocidadeBase = velocidade;
        }
       
        void Update()
        {
            base.Update();

            if (estadoAtual == EstadoJogador.ModoAtaque)
            {
                if (controleGuerreiro.Defender())
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaDefendendo());
                    return;
                }
            }
            if (estadoAtual == EstadoJogador.AndandoArmado)
            {
                if (controleGuerreiro.Defender())
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaDefendendo());
                    return;
                }
            }

        }



        IEnumerator RotinaDefendendo()
        {
            animacaoGuerreiro.AnimacaoDefendendo();

            while (!animacaoGuerreiro.animacaoTerminou)
            {
                if (animacaoGuerreiro.estaRepelindo && sofreuAtaque)//janela para parry
                {
                    sofreuAtaque = false;
                    yield return StartCoroutine(RotinaRepelindo());
                    yield break;
                }

                else if (animacaoGuerreiro.estaDefendendo && sofreuAtaque)//janela para defesas
                {
                    sofreuAtaque = false;
                    yield return StartCoroutine(RotinaSofrendoAtqDefendendo());
                    yield break;
                }
                yield return null;

            }
            yield return StartCoroutine(animacaoGuerreiro.EsperarAnimacao());
            estadoAtual = EstadoJogador.ModoAtaque;
        }

        IEnumerator RotinaRepelindo()
        {
            animacaoGuerreiro.AnimacaoRepelindo();
            yield return StartCoroutine(animacaoGuerreiro.EsperarAnimacao());
            estadoAtual = EstadoJogador.ModoAtaque;
        }
        IEnumerator RotinaSofrendoAtqDefendendo()
        {
            animacaoGuerreiro.AnimacaoSofrendoAtqDef();
            yield return StartCoroutine(animacaoGuerreiro.EsperarAnimacao());
            estadoAtual = EstadoJogador.ModoAtaque;
        }



        //métodos da classe
        public void Agachar(KeyCode comando)
        {
            //animação
        }

        public override void SofrerAtaque(HitBox golpe)
        {
            if (animacaoGuerreiro.estaRepelindo)
            {
                sofreuAtaque = true;

                Destroy(golpe.gameObject);
                Debug.Log("Repeliu");

                GameObject VFXrepelir = Instantiate(prefabsVFX[0], posicaoGuerreiro.position, posicaoGuerreiro.rotation);
                float direcao = sprite.flipX ? -1f : 1f;

                Vector3 scale = VFXrepelir.transform.localScale;
                scale.x = Mathf.Abs(scale.x) * direcao;
                VFXrepelir.transform.localScale = scale;
                //mudanças futuras
            }
            else if (animacaoGuerreiro.estaDefendendo)
            {
                sofreuAtaque = true;

                golpe.dano -= defesa;
                base.SofrerAtaque(golpe);               
                Debug.Log("Defendeu");

                GameObject VFXdefesa = Instantiate(prefabsVFX[1], posicaoGuerreiro.position, posicaoGuerreiro.rotation);
                float direcao = sprite.flipX ? -1f : 1f;

                Vector3 scale = VFXdefesa.transform.localScale;
                scale.x = Mathf.Abs(scale.x) * direcao;
                VFXdefesa.transform.localScale = scale;

                rb.AddForce(new Vector2(50, 0), ForceMode2D.Impulse);
            }
            else
            {
                base.SofrerAtaque(golpe);
            }
        }
    }
}