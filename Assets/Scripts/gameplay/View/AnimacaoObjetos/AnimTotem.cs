using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Gameplay.View.AnimacaoObjetos
{
	public class AnimTotem : MonoBehaviour
	{

        Animator animator;
        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public void animacaoAtivando()
        {
            animator.SetTrigger("ativar");
        }
    }
}