using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens.EnumsInimigos;
using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;
using Assets.Scripts.Gameplay.View.AnimacaoPersonagens;
using Assets.Scripts.Gameplay.View.EntradaDados;
using Assets.Scripts.Geral;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Controller
{
    public class ControladorGoblin : ControladorInimigo
    {
        public Goblin goblin;
        private FisicaGoblin fisicaGoblin;
        private AnimGoblin animGoblin;
        private CerebroGoblin cerebro;


        [Header("Esquiva / Invulnerabilidade")]
        [Tooltip("Tempo mínimo entre duas esquivas preventivas.")]
        [SerializeField] private float cooldownEsquiva = 2f;
        [Tooltip("Quanto tempo fica invulnerável a cada arrancada (frente ou trás).")]
        [SerializeField] private float duracaoInvulneravel = 0.25f;

        private float proximaEsquivaPermitida = 0f;

        protected override void Awake()
        {
            base.Awake();
            fisicaGoblin = fisica as FisicaGoblin;
            animGoblin = animacao as AnimGoblin;
            goblin = inimigo as Goblin;
            cerebro = comportamento as CerebroGoblin;            
        }

        private void Start()
        {
            OnEstadoAlterado += (c, estado) => Debug.Log($"Goblin -> {estado}");
        }

        protected override Inimigo CriarInimigo()
        {
            return new Goblin();
        }

        // ---------------- Update: esquiva tem prioridade sobre o resto ----------------

        protected override void Update()
        {
            if (GerenciadorPausa.JogoPausado) return;

            bool livre = estadoAtual == EstadoInimigo.Parado || estadoAtual == EstadoInimigo.Andando;
            if (livre && PodeEsquivar())
            {
                StartCoroutine(RotinaEsquivaPreventiva());
                return;
            }

            base.Update();
        }

        private bool PodeEsquivar()
        {
            return cerebro != null
                && Time.time >= proximaEsquivaPermitida
                && cerebro.JogadorVaiAtacar();
        }

        // ---------------- Eventos de animação (chamados pelo AnimGoblin) ----------------

        public void EventoImpulsoFrente()
        {
            fisicaGoblin.ImpulsoFrente();
            IniciarInvulnerabilidade(duracaoInvulneravel);
        }

        public void EventoImpulsoTras()
        {
            fisicaGoblin.ImpulsoTras();
            IniciarInvulnerabilidade(duracaoInvulneravel);
        }

        // ---------------- Dano ----------------

        protected override void AoReceberGolpe(HitBox golpe, int variacao)
        {
            // o empurrão do golpe já foi aplicado; a arrancada não pode sobrescrevê-lo
            fisicaGoblin.CancelarImpulso();
        }

        // ---------------- Rotinas ----------------

        // indice 0 = ataque 1 (combo corte/recuo/faca) | indice 1 = só a faca
        protected override IEnumerator RotinaAtacando(int indice)
        {
            if (indice == 0) yield return StartCoroutine(RotinaComboCorteEFaca());
            else yield return StartCoroutine(RotinaSoFaca());
        }

        private IEnumerator RotinaComboCorteEFaca()
        {
            estadoAtual = EstadoInimigo.Atacando;
            inimigo.LimparRepelido();
            fisica.Parar();
            OlharParaAlvo();

            // 1) arrancada para frente + corte
            animacao.AnimacaoAtacando(0);
            yield return StartCoroutine(EsperarAnimacaoOuRepelido());

            if (inimigo.foiRepelido)
            {
                yield return StartCoroutine(RotinaCaindo());
                yield break;
            }

            // 2) impulso para trás
            yield return StartCoroutine(RotinaEsquivar());

            // 3) faca (com outro impulso para trás)
            yield return StartCoroutine(RotinaLancarFaca());

            IniciarCooldownAtaque();
            estadoAtual = EstadoInimigo.Parado;
        }

        private IEnumerator RotinaSoFaca()
        {
            estadoAtual = EstadoInimigo.Atacando;
            fisica.Parar();

            yield return StartCoroutine(RotinaLancarFaca());

            IniciarCooldownAtaque();
            estadoAtual = EstadoInimigo.Parado;
        }

        private IEnumerator RotinaLancarFaca()
        {
            OlharParaAlvo();
            animacao.AnimacaoAtacando(1);
            yield return StartCoroutine(animacao.EsperarAnimacao());

            fisica.Parar();
            cerebro.ReiniciarIntervaloFaca();
        }

        private IEnumerator RotinaEsquivaPreventiva()
        {
            yield return StartCoroutine(RotinaEsquivar());
            estadoAtual = EstadoInimigo.Parado;
        }

        // Usada tanto na esquiva preventiva quanto no recuo do combo
        private IEnumerator RotinaEsquivar()
        {
            estadoAtual = EstadoInimigo.Ocupado;
            proximaEsquivaPermitida = Time.time + cooldownEsquiva;
            fisica.Parar();
            OlharParaAlvo();

            animGoblin.AnimacaoEsquivando();
            yield return StartCoroutine(animacao.EsperarAnimacao());

            fisica.Parar();
        }

        // Guerreiro repeliu o corte: cai no chão e se levanta (tudo dentro da animação "cair")
        private IEnumerator RotinaCaindo()
        {
            estadoAtual = EstadoInimigo.Ocupado;
            fisica.Parar();

            float direcao = Mathf.Sign(transform.position.x - comportamento.PosicaoAlvo().x);
            fisica.AplicarImpulsoRepulsao(direcao);

            animGoblin.AnimacaoCaindo();
            yield return StartCoroutine(animacao.EsperarAnimacao());

            inimigo.LimparRepelido();
            fisica.Parar();
            IniciarCooldownAtaque();
            estadoAtual = EstadoInimigo.Parado;
        }

        // ---------------- Auxiliares ----------------

        private IEnumerator EsperarAnimacaoOuRepelido()
        {
            while (!animacao.animacaoTerminou && !inimigo.foiRepelido)
                yield return null;
        }

        private void OlharParaAlvo()
        {
            fisica.VirarPara(comportamento.PosicaoAlvo().x - transform.position.x);
        }
    }
}