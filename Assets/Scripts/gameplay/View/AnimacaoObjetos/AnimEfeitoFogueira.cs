using Assets.Scripts.Geral;
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

        public virtual void IniciarSom2D(string nome)
        {
            GerenciadorAudio.instancia.TocarAudio2D(nome);
        }

        public virtual void IniciarSom3D(string nome)
        {
            GerenciadorAudio.instancia.TocarAudio3D(nome, transform.position);
        }
    }
}
