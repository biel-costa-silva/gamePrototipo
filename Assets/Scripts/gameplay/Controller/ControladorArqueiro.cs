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

        [Header("Ataque carregado")]
        [SerializeField] private float tempoParaCarregar = 0.5f;
        [SerializeField] private int indiceVfxCarregado = 0;

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
                arqueiro.SetVelocidade(arqueiro.GetVelocidadeBase() + 3);

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
            
            animArqueiro.AnimacaoPuxando();
            yield return StartCoroutine(animArqueiro.EsperarAnimacao());

            // 2. Soltou durante a animação de puxar (antes mesmo de carregar)? -> ataque padrão
            if (!controleArqueiro.ComandoAtaqueCarregando())
            {
                yield return StartCoroutine(ExecutarAtaquePadrao());
                yield break;
            }

            // 3. Já está em "Segurando" (automático). Agora cronometra a carga.
            float tempoSegurando = 0f;
            bool cargaPronta = false;

            while (controleArqueiro.ComandoAtaqueCarregando())
            {
                tempoSegurando += Time.deltaTime;

                if (!cargaPronta && tempoSegurando >= tempoParaCarregar)
                {
                    cargaPronta = true;
                    fisicaArqueiro.SpawnarVFX(indiceVfxCarregado); // sinal visual de "pronto"                    
                }

                yield return null;
            }

            // 4. Soltou o botão: decide o destino final
            if (cargaPronta)
            {
                animArqueiro.AnimacaoDispararCarregado();
                fisicaArqueiro.AplicarImpulsoAtaque(5);
                yield return StartCoroutine(animArqueiro.EsperarAnimacao());
                estadoAtual = EstadoJogador.ModoAtaque; // AtaqueCarregado -> ModoAtaque já é automático no Animator, isso só sincroniza o C#
            }
            else
            {
                yield return StartCoroutine(ExecutarAtaquePadrao());
            }
        }

        private IEnumerator ExecutarAtaquePadrao()
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