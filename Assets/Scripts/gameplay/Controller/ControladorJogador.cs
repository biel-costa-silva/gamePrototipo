using Assets.Scripts.Gameplay.Model.Entidades.Objetos;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens.EnumsPersonagens;
using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;
using Assets.Scripts.Gameplay.View;
using Assets.Scripts.Gameplay.View.EntradaDados;
using Assets.Scripts.Geral;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


namespace Assets.Scripts.Gameplay.Controller
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
        protected MonoBehaviour controleBehaviour;
        protected IInteracoes interagivelAtual;


        //variaveis de controle
        private bool estaSendoAlertado = false;
        private bool estaSendoDesarmado = false;
        protected bool invulneravel = false;


        [SerializeField] private SpriteRenderer spriteRenderer;
        private IconeInteracaoJogador iconeInteracao;
        [SerializeField] private float duracaoInvulneravel = 1f;
        [SerializeField] private float intervaloTransicao = 0.15f;
        [SerializeField] private Color corEscura = new Color(0.35f, 0.35f, 0.35f, 1f);
        private Color corBase;


        protected bool estaArmado = false;
        private EstadoJogador _estadoAtual = EstadoJogador.Parado;
        public EstadoJogador estadoAtual
        {
            get => _estadoAtual;
            protected set
            {
                if (_estadoAtual == value) return;
                _estadoAtual = value;
                AtualizarArmado(value);
                AtualizarAnimacaoDeAndar(value);
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
            controleBehaviour = controle as MonoBehaviour;
            iconeInteracao = GetComponentInChildren<IconeInteracaoJogador>();

            //cria o model
            jogador = CriarJogador();
            if (spriteRenderer != null) corBase = spriteRenderer.color;

        }
        protected void AtivarControles(bool ativo)
        {
            if (controleBehaviour != null)
                controleBehaviour.enabled = ativo;
        }
        protected virtual Jogador CriarJogador()
        {
            return new Jogador();
        }

        // ----------------------------- Métodos de controle ---------------------------------
        private void AtualizarArmado(EstadoJogador estado)
        {
            switch (estado)
            {
                case EstadoJogador.ModoAtaque:
                case EstadoJogador.AndandoArmado:
                case EstadoJogador.Agachado:
                case EstadoJogador.Atacando:
                    estaArmado = true; break;
                case EstadoJogador.Parado:
                case EstadoJogador.Andando:
                    estaArmado = false; break;                    
            }
        }
        protected void DefinirEstado(EstadoJogador novoEstado)
        {
            estadoAtual = novoEstado;
        }

        public void ReceberGolpe(HitBox golpe)
        {
            if (golpe == null || golpe.consumida) return;
            ProcessarDano(golpe);
        }
        protected virtual void ProcessarDano(HitBox golpe)
        {            
            if (invulneravel) return;//ignora golpe enquanto vulneravel

            bool deCosta = fisica.EstaDeCostas(golpe.direcao);

            if (deCosta) fisica.VirarParaLadoDoGolpe(golpe.direcao);

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

        private void AtualizarAnimacaoDeAndar(EstadoJogador estado)
        {
            if (animacao == null) return;

            switch (estado)
            {
                case EstadoJogador.Parado:
                case EstadoJogador.ModoAtaque:
                    animacao.AnimacaoParado();           // isAndando = false
                    animacao.AnimacaoParadoArmado();     // isAndandoArm = false
                    break;
                case EstadoJogador.Andando:
                    animacao.AnimacaoParadoArmado();     // não pode estar "andando armado" também
                    break;
                case EstadoJogador.AndandoArmado:
                    animacao.AnimacaoParado();
                    break;
            }
        }
        public void MorrerForcado()
        {
            vidaUI.Decrementar(10);//gambiarra
            StopAllCoroutines();
            RestaurarEstadoVisualEFlags();           
            StartCoroutine(RotinaMorrendo());
        }

        public void Reviver()
        {
            jogador.RestaurarVida();
            vidaUI.RestaurarCheia();
            StopAllCoroutines();
            RestaurarEstadoVisualEFlags();
            estaArmado = false;
            estadoAtual = EstadoJogador.Parado;
            animacao.AnimacaoPosTP();
            animacao.OnAnimacaoTerminou();
        }

        private void RestaurarEstadoVisualEFlags()
        {
            if (spriteRenderer != null) spriteRenderer.color = corBase;
            invulneravel = false;
            estaSendoAlertado = false;
            estaSendoDesarmado = false;
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
        private void TravarPorMenu()
        {
            fisica.Parar();   // não continua deslizando

            if (estadoAtual == EstadoJogador.Andando)
            {
                DefinirEstado(EstadoJogador.Parado);
                animacao.AnimacaoParado();
            }
            else if (estadoAtual == EstadoJogador.AndandoArmado)
            {
                DefinirEstado(EstadoJogador.ModoAtaque);
                animacao.AnimacaoParadoArmado();
            }
            // Parado e ModoAtaque já estão parados
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

            if (estadoAtual == EstadoJogador.Ocupado || estadoAtual == EstadoJogador.Morto) return;
            
            if (GerenciadorInteracoes.menuAberto)
            {
                TravarPorMenu();
                return;
            }




            // --- SENTADO ---
            if (estadoAtual == EstadoJogador.Sentado)
            {
                //limita o personagem à interações com o canva na fogueira
            }
            // --- PARADO ---
            if (estadoAtual == EstadoJogador.Parado)
            {
                jogador.SetVelocidade(jogador.GetVelocidadeBase());
                animacao.AnimacaoParado();

                if (jogador.sofreuAtaque)
                {
                    jogador.LimparFlagAtaque();
                    DefinirEstado(EstadoJogador.Ocupado);
                    StartCoroutine(RotinaSofrendoAtaqueDesarm());
                    return;
                }
                if (controle.ComandoMovimento() != 0)
                {
                    DefinirEstado(EstadoJogador.Andando);
                    return;
                }
                if (controle.ComandoSaqueArma())
                {
                    DefinirEstado(EstadoJogador.Ocupado);
                    StartCoroutine(RotinaSacanadoArma());
                    return;
                }
                if (controle.ComandoInteracao() && interagivelAtual != null && interagivelAtual.PodeSofrerInteracao())
                {                    
                    DefinirEstado(EstadoJogador.Ocupado);
                    StartCoroutine(RotinaInteracao());
                    
                    return;
                }
                if (controle.ComandoInteracao() && interagivelAtual != null && !interagivelAtual.PodeSofrerInteracao())
                {
                    Debug.Log("[Interacao] tentou interagir sem poder");//aciona movimento de negação do icone
                    if (iconeInteracao != null) iconeInteracao.Tremer();
                }
            }

            // --- ANDANDO ---
            if (estadoAtual == EstadoJogador.Andando)
            {
                if (jogador.sofreuAtaque)
                {
                    jogador.LimparFlagAtaque();
                    DefinirEstado(EstadoJogador.Ocupado);
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
                    DefinirEstado(EstadoJogador.Parado);
                }
                if (controle.ComandoSaqueArma())
                {
                    DefinirEstado(EstadoJogador.Ocupado);
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
                    DefinirEstado(EstadoJogador.Ocupado);
                    StartCoroutine(RotinaSofrendoAtaqueArm());
                    return;
                }
                if (controle.ComandoMovimento() != 0)
                {
                    DefinirEstado(EstadoJogador.AndandoArmado);
                    return;
                }
                if (controle.ComandoSaqueArma())
                {
                    DefinirEstado(EstadoJogador.Ocupado);
                    StartCoroutine(RotinaGuardandoArma());
                    return;
                }
                if (controle.ComandoAtaque() > 0)
                {
                    DefinirEstado(EstadoJogador.Ocupado);
                    StartCoroutine(RotinaAtacando());
                    return;
                }
                if (controle.ComandoInteracao() && interagivelAtual != null && interagivelAtual.PodeSofrerInteracao())
                {
                    DefinirEstado(EstadoJogador.Ocupado);
                    StartCoroutine(RotinaInteracao());

                    return;
                }
                if (controle.ComandoInteracao() && interagivelAtual != null && !interagivelAtual.PodeSofrerInteracao())
                {
                    Debug.Log("[Interacao] tentou interagir sem poder");//aciona movimento de negação do icone
                    if (iconeInteracao != null) iconeInteracao.Tremer();
                }
            }

            // --- ANDANDO ARMADO ---
            if (estadoAtual == EstadoJogador.AndandoArmado)
            {
                if (jogador.sofreuAtaque)
                {
                    jogador.LimparFlagAtaque();
                    DefinirEstado(EstadoJogador.Ocupado);
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
                    DefinirEstado(EstadoJogador.ModoAtaque);
                }
                if (controle.ComandoSaqueArma())
                {
                    DefinirEstado(EstadoJogador.Ocupado);
                    StartCoroutine(RotinaGuardandoArma());
                    return;
                }
                if (controle.ComandoAtaque() > 0)
                {
                    DefinirEstado(EstadoJogador.Ocupado);
                    StartCoroutine(RotinaAtacando());
                    return;
                }
            }
        }

        // #------------------------------ Rotinas - Disparo de Animações Diretas sem Interrupção -----------------------------#
        private IEnumerator RotinaInvulneravel()
        {
            invulneravel = true;
            Color corOriginal = corBase;

            float periodo = intervaloTransicao * 2f; // ida + volta
            float duracaoReal = Mathf.Ceil(duracaoInvulneravel / periodo) * periodo;
            float inicio = Time.time;

            while (Time.time - inicio < duracaoReal)
            {
                float t = Mathf.PingPong((Time.time - inicio) / intervaloTransicao, 1f);
                t = Mathf.SmoothStep(0f, 1f, t);
                spriteRenderer.color = Color.Lerp(corOriginal, corEscura, t);
                               
                yield return null;
            }

            spriteRenderer.color = corOriginal;
            invulneravel = false;
        }

        protected IEnumerator RotinaAlertaESaque()
        {
            DefinirEstado(EstadoJogador.Ocupado);
            animacao.AnimacaoAlerta();

            yield return StartCoroutine(animacao.EsperarAnimacao());

            estaSendoAlertado = false;
            StartCoroutine(RotinaSacanadoArma());
        }

        protected virtual IEnumerator RotinaDesarmeForcado()
        {
            DefinirEstado(EstadoJogador.Ocupado);
            fisica.Parar();
            animacao.AnimacaoInterrogado();

            yield return StartCoroutine(animacao.EsperarAnimacao()); // reaproveita a rotina existente, que já termina em Parado

            estaSendoDesarmado = false;
            StartCoroutine (RotinaGuardandoArma());
        }

        protected IEnumerator RotinaSacanadoArma()
        {
            animacao.AnimacaoSacandoArma();
            yield return StartCoroutine(animacao.EsperarAnimacao());
            DefinirEstado(EstadoJogador.ModoAtaque);
        }
        //
        protected IEnumerator RotinaGuardandoArma()
        {
            animacao.AnimacaoGuardandoArma();
            yield return StartCoroutine(animacao.EsperarAnimacao());
            DefinirEstado(EstadoJogador.Parado);
        }
        //
        protected IEnumerator RotinaInteracao()//especificar o tipo da interação
        {
            IInteracoes alvo = interagivelAtual;

            if (estaArmado)
            {
                animacao.AnimacaoGuardandoArma();
                yield return StartCoroutine(animacao.EsperarAnimacao());
                DefinirEstado(EstadoJogador.Parado);
                animacao.AnimacaoForcarParado();
                DefinirEstado(EstadoJogador.Ocupado);
            }

            if(alvo is Fogueira)
            {
                animacao.AnimacaoInteragindoFogueira();
                alvo.SofrerInteracao(jogador);
                yield return StartCoroutine(animacao.EsperarAnimacao()); //talvez não sirva nesse caso
                DefinirEstado(EstadoJogador.Sentado);

                yield break;
            }
            if (alvo is Item)
            {
                animacao.AnimacaoInteragindoItem();
                alvo.SofrerInteracao(jogador);
                yield return StartCoroutine(animacao.EsperarAnimacao());
                DefinirEstado(EstadoJogador.Parado);

                yield break;
            }
            if(alvo is Totem)
            {
                GerenciadorInteracoes.instancia.PedirConfirmacao();

                yield return new WaitUntil(() =>
                    GerenciadorInteracoes.resposta != RespostaConfirmacao.Aguardando
                    || GerenciadorPartida.jogoAcabou);

                if (GerenciadorInteracoes.resposta == RespostaConfirmacao.Sim)
                {
                    alvo.SofrerInteracao(jogador);
                }                
                DefinirEstado(EstadoJogador.Parado);
            }            
        }
        //
        protected virtual IEnumerator RotinaAtacando()
        {
            int forcaAtq = controle.ComandoAtaque();

            animacao.ResetarAnimacao();
            fisica.AplicarImpulsoAtaque(forcaAtq);
            animacao.AnimacaoAtacando();

            yield return StartCoroutine(animacao.EsperarAnimacao()); // aguarda o último frame
            DefinirEstado(EstadoJogador.ModoAtaque);
        }

        protected IEnumerator RotinaMorrendo()
        {
            DefinirEstado(EstadoJogador.Morto);
            animacao.ResetarTriggers();
            animacao.AnimacaoParado();      
            animacao.AnimacaoParadoArmado();

            if (estaArmado) animacao.AnimacaoMorrendo();
            else animacao.AnimacaoMorrendo2();

            yield break;
        }

        //------------------- Sofrendo Ataques: possibilidades --------------------
        protected IEnumerator RotinaSofrendoAtaqueDesarm()
        {
            if (jogador.EstaMorto())
            {
                estaArmado = false;
                OnMorreu?.Invoke(this);
                StartCoroutine(RotinaMorrendo());
                yield break;
            }

            animacao.AnimacaoSofrendoAtqDesarm();         
            yield return StartCoroutine(animacao.EsperarAnimacao());
            DefinirEstado(EstadoJogador.Parado);
            
        }
        protected IEnumerator RotinaSofrendoAtaqueArm()
        {
            if (jogador.EstaMorto())
            {
                estaArmado = true;
                OnMorreu?.Invoke(this);
                StartCoroutine(RotinaMorrendo());
                yield break;
            }

            animacao.AnimacaoSofrendoAtqArm();
            yield return StartCoroutine(animacao.EsperarAnimacao());
            DefinirEstado(EstadoJogador.ModoAtaque);
        }
        //-------------------------------------------------------------------------

    }
}