using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Model.Entidades.Peoes;
using Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes;
using Assets.Scripts.Model.Entidades.Peoes.OficiosPeoes;
using Assets.Scripts.View.AnimacaoPeoes;
using Assets.Scripts.View.EntradaDados;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class ControladorGuerreiro : ControladorJogador
    {
        private Guerreiro guerreiro;
        private OficiosGuerreiro oficioGuerreiro;
        private AnimGuerreiro animGuerreiro;
        private ControlesGuerreiro controleGuerreiro;
        
        private void Awake()
        {
            base.Awake();

            guerreiro = jogador as Guerreiro;
            oficioGuerreiro = oficio as OficiosGuerreiro;
            animGuerreiro = animacao as AnimGuerreiro;
            controleGuerreiro = controle as ControlesGuerreiro;
        }
        protected override Jogador CriarJogador()
        {
            return new Guerreiro();
        }

        void Update()
        {
            base.Update();

            // --- MODO ATAQUE ---
            if (estadoAtual == EstadoJogador.ModoAtaque || estadoAtual == EstadoJogador.AndandoArmado)
            {
                if (controleGuerreiro.Defender())
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaDefendendo());
                    return;
                }
                if (controleGuerreiro.ComandoAgachar())
                {
                    estadoAtual = EstadoJogador.Agachado;
                    return;
                }
            }            

            // --- AGACHADO ---
            if(estadoAtual == EstadoJogador.Agachado)
            {
                if (guerreiro.sofreuAtaque)
                {
                    guerreiro.LimparFlagAtaque();
                    animGuerreiro.AnimacaoAgachado(false);
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaSofrendoAtaqueArm());
                    return;
                }
                if (controleGuerreiro.ComandoAgachar())
                {
                    animGuerreiro.AnimacaoAgachado(true);
                }
                else
                {
                    animGuerreiro.AnimacaoAgachado(false);
                    estadoAtual = EstadoJogador.ModoAtaque;
                }
            }
        }
               
        protected override void ProcessarDano(HitBox golpe)
        {
            bool repelindo = animGuerreiro.estaRepelindo;
            bool defendendo = animGuerreiro.estaDefendendo;
            bool deCostas = oficio.EstaDeCostas(golpe.direcao);
                                 
            guerreiro.deCostas = deCostas;

            if (deCostas)
            {
                oficio.VirarParaLadoDoGolpe(golpe.direcao);
                jogador.ReceberDano(golpe.dano);
                oficio.AplicarImpulsoGolpeRecebido(golpe);
                Debug.Log("Recebeu dano" + golpe.dano);
                return;
            }

            int danoFinal = guerreiro.CalcularDanoRecebido(golpe.dano, defendendo, repelindo);

            if (repelindo)
            {
                guerreiro.SetSofreuAtaque();
                oficioGuerreiro.SpawnarVFX(0);
                oficioGuerreiro.AplicarImpulsoCustom(10f);
            }
            else if (defendendo)
            {               
                jogador.ReceberDano(danoFinal);
                oficio.AplicarImpulsoGolpeRecebido(golpe);
                oficioGuerreiro.SpawnarVFX(1);
                oficioGuerreiro.AplicarImpulsoCustom(50f);
                Debug.Log("Recebeu dano" + danoFinal);                
            }
            else
            {
                jogador.ReceberDano(danoFinal);
                oficio.AplicarImpulsoGolpeRecebido(golpe);
                Debug.Log("Recebeu dano" + danoFinal);
            }
        }
       
        protected override IEnumerator RotinaAtacando()
        {
            bool comboRegistrado = false;
            guerreiro.sofreuChoque = false;
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
                    if (contadorCombo <= 2 && controle.ComandoAtaque() > 0)//dano igual 0 significa que não atacou
                    {
                        comboRegistrado = true;
                    }
                    if (guerreiro.sofreuChoque)
                    {
                        guerreiro.LimparFlagChoque();
                        yield return StartCoroutine(RotinaRecebeChoqueAtqs());
                        yield break;
                    }
                    //pode sofrer dano durante o ataque (quem acerta outro antes)
                    if (guerreiro.sofreuAtaque)
                    {
                        guerreiro.LimparFlagAtaque();
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
            yield return StartCoroutine(animacao.EsperarAnimacao());
            estadoAtual = EstadoJogador.ModoAtaque;
        }

        IEnumerator RotinaDefendendo()
        {
            animGuerreiro.AnimacaoDefendendo();

            while (!animGuerreiro.animacaoTerminou)
            {
                if (guerreiro.sofreuAtaque && guerreiro.deCostas)
                {
                    guerreiro.LimparFlagAtaque();
                    yield return StartCoroutine(RotinaSofrendoAtaqueArm());
                    yield break;
                }
                else if (guerreiro.sofreuAtaque)
                {
                    guerreiro.LimparFlagAtaque();

                    if (animGuerreiro.estaRepelindo)
                    {
                        yield return StartCoroutine(RotinaRepelindo());
                    }
                    else if (animGuerreiro.estaDefendendo)
                    {
                        yield return StartCoroutine(RotinaSofrendoAtqDefendendo());
                    }
                    else
                    {
                        yield return StartCoroutine(RotinaSofrendoAtaqueArm());
                    }
                    yield break;
                }
                yield return null;
            }
            yield return StartCoroutine(animGuerreiro.EsperarAnimacao());
            estadoAtual = EstadoJogador.ModoAtaque;
        }
        
        IEnumerator RotinaRepelindo()
        {
            animGuerreiro.AnimacaoRepelindo();
            yield return StartCoroutine(animGuerreiro.EsperarAnimacao());
            estadoAtual = EstadoJogador.ModoAtaque;
        }

        IEnumerator RotinaSofrendoAtqDefendendo()
        {
            animGuerreiro.AnimacaoSofrendoAtqDef();
            yield return StartCoroutine(animGuerreiro.EsperarAnimacao());
            estadoAtual = EstadoJogador.ModoAtaque;
        }
        IEnumerator RotinaRecebeChoqueAtqs()
        {
            oficioGuerreiro.ReceberChoque();
            animGuerreiro.AnimacaoRecebeChoqueAtqs();
            yield return StartCoroutine(animGuerreiro.EsperarAnimacao());
            estadoAtual = EstadoJogador.ModoAtaque;
        }

    }
}