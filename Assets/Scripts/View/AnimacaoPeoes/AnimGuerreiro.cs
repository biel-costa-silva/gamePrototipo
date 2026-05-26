using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Scripts.View.AnimacaoPeoes
{
    public class AnimGuerreiro : ControladorAnim
    {

        //variaveis de controle
        public bool estaDefendendo = false;
        public bool estaRepelindo = false;

        // ----------------  Métodos de acionamento ------------------


        // ----------------------- Método de animações especificas do guerreiro -------------------- #

        public void AnimacaoDefendendo()
        {
            animacaoTerminou = false;
            animator.SetTrigger("defender");
        }
        public void AnimacaoSofrendoAtqDef()
        {

            animacaoTerminou = false;
            animator.SetTrigger("sofrerAtqDefendendo");
        }
        public void AnimacaoRepelindo()
        {

            animacaoTerminou = false;
            animator.SetTrigger("repelir");
        }
        public void AnimacaoRecebeChoqueAtqs()
        {
            animacaoTerminou = false;
            animator.SetTrigger("receberChoque");
        }

        // ------------------------------------------------------------------------------------ #

        // EVENTOS ESPECIFICOS DO GUERREIRO!! --------------------------------------------------

        public void EventoRepelir()
        {
            estaRepelindo = true;
        }
        public void EventoDefesa()
        {
            estaRepelindo = false;
            estaDefendendo = true;
        }
        public void FimDefesa()
        {
            estaDefendendo = false;
        }
        public override void OnAnimacaoTerminou()
        {
            base.OnAnimacaoTerminou();
            estaDefendendo = false;
            estaRepelindo = false;
        }


    }
}
