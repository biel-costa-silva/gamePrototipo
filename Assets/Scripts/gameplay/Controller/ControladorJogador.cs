using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;
using Assets.Scripts.Geral;
using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes;
using Assets.Scripts.View;
using Assets.Scripts.View.EntradaDados;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class ControladorJogador : MonoBehaviour
    {
        //model
        public Jogador jogador;
        //fisica do model
        protected FisicaJogador fisica;
        //views      
        protected ControladorAnim animacao;
        [SerializeField] protected VidaUIController vidaUI;
        //controles
        protected IControles controle;
        protected IInteracoes interagivelAtual;
        //variaveis de controle
        private bool estaSendoAlertado = false;
        private bool estaSendoDesarmado = false;
        protected bool invulneravel = false;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float duracaoInvulneravel = 1f;
        [SerializeField] private float intervaloPiscar = 0.1f;

        private EstadoJogador _estadoAtual = EstadoJogador.Parado;
        public EstadoJogador estadoAtual
        {
            get => _estadoAtual;
            protected set
            {
                if (_estadoAtual == value) return;
                _estadoAtual = value;
                OnEstadoAlterado?.Invoke(this, _estadoAtual);
            }
        }
        public event Action<ControladorJogador, EstadoJogador> OnEstadoAlterado;
        public event Action<ControladorJogador> OnMorreu;



        // --------------------------------------------------------------
        public void Awake()
        {
            fisica = GetComponent<FisicaJogador>();
            animacao = GetComponent<ControladorAnim>();
            controle = GetComponent<IControles>();

            //cria o model
            jogador = CriarJogador();
        }
        protected virtual Jogador CriarJogador()
        {
            return new Jogador();
        }

        // ----------------------------- Métodos de controle ---------------------------------
        public void ReceberGolpe(HitBox golpe)
        {
            if (golpe == null || golpe.consumida) return;
            ProcessarDano(golpe);
        }
        protected virtual void ProcessarDano(HitBox golpe)
        {
            if (invulneravel) return;//ignora golpe enquanto vulneravel

            jogador.ReceberDano(golpe.dano);
            vidaUI.Decrementar(golpe.dano);
            fisica.AplicarImpulsoGolpeRecebido(golpe);
            Debug.Log("Recebeu dano:" + golpe.dano);

            IniciarInvulnerabilidade();
        }

        protected void IniciarInvulnerabilidade()
        {
            StartCoroutine(RotinaInvulneravel());
        }       

        public void AplicarGolpe(int indice)
        {
            fisica.AplicarGolpe(this, jogador.GetDano(), indice);
        }

        public void MorrerForcado()
        {
            vidaUI.Decrementar(10);//gambiarra
            StopAllCoroutines();
            StartCoroutine(RotinaMorrendo());
        }

        public void Reviver()
        {
            jogador.RestaurarVida();
            vidaUI.RestaurarCheia();
            StopAllCoroutines();
            estadoAtual = EstadoJogador.Parado;
            animacao.AnimacaoPosTP();
            animacao.OnAnimacaoTerminou();
        }

        public void AlertarEntrarEmModoAtaque()
        {
            if (estaSendoAlertado) return;
            if (estadoAtual != EstadoJogador.Parado && estadoAtual != EstadoJogador.Andando) return;

            estaSendoAlertado = true;
            StartCoroutine(RotinaAlertaESaque());
        }
        public void AlertarEDesarmar()
        {
            if (estaSendoDesarmado) return;
            if (estadoAtual != EstadoJogador.ModoAtaque && estadoAtual != EstadoJogador.AndandoArmado) return; // só desarma se estiver parado e armado

            estaSendoDesarmado = true;
            StartCoroutine(RotinaDesarmeForcado());
        }

        // --------------------------------------------------------------

        protected virtual void Update()
        {
            if (GerenciadorPausa.JogoPausado) return;

            HitBox golpe = fisica.ConsumirGolpePendente();
            if (golpe != null && !golpe.Equals(null) && !golpe.consumida)
            {
                Debug.Log($"Controller processando golpe. Consumida: {golpe.consumida}");
                ProcessarDano(golpe);
            }

            interagivelAtual = fisica.GetInteragivelPendente();

            if (estadoAtual == EstadoJogador.Ocupado || estadoAtual == EstadoJogador.morto) return;

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
                if (controle.ComandoInteracao() && interagivelAtual != null && interagivelAtual.PodeSofrerInteracao())
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
                    fisica.Locomover(controle.ComandoMovimento(), jogador.GetVelocidade());
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
                    fisica.Locomover(controle.ComandoMovimento(), jogador.GetVelocidade());
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

        // #------------------------------ Rotinas - Disparo de Animações Diretas sem Interrupção -----------------------------#
        private IEnumerator RotinaInvulneravel()
        {
            invulneravel = true;
            float tempoPassado = 0f;

            while (tempoPassado < duracaoInvulneravel)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(intervaloPiscar);
                tempoPassado += intervaloPiscar;
            }
            spriteRenderer.enabled = true;
            invulneravel = false;
        }

        protected IEnumerator RotinaAlertaESaque()
        {
            estadoAtual = EstadoJogador.Ocupado;
            animacao.AnimacaoAlerta();

            yield return StartCoroutine(animacao.EsperarAnimacao());

            estaSendoAlertado = false;
            StartCoroutine(RotinaSacanadoArma());
        }

        protected virtual IEnumerator RotinaDesarmeForcado()
        {
            estadoAtual = EstadoJogador.Ocupado;

            yield return StartCoroutine(RotinaGuardandoArma()); // reaproveita a rotina existente, que já termina em Parado

            estaSendoDesarmado = false;
        }

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
            fisica.AplicarImpulsoAtaque(forcaAtq);
            animacao.AnimacaoAtacando();

            yield return StartCoroutine(animacao.EsperarAnimacao()); // aguarda o último frame
            estadoAtual = EstadoJogador.ModoAtaque;
        }

        protected IEnumerator RotinaMorrendo()
        {
            animacao.AnimacaoMorrendo();
            yield return StartCoroutine(animacao.EsperarAnimacao());
            estadoAtual = EstadoJogador.morto;
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
            if (jogador.EstaMorto())
            {
                OnMorreu?.Invoke(this);
                StartCoroutine(RotinaMorrendo());
            }
            else
            {
                yield return StartCoroutine(animacao.EsperarAnimacao());
                estadoAtual = EstadoJogador.ModoAtaque;
            }
        }
        //-------------------------------------------------------------------------

    }
}