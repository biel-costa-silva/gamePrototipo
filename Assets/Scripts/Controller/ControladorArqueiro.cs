using Assets.Scripts.Model.Entidades.Peoes;
using Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes;
using Assets.Scripts.Model.Entidades.Peoes.OficiosPeoes;
using Assets.Scripts.Model.Entidades.Peoes.UtilitariosPeoes;
using Assets.Scripts.View.AnimacaoPeoes;
using Assets.Scripts.View.EntradaDados;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class ControladorArqueiro : ControladorJogador
    {
        private Arqueiro arqueiro;
        private OficiosArqueiro oficioArqueiro;
        private AnimArqueiro animArqueiro;
        private ControlesArqueiro controleArqueiro;

        private void Awake()
        {
            base.Awake();

            arqueiro = jogador as Arqueiro;
            oficioArqueiro = oficio as OficiosArqueiro; 
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
            oficio.AplicarImpulsoAtaque(forcaAtq);
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
                        oficio.AplicarImpulsoAtaque(forcaAtq);
                        animacao.AnimacaoAtacando();
                        yield return null;
                    }
                    else break;

                }
            }

            yield return StartCoroutine(animacao.EsperarAnimacao()); // aguarda o último frame
            estadoAtual = EstadoJogador.ModoAtaque;
        }
    }
}