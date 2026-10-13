using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.View.AnimacaoPersonagens
{
    public class AnimSlime : AnimInimigo
    {
        public bool noAr { get; set; }

        //animationEvent do Clip de andar (o pulo)

        public void EventoDecolou() => noAr = true;
        public void EventoPousou() => noAr = false;

        public override void OnAnimacaoTerminou()
        {
            base.OnAnimacaoTerminou();
            noAr = false;
        }
    }
}