using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;
using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Model.Entidades.Peoes;
using Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes;
using Assets.Scripts.View.AnimacaoPeoes;
using Assets.Scripts.View.EntradaDados;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class ControladorArqueiro : ControladorJogador
    {
        private Arqueiro arqueiro;
        private FisicaArqueiro fisicaArqueiro;
        private AnimArqueiro animArqueiro;
        private ControlesArqueiro controleArqueiro;

        //variaveis de controle
        [SerializeField] private float duracaoRolagem = 0.5f;

        private void Awake()
        {
            base.Awake();

            arqueiro = jogador as Arqueiro;
            fisicaArqueiro = fisica as FisicaArqueiro; 
            animArqueiro = animacao as AnimArqueiro;
            controleArqueiro = controle as ControlesArqueiro;
        }
        protected override Jogador CriarJogador()
        {
            return new Arqueiro();
        }

        void Update()
        {
            base.Update();

            // --- MODO DE ATAQUE ----
            if (estadoAtual == EstadoJogador.ModoAtaque || estadoAtual == EstadoJogador.AndandoArmado)
            {
                if (controleArqueiro.ComandoRolar())
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaRolando());
                    return;
                }

            }
        }

        protected override void ProcessarDano(HitBox golpe)
        {
            base.ProcessarDano(golpe);
        }

        protected override IEnumerator RotinaAtacando()
        {
            bool comboRegistrado = false;
            int forcaAtq = controle.ComandoAtaque();
            int contadorCombo = 0;
            animacao.indiceAtaque = 0;

            animacao.ResetarAnimacao();
            fisicaArqueiro.AplicarImpulsoAtaque(forcaAtq);
            animacao.AnimacaoAtacando();

            yield return null;

            while (!animacao.animacaoTerminou)
            {
                if (animacao.novoAtaque)//janela aberta
                {
                    if (contadorCombo <= 1 && controle.ComandoAtaque() > 0)//dano igual 0 significa que não atacou
                    {
                        comboRegistrado = true;
                    }

                    //pode sofrer dano durante o ataque (quem acerta outro antes)
                    if (arqueiro.sofreuAtaque)
                    {
                        arqueiro.LimparFlagAtaque();
                        yield return StartCoroutine(RotinaSofrendoAtaqueArm());
                        yield break;
                    }
                    yield return null;
                }
                else//janela fechada
                {
                    if (comboRegistrado)
                    {
                        comboRegistrado = false;
                        contadorCombo++;
                        animacao.indiceAtaque = contadorCombo;//muda na classe ControladorAnim.

                        animacao.ResetarAnimacao();
                        fisicaArqueiro.AplicarImpulsoAtaque(forcaAtq);
                        animacao.AnimacaoAtacando();
                        yield return null;
                    }
                    else break;

                }
            }

            yield return StartCoroutine(animacao.EsperarAnimacao()); // aguarda o último frame
            estadoAtual = EstadoJogador.ModoAtaque;
        }

        IEnumerator RotinaRolando()
        {
            invulneravel = true;
            animArqueiro.AnimacaoRolando();

            float velocidade = arqueiro.GetVelocidade();
            float tempo = 0f;

            while (tempo < duracaoRolagem)
            {
                fisicaArqueiro.AplicarVelocidadeRolagem(velocidade, tempo / duracaoRolagem);
                tempo += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            fisicaArqueiro.PararRolagem();
            invulneravel = false;

            yield return StartCoroutine(animArqueiro.EsperarAnimacao());
            estadoAtual = EstadoJogador.ModoAtaque;
        }
    }
}