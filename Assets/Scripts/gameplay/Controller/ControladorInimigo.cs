using Assets.Scripts.Controller;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens.EnumsInimigos;
using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;
using Assets.Scripts.Gameplay.View;
using Assets.Scripts.Gameplay.View.EntradaDados;
using Assets.Scripts.Geral;
using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes;
using Assets.Scripts.View;
using Assets.Scripts.View.EntradaDados;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Controller
{
    public class ControladorInimigo : MonoBehaviour
    {

        public Inimigo inimigo;
        //fisica do model
        protected FisicaInimigo fisica;
        //views      
        protected AnimInimigo animacao;
        //controles
        protected IComportamento comportamento;


        [Header("Dano por toque")]
        [SerializeField] private int danoContato = 1;
        [SerializeField] private float intervaloDanoContato = 1f;
        private readonly Dictionary<ControladorJogador, float> proximoToque = new Dictionary<ControladorJogador, float>();


        [SerializeField] private float cooldownAtaque = 1.5f;
        [SerializeField] private int quantidadeAnimacoesDano = 2;
        private float proximoAtaquePermitido = 0f;

        private EstadoInimigo _estadoAtual = EstadoInimigo.Parado;
        public EstadoInimigo estadoAtual
        {
            get => _estadoAtual;
            protected set
            {
                if (_estadoAtual == value) return;
                _estadoAtual = value;
                OnEstadoAlterado?.Invoke(this, _estadoAtual);
            }
        }
        public static readonly List<ControladorInimigo> Ativos = new List<ControladorInimigo>();
        protected virtual void OnEnable() => Ativos.Add(this);
        protected virtual void OnDisable() => Ativos.Remove(this);
        public event Action<ControladorInimigo, EstadoInimigo> OnEstadoAlterado;
        public event Action<ControladorInimigo> OnMorreu;


        private void OnCollisionStay2D(Collision2D col)
        {
            if (estadoAtual == EstadoInimigo.Morto) return;

            ControladorJogador jogador = col.collider.GetComponentInParent<ControladorJogador>();
            if (jogador == null || jogador.estadoAtual == EstadoJogador.Morto) return;

            if (proximoToque.TryGetValue(jogador, out float liberaEm) && Time.time < liberaEm) return;
            proximoToque[jogador] = Time.time + intervaloDanoContato;

            float direcao = Mathf.Sign(jogador.transform.position.x - transform.position.x);
            fisica.EnviarGolpeDeContato(this, danoContato, direcao, jogador.GetComponent<FisicaPersonagem>());
        }

        // --------------------------------------------------------------
        protected virtual void Awake()
        {
            fisica = GetComponent<FisicaInimigo>();
            animacao = GetComponent<AnimInimigo>();
            comportamento = GetComponent<IComportamento>();
            inimigo = CriarInimigo();
        }
        protected virtual Inimigo CriarInimigo()
        {
            return new Inimigo();
        }



        // ----------------------------- Métodos de controle ---------------------------------
        

        // Update is called once per frame
        protected virtual void Update()
        {
            if (GerenciadorPausa.JogoPausado) return;

            HitBox golpe = fisica.ConsumirGolpePendente();
            if (golpe != null && !golpe.Equals(null)){
                ReceberGolpe(golpe);
            }

            if (estadoAtual == EstadoInimigo.Ocupado ||
                estadoAtual == EstadoInimigo.Atacando ||
                estadoAtual == EstadoInimigo.Morto) return;

            // --- PARADO ---
            if (estadoAtual == EstadoInimigo.Parado)
            {
                animacao.AnimacaoParado();

                if (TentarAtacar()) return;
                if (comportamento.ComandoMovimento() != 0)
                {
                    estadoAtual = EstadoInimigo.Andando;
                }
                return;

            }

            // --- ANDANDO ---
            if (estadoAtual == EstadoInimigo.Andando)
            {
                if (TentarAtacar()) return;

                float direcao = comportamento.ComandoMovimento();
                if (direcao != 0)
                {
                    fisica.Locomover(direcao, inimigo.GetVelocidade());
                    animacao.AnimacaoAndando();
                }
                else
                {
                    fisica.Parar();
                    estadoAtual = EstadoInimigo.Parado;
                }
            }
        }
        private bool TentarAtacar()
        {
            if (Time.time < proximoAtaquePermitido) return false;

            int ataque = comportamento.ComandoAtaque();
            if (ataque <= 0) return false;

            StartCoroutine(RotinaAtacando(ataque - 1));
            return true;
        }

        public void ReceberGolpe(HitBox golpe)
        {
            if (golpe == null || golpe.consumida) return;
            ProcessarDano(golpe);
        }

        protected virtual int EscolherVariacaoDano() => UnityEngine.Random.Range(0, quantidadeAnimacoesDano);

        protected virtual void ProcessarDano(HitBox golpe)
        {
            if (estadoAtual == EstadoInimigo.Morto) return;
            if (golpe.origemEhInimigo) return; // inimigos não se ferem entre si

            int variacao = EscolherVariacaoDano();

            if (fisica.EstaDeCostas(golpe.direcao)) fisica.VirarParaLadoDoGolpe(golpe.direcao);

            inimigo.ReceberDano(golpe.dano);
            fisica.AplicarImpulsoGolpeRecebido(golpe);
            AoReceberGolpe(golpe, variacao);
            Debug.Log("Inimigo recebeu dano: " + golpe.dano);

            StopAllCoroutines();          // cancela ataque/qualquer rotina em andamento
            animacao.ResetarTriggers();

            if (inimigo.EstaMorto()) StartCoroutine(RotinaMorrendo());
            else StartCoroutine(RotinaSofrendoDano(variacao));
        }
        protected virtual void AoReceberGolpe(HitBox golpe, int variacao) { }

        public virtual void AplicarGolpe(int indice)
        {
            fisica.AplicarGolpe(this, inimigo.GetDano(), indice);
        }

        // ----------- ROTINAS -----------
        protected virtual IEnumerator RotinaAtacando(int indice)
        {
            estadoAtual = EstadoInimigo.Atacando;
            fisica.Parar();
            fisica.VirarPara(comportamento.PosicaoAlvo().x - transform.position.x);

            animacao.AnimacaoAtacando(indice);
            yield return StartCoroutine(animacao.EsperarAnimacao());

            proximoAtaquePermitido = Time.time + cooldownAtaque;
            estadoAtual = EstadoInimigo.Parado;
        }

        protected virtual IEnumerator RotinaSofrendoDano(int varicao)
        {
            estadoAtual = EstadoInimigo.Ocupado;
            animacao.AnimacaoSofrendoDano(varicao);
            yield return StartCoroutine(animacao.EsperarAnimacao());

            fisica.Parar();
            estadoAtual = EstadoInimigo.Parado;
        }

        protected virtual IEnumerator RotinaMorrendo()
        {
            estadoAtual = EstadoInimigo.Morto;
            animacao.AnimacaoMorrendo();

            yield return StartCoroutine(animacao.EsperarAnimacao());

            OnMorreu?.Invoke(this);
            Destroy(gameObject);
        }
    }
}