using Assets.Scripts.Entidades.Enums;
using Assets.Scripts.Personagens.Guerreiro;
using Assets.Scripts.Visual.Animacoes.Personagens;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Personagens.Arqueiro
{
	public class Arqueiro : Jogador
	{
        private AnimArqueiro animacaoArqueiro;
        private ComandosArqueiro controleArqueiro;
        private void Awake()
        {
            //força a adição dos componentes comandosArqueiro e AnimArqueiro no gameObject
            base.Awake();
            animacaoArqueiro = animacao as AnimArqueiro;
            controleArqueiro = controle as ComandosArqueiro;

        }
        void Start()
		{
            estadoAtual = base.estadoAtual;

            nome = "Arqueiro";
            vida = 4;
            dano = 3;
            defesa = 0; // quando acionada aumenta para a quantidade do nivel atual
            velocidade = 5; // muda se estiver em modo de ataque 
            velocidadeBase = velocidade;
        }


		// precisa chamar o funcionamento base.Update da classe jogador.
		// precisa existir para implementar funcinalidade especifica da classe.
		void Update()
		{
			base.Update();
		}

        protected override IEnumerator RotinaAtacando()
        {
            bool comboRegistrado = false;
            int forcaAtq = controle.ComandoAtaque();
            int contadorCombo = 0;
            animacao.indiceAtaque = 0;

            animacao.ResetarAnimacao();
            Atacar(forcaAtq);
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
                    if (sofreuAtaque)
                    {
                        sofreuAtaque = false;
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
                        Atacar(forcaAtq);
                        animacao.AnimacaoAtacando();
                        yield return null;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            yield return StartCoroutine(animacao.EsperarAnimacao()); // aguarda o último frame
            estadoAtual = EstadoJogador.ModoAtaque;
        }
       
    }
}