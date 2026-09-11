using Assets.Scripts.Controller;
using Assets.Scripts.Gameplay.View.AnimacaoObjetos;
using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.View.AnimacaoObjetos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Objetos
{
    public class Totem : MonoBehaviour, IInteracoes
    {
        private AnimTotem animacao;               
        private StatusQueryOptions statusTotem { get; set; } // Enums, adicionar posteriormente

        private bool jaAtivado = false;
        private HashSet<Jogador> jogadoresPresentes = new HashSet<Jogador>();

        public event Action OnAtivado;

        private void Awake()
        {
            animacao = GetComponent<AnimTotem>();
            if (animacao == null) animacao = gameObject.AddComponent<AnimTotem>();
        }

        public void SofrerInteracao(Jogador jogador)
        {
            if (jaAtivado) return;
            if (jogadoresPresentes.Count < 2) return;

            jaAtivado = true;

            animacao.animacaoAtivando();
            OnAtivado?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ControladorJogador contJogador = other.GetComponent<ControladorJogador>();
            if (contJogador != null) jogadoresPresentes.Add(contJogador.jogador);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ControladorJogador contJogador = other.GetComponent<ControladorJogador>();
            if (contJogador != null) jogadoresPresentes.Remove(contJogador.jogador);
        }

        //travar camera()

        //sinal de spawnar inimigos()
    }
}