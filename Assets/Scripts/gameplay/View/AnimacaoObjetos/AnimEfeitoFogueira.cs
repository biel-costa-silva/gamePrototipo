using UnityEngine;

namespace Assets.Scripts.Gameplay.View.AnimacaoObjetos
{
    public class AnimEfeitoFogueira : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void animacaoInspencionar()
        {
            if (animator == null) return;
            animator.ResetTrigger("voltar");
            animator.SetTrigger("inspencionar");
        }

        public void animacaoVoltar()
        {
            if (animator == null) return;
            animator.ResetTrigger("inspencionar");
            animator.SetTrigger("voltar");
        }
    }
}
