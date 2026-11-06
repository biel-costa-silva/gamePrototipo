using UnityEngine;

namespace Assets.Scripts.Gameplay.View.AnimacaoObjetos
{
    public class AnimEfeitoFogueira : MonoBehaviour
    {
        Animator animator;        
        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public void animacaoInspencionar()
        {
            animator.SetTrigger("inspencionar");
        }
        public void animacaoVoltar()
        {
            animator.SetTrigger("voltar");
        }
    }
}