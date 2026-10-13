using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.View.AnimacaoPersonagens
{
    public class AnimSlime : AnimInimigo
    {
        private FisicaSlime fisicaSlime;
        public bool noAr { get; set; }

        protected override void Awake()
        {
            base.Awake();
            fisicaSlime = GetComponent<FisicaSlime>();
        }

        //animationEvent do Clip de andar (o pulo)
        public void EventoVFXAndando() => fisicaSlime.VfxAndando();
        public void EventoVFXGoticulas() => fisicaSlime.VfxGoticulas();

        public void EventoDecolou() => noAr = true;
        public void EventoPousou() => noAr = false;

        public override void OnAnimacaoTerminou()
        {
            base.OnAnimacaoTerminou();
            noAr = false;
        }
    }
}