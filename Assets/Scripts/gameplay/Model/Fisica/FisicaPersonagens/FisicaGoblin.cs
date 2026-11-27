using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public class FisicaGoblin : FisicaInimigo
    {
        [Header("Impulsos (arrancadas)")]
        [Tooltip("Velocidade horizontal durante a arrancada para frente (ataque corpo a corpo).")]
        [SerializeField] private float velocidadeImpulsoFrente = 9f;
        [Tooltip("Velocidade horizontal durante a arrancada para trás (esquiva e arremesso da faca).")]
        [SerializeField] private float velocidadeImpulsoTras = 9f;

        private Coroutine impulsoAtual;

        // ---------------- Impulsos ----------------

        public void ImpulsoFrente() => Impulsar(GetDirecao(), velocidadeImpulsoFrente);

        public void ImpulsoTras() => Impulsar(-GetDirecao(), velocidadeImpulsoTras);

        private void Impulsar(float direcao, float velocidade)
        {
            CancelarImpulso();
            impulsoAtual = StartCoroutine(RotinaImpulso(direcao, velocidade));
        }

        // Interrompe a arrancada sem zerar a velocidade (útil quando um golpe acabou de empurrar o goblin)
        public void CancelarImpulso()
        {
            if (impulsoAtual == null) return;
            StopCoroutine(impulsoAtual);
            impulsoAtual = null;
        }

        private IEnumerator RotinaImpulso(float direcao, float velocidade)
        {            
            rb.AddForce(new Vector2(direcao * velocidade, 0), ForceMode2D.Impulse);
            yield return null;
            
            impulsoAtual = null;
        }

        // Mesmo ajuste do Slime: o offset do collider acompanha para onde o goblin olha
        protected override void CorrigirColliderFlip()
        {
            float x = GetDirecao() < 0 ? -offsetXBase : offsetXBase;
            boxColliderP1.offset = new Vector2(x, boxColliderP1.offset.y);
        }
    }
}