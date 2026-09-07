using Assets.Scripts.Gameplay.View.AnimacaoObjetos;
using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.View.AnimacaoObjetos;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Objetos
{
    public class Totem : MonoBehaviour, IInteracoes
    {
        private AnimTotem animacao;        
        private StatusQueryOptions statusTotem { get; set; } // Enums, adicionar posteriormente

        private void Awake()
        {
            animacao = GetComponent<AnimTotem>();

            if (animacao == null) animacao = gameObject.AddComponent<AnimTotem>();
        }

        public void SofrerInteracao(Jogador jogador)
        {
            animacao.animacaoAtivando();
        }
        //travar camera()

        //sinal de spawnar inimigos()
    }
}