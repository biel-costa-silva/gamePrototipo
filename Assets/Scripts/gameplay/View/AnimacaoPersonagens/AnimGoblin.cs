using Assets.Scripts.Gameplay.Controller;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.View.AnimacaoPersonagens
{
    public class AnimGoblin : AnimInimigo {
        private ControladorGoblin controladorGoblin;

        protected override void Awake()
        {
            base.Awake();
            controladorGoblin = GetComponent<ControladorGoblin>();
        }

        // ---------------- Disparo de animações ----------------

        public void AnimacaoEsquivando()
        {
            animacaoTerminou = false;
            animator.SetTrigger("esquivar");
        }

        public void AnimacaoCaindo()
        {
            animacaoTerminou = false;
            animator.SetTrigger("cair");
        }

        public override void ResetarTriggers()
        {
            base.ResetarTriggers();
            animator.ResetTrigger("esquivar");
            animator.ResetTrigger("cair");
        }

        // ---------------- Animation Events (colocados nos clips) ----------------

        public void EventoImpulsoFrente() => controladorGoblin.EventoImpulsoFrente();

        public void EventoImpulsoTras() => controladorGoblin.EventoImpulsoTras();
    }
}