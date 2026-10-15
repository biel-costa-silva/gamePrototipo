using UnityEngine;
using UnityEditor;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Gameplay.View.AnimacaoObjetos;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
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

        public bool PodeSofrerInteracao()
        {
            throw new System.NotImplementedException();
        }
        //Curar()

        //SalvarJogo()
    }
}