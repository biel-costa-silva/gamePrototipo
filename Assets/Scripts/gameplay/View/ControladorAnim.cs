using Assets.Scripts.Controller;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

namespace Assets.Scripts.View
{
    abstract public class ControladorAnim : MonoBehaviour
    {
        [SerializeField] private ControladorJogador controller;
        protected Animator animator;

        //variaveis de controle
        public bool animacaoTerminou = false;
        public bool novoAtaque = true;
        public int indiceAtaque { get; set; }

        void Awake()
        {
            animator = GetComponent<Animator>();
            if (controller == null)
            {
                controller = GetComponent<ControladorJogador>();
            }
        }

        public void ResetarTriggers()
        {
            animator.ResetTrigger("sacarArma");
            animator.ResetTrigger("guardarArma");
            animator.ResetTrigger("interagir");
            animator.ResetTrigger("atacar");
            animator.ResetTrigger("sofrerAtqArm");
            animator.ResetTrigger("sofrerAtqDesarm");
            animator.ResetTrigger("defender");
            animator.ResetTrigger("repelir");
            animator.ResetTrigger("sofrerAtqDefendendo");
        }

        //Parâmetros
        public void AnimacaoParado() => animator.SetBool("isAndando", false);
        public void AnimacaoAndando(bool parametro) => animator.SetBool("isAndando", parametro);
        public void AnimacaoParadoArmado() => animator.SetBool("isAndandoArm", false);
        public void AnimacaoAndandoArmado(bool parametro) => animator.SetBool("isAndandoArm", parametro);

        public void AnimacaoSacandoArma()
        {
            animacaoTerminou = false;
            animator.SetTrigger("sacarArma");
        }
        //
        public void AnimacaoGuardandoArma()
        {
            animacaoTerminou = false;
            animator.SetTrigger("guardarArma");
        }
        //
        public void AnimacaoInteragindo()
        {
            animacaoTerminou = false;
            animator.SetTrigger("interagir");
        }
        //
        public void AnimacaoAtacando()
        {
            animacaoTerminou = false;
            animator.SetTrigger("atacar");
        }
        //
        public void AnimacaoSofrendoAtqArm()
        {
            animacaoTerminou = false;
            animator.SetTrigger("sofrerAtqArm");
        }
        public void AnimacaoSofrendoAtqDesarm()
        {
            animacaoTerminou = false;
            animator.SetTrigger("sofrerAtqDesarm");
        }


        //Controle de EVENTOS
        public IEnumerator EsperarAnimacao()
        {
            while (!animacaoTerminou) yield return null;
        }

        public void ResetarAnimacao()
        {
            animacaoTerminou = false;
            novoAtaque = true;
        }

        public virtual void OnAnimacaoTerminou()
        {
            ResetarTriggers();
            animacaoTerminou = true;
        }

        //COMBOS!
        public void JanelaNovoAtq()
        {
            novoAtaque = false;
        }

        public void EventoAtaque()//frame do ataque
        {
            controller.AplicarGolpe(indiceAtaque);
        }

    }
}