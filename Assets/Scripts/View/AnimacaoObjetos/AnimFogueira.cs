using UnityEngine;
using System.Collections;

namespace Assets.Scripts.View.AnimacaoObjetos
{
    public class AnimFogueira : MonoBehaviour
    {
        Animator animator;        
        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public void animacaoAcendendo()
        {
            animator.SetTrigger("acender");
        }
    }
}