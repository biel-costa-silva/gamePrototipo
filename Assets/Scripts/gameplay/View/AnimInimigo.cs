using Assets.Scripts.Gameplay.Controller;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.View
{
    public abstract class AnimInimigo : MonoBehaviour
    {

        [SerializeField] private ControladorInimigo controller;
        protected Animator animator;

        public bool animacaoTerminou = false;
        public int indiceAtaque { get; set; }

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            if (controller == null) controller = GetComponent<ControladorInimigo>();
        }

        public void AnimacaoParado() => animator.SetBool("isAndando", false);
        public void AnimacaoAndando() => animator.SetBool("isAndando", true);

        public void AnimacaoAtacando(int indice)
        {
            animacaoTerminou = false;
            indiceAtaque = indice;
            animator.SetInteger("indiceAtaque", indice);
            animator.SetTrigger("atacar");
        }

        public void AnimacaoSofrendoDano(int variacao)
        {
            animacaoTerminou = false;
            animator.SetInteger("indiceDano", variacao);
            animator.SetTrigger("sofrerDano");
        }

        public void AnimacaoMorrendo()
        {
            animacaoTerminou = false;
            animator.SetTrigger("morrer");
        }

        public virtual void ResetarTriggers()
        {
            animator.ResetTrigger("atacar");
            animator.ResetTrigger("sofrerDano");
            animator.ResetTrigger("morrer");
        }

        public IEnumerator EsperarAnimacao()
        {
            while (!animacaoTerminou) yield return null;
        }

        public virtual void OnAnimacaoTerminou()
        {
            ResetarTriggers();
            animacaoTerminou = true;
        }

        public void EventoAtaque() => controller.AplicarGolpe(indiceAtaque);
    }
}