using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes;
using Assets.Scripts.Model.Entidades.Peoes.OficiosPeoes;
using Assets.Scripts.View;
using Assets.Scripts.View.EntradaDados;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class ControladorJogador : MonoBehaviour
    {
        //model
        public Jogador jogador;
        //oficios do model
        protected OficiosJogador oficio;
        //view       
        protected ControladorAnim animacao;
        //controle
        protected IControles controle;
        //variaveis de controle
        protected EstadoJogador estadoAtual = EstadoJogador.Parado;
        protected IInteracoes interagivelAtual;
     
        public void Awake()
        {
            oficio = GetComponent<OficiosJogador>();            
            animacao = GetComponent<ControladorAnim>();
            controle = GetComponent<IControles>();

            //cria o model
            jogador = CriarJogador();  
        }
        protected virtual Jogador CriarJogador()
        {
            return new Jogador();
        }

        protected virtual void Update()
        {
            HitBox golpe = oficio.ConsumirGolpePendente();
            if (golpe != null && !golpe.consumida)
            {
                Debug.Log($"Controller processando golpe. Consumida: {golpe.consumida}");
                ProcessarDano(golpe);
            }
              
            interagivelAtual = oficio.GetInteragivelPendente();

            if (estadoAtual == EstadoJogador.Ocupado) return;

            // --- PARADO ---
            if (estadoAtual == EstadoJogador.Parado)
            {
                jogador.SetVelocidade(jogador.GetVelocidadeBase());
                animacao.AnimacaoParado();

                if (jogador.sofreuAtaque)
                {
                    jogador.LimparFlagAtaque();
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaSofrendoAtaqueDesarm());
                    return;
                }
                if (controle.ComandoMovimento() != 0)
                {
                    estadoAtual = EstadoJogador.Andando;
                    return;
                }
                if (controle.ComandoSaqueArma())
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaSacanadoArma());
                    return;
                }
                if (controle.ComandoInteracao() && interagivelAtual != null)
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaInteracao());
                    interagivelAtual.SofrerInteracao(jogador);
                    return;
                }
            }

            // --- ANDANDO ---
            if (estadoAtual == EstadoJogador.Andando)
            {
                if (jogador.sofreuAtaque)
                {
                    jogador.LimparFlagAtaque();
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaSofrendoAtaqueDesarm());
                    return;
                }
                if (controle.ComandoMovimento() != 0)
                {
                    oficio.Locomover(controle.ComandoMovimento(), jogador.GetVelocidade());
                    animacao.AnimacaoAndando(true);
                }
                else
                {
                    estadoAtual = EstadoJogador.Parado;
                }
                if (controle.ComandoSaqueArma())
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaSacanadoArma());
                    return;
                }
            }

            // --- MODO ATAQUE ---
            if (estadoAtual == EstadoJogador.ModoAtaque)
            {
                jogador.SetVelocidade(jogador.GetVelocidadeBase() + 2);
                animacao.AnimacaoParadoArmado();

                if (jogador.sofreuAtaque)
                {
                    jogador.LimparFlagAtaque();
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaSofrendoAtaqueArm());
                    return;
                }
                if (controle.ComandoMovimento() != 0)
                {
                    estadoAtual = EstadoJogador.AndandoArmado;
                    return;
                }
                if (controle.ComandoSaqueArma())
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaGuardandoArma());
                    return;
                }
                if (controle.ComandoAtaque() > 0)
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaAtacando());
                    return;
                }
            }

            // --- ANDANDO ARMADO ---
            if (estadoAtual == EstadoJogador.AndandoArmado)
            {
                if (jogador.sofreuAtaque)
                {
                    jogador.LimparFlagAtaque();
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaSofrendoAtaqueArm());
                    return;
                }
                if (controle.ComandoMovimento() != 0)
                {
                    oficio.Locomover(controle.ComandoMovimento(), jogador.GetVelocidade());
                    animacao.AnimacaoAndandoArmado(true);
                }
                else
                {
                    estadoAtual = EstadoJogador.ModoAtaque;
                }
                if (controle.ComandoSaqueArma())
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaGuardandoArma());
                    return;
                }
                if (controle.ComandoAtaque() > 0)
                {
                    estadoAtual = EstadoJogador.Ocupado;
                    StartCoroutine(RotinaAtacando());
                    return;
                }
            }
        }       
        public void ReceberGolpe(HitBox golpe)
        {
            if (golpe == null || golpe.consumida) return;            
            ProcessarDano(golpe);
        }
        // Virtual — cada subclasse sobrescreve se tiver regra especial 
        protected virtual void ProcessarDano(HitBox golpe)
        {
            jogador.ReceberDano(golpe.dano);
            oficio.AplicarImpulsoGolpeRecebido(golpe);
            Debug.Log("Recebeu dano:" + golpe.dano);
        }      

        public void AplicarGolpe(int indice)
        {
            oficio.AplicarGolpe(this, jogador.GetDano(), indice);
        }




        // #------------------------------ Rotinas - Disparo de Animações Diretas sem Interrupção -----------------------------#

        protected IEnumerator RotinaSacanadoArma()
        {
            animacao.AnimacaoSacandoArma();
            yield return StartCoroutine(animacao.EsperarAnimacao());
            estadoAtual = EstadoJogador.ModoAtaque;
        }
        //
        protected IEnumerator RotinaGuardandoArma()
        {
            animacao.AnimacaoGuardandoArma();
            yield return StartCoroutine(animacao.EsperarAnimacao());
            estadoAtual = EstadoJogador.Parado;
        }
        //
        protected IEnumerator RotinaInteracao()
        {
            animacao.AnimacaoInteragindo();
            yield return StartCoroutine(animacao.EsperarAnimacao());
            estadoAtual = EstadoJogador.Parado;
        }
        //
        protected virtual IEnumerator RotinaAtacando()
        {
            int forcaAtq = controle.ComandoAtaque();

            animacao.ResetarAnimacao();
            oficio.AplicarImpulsoAtaque(forcaAtq);
            animacao.AnimacaoAtacando();

            yield return StartCoroutine(animacao.EsperarAnimacao()); // aguarda o último frame
            estadoAtual = EstadoJogador.ModoAtaque;
        }

        //------------------- Sofrendo Ataques: possibilidades --------------------
        protected IEnumerator RotinaSofrendoAtaqueDesarm()
        {
            animacao.AnimacaoSofrendoAtqDesarm();
            yield return StartCoroutine(animacao.EsperarAnimacao());
            estadoAtual = EstadoJogador.Parado;
        }
        protected IEnumerator RotinaSofrendoAtaqueArm()
        {           
            animacao.AnimacaoSofrendoAtqArm();            
            yield return StartCoroutine(animacao.EsperarAnimacao());           
            estadoAtual = EstadoJogador.ModoAtaque;
        }
        //-------------------------------------------------------------------------

    }
}