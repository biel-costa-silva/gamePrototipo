using Assets.Scripts.Entidades.Enums;
using Assets.Scripts.Nucleo.Interfaces;
using Assets.Scripts.Visoes.Animacoes;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;
using Unity;

public class Jogador : Personagem
{

    //variaveis internas Unity    
    public Camera cameraController;

    protected EstadoJogador estadoAtual = EstadoJogador.Parado;
    private IInteracoes interagivelAtual;


    //Controle de ESTADOS
    public void Update()// -------------------------------------------------------------------------------------------------------------- 1
    {

        if (estadoAtual == EstadoJogador.Ocupado) return; //Garante consistência, não roda o Update completo enquanto alguma coisa não ecerrar


        // ------------------------------ POSSIBILIDADES ENQUANTO PARADO ------------------------------- //

        if (estadoAtual == EstadoJogador.Parado)
        {
            velocidade = velocidadeBase;            
            animacao.AnimacaoParado();// looping fixo, sem inputs 

            if (sofreuAtaque)
            {
                estadoAtual = EstadoJogador.Ocupado;
                StartCoroutine(RotinaSofrendoAtaqueDesarm());
                sofreuAtaque = false;
                return;
            }

            if (controle.ComandoMovimento() != 0)
            {
                estadoAtual = EstadoJogador.Andando;
                return;//necessita para mudança de estados em looping
            }
            //
            if (controle.ComandoSaqueArma())
            {
                estadoAtual = EstadoJogador.Ocupado;

                StartCoroutine(RotinaSacanadoArma());
                return;
            }
            //
            if (controle.ComandoInteracao())
            {
                if (interagivelAtual != null)
                {
                    estadoAtual = EstadoJogador.Ocupado;

                    StartCoroutine(RotinaInteracao());
                    interagivelAtual.SofrerInteracao(this);
                    return;
                }// outros if para cada obejto interagivel                
            }
            /*
            if sofrer ataque, hitbox, troque para essa ainmação
            */
        }
        // --------------------------------------------------------------------------------------------- //

        // ------------------------------ POSSIBILIDADES ENQUANTO ANDANDO ------------------------------ //
        if (estadoAtual == EstadoJogador.Andando)
        {
            if (sofreuAtaque)
            {
                estadoAtual = EstadoJogador.Ocupado;
                StartCoroutine(RotinaSofrendoAtaqueDesarm());
                sofreuAtaque = false;
                return;
            }

            if (controle.ComandoMovimento() != 0)
            {
                Locomover(controle.ComandoMovimento());
                animacao.AnimacaoAndando(estadoAtual == EstadoJogador.Andando);//looping dinâmico, necessário mesclar animator com Estado real do jogador     
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
        // --------------------------------------------------------------------------------------------- //

        // ------------------------ POSSIBILIDADES ENQUANTO EM MODO DE ATAQUE -------------------------- //
        if (estadoAtual == EstadoJogador.ModoAtaque)
        {
            velocidade = velocidadeBase + 2;
            animacao.AnimacaoParadoArmado();

            if (sofreuAtaque)
            {
                estadoAtual = EstadoJogador.Ocupado;
                StartCoroutine(RotinaSofrendoAtaqueArm());
                sofreuAtaque = false;
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
        // --------------------------------------------------------------------------------------------- //

        // ----------------------- POSSIBILIDADES ENQUANTO CORRE EM MODO DE ATAQUE ---------------------- //
        if (estadoAtual == EstadoJogador.AndandoArmado)
        {
            if (sofreuAtaque)
            {
                estadoAtual = EstadoJogador.Ocupado;
                StartCoroutine(RotinaSofrendoAtaqueArm());
                sofreuAtaque = false;
                return;
            }

            if (controle.ComandoMovimento() != 0)
            {
                Locomover(controle.ComandoMovimento());
                animacao.AnimacaoAndandoArmado(estadoAtual == EstadoJogador.AndandoArmado);
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
        // --------------------------------------------------------------------------------------------- //


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
        Interagir();

        yield return StartCoroutine(animacao.EsperarAnimacao());

        estadoAtual = EstadoJogador.Parado;
    }
    //
    protected IEnumerator RotinaAtacando()
    {
        bool comboRegistrado = false;
        int forcaAtq = controle.ComandoAtaque();
        int contadorCombo = 0;
        animacao.indiceAtaque = 0;
        
        animacao.ResetarAnimacao();
        Atacar(forcaAtq);
        animacao.AnimacaoAtacando();

        yield return null;

        
        while (true)
        {
            if (animacao.novoAtaque)//janela aberta
            {
                if (contadorCombo <= 2 && controle.ComandoAtaque() > 0)//dano igual 0 significa que não atacou
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

    //------------------- Sofrendo Ataques: possibilidades --------------------
    protected IEnumerator RotinaSofrendoAtaqueDesarm()
    {
        animacao.AnimacaoSofrendoAtqDesarm();
        yield return StartCoroutine(animacao.EsperarAnimacao());
        estadoAtual = EstadoJogador.Parado;
    }
    protected IEnumerator RotinaSofrendoAtaqueArm()
    {       
        Debug.Log("Entrou RotinaSofrendoAtaqueArm");
        animacao.AnimacaoSofrendoAtqArm();
        Debug.Log("animacaoTerminou antes de esperar: " + animacao.animacaoTerminou);
        yield return StartCoroutine(animacao.EsperarAnimacao());
        Debug.Log("Saiu do EsperarAnimacao");
        estadoAtual = EstadoJogador.ModoAtaque;
        Debug.Log("Estado setado para ModoAtaque");
    }
    //-------------------------------------------------------------------------




    // #-------------------------------------------------------------------------------------------------------------------#


    public override void OnTriggerEnter2D(Collider2D other) //aciona se colidiu com algo
    {
        base.OnTriggerEnter2D (other);

        IInteracoes interagivel = other.GetComponent<IInteracoes>();
        if (interagivel != null)
        {
            interagivelAtual = interagivel;
            Debug.Log("possivel interagir com algo");
        }
    }

    public void OnTriggerExit2D(Collider2D other)//aciona quando saiu da colisao com algo
    {
        IInteracoes interagivel = other.GetComponent<IInteracoes>();
        if (interagivel != null)
        {
            interagivelAtual = null;

            Debug.Log("Fora da area de interacao");
        }
    }
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 




    //métodos da classe - Jogador
    public virtual bool Interagir() // Se comando for acionado, personagem fica invulneravel e imóvel.
    {
        animacao.AnimacaoInteragindo();
        return true;
    }
    public virtual void Reviver(bool podeReviver) // Se Morrer(método herdado) acontece recebe true e faça
    {

    }
    public virtual void SubirNivel(bool podeSubir)// Se troca acontece, retorne true e faça
    {

    }
}
