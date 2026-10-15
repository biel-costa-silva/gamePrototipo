using UnityEngine;

namespace Assets.Scripts.Gameplay.View.AnimacaoObjetos
{
	public class AnimTotem : MonoBehaviour
	{

        Animator animator;
        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public void AnimacaoAtivando()
        {
            animator.SetTrigger("ativar");
        }
        public void AnimacaoDesativado()
        {
            animator.Play("totemDesativado", 0, 0f);
        }
    }
}