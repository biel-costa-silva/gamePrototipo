using UnityEngine;
using System.Collections;
using UnityEditor;
using Unity.VisualScripting;
using Assets.Scripts.Nucleo.Interfaces;
using Assets.Scripts.Visual.Animacoes.Mundo;

namespace Assets.Scripts.Jogabilidade.Mundo
{
	public class Fogueira : MonoBehaviour, IInteracoes
	{
        private AnimFogueira animacao;
        private int id { get; set; }
        private StatusQueryOptions statusFogueira { get; set; } // Enums, adicionar posteriormente

        private void Awake()
        {
            animacao = GetComponent<AnimFogueira>();

            if(animacao == null) animacao = gameObject.AddComponent<AnimFogueira>();
        }

        public void SofrerInteracao(Jogador jogador)
        {           
            animacao.animacaoAcendendo();
        }
        //Curar()

        //SalvarJogo()
    }
}