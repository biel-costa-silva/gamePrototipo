using Assets.Scripts.Gameplay.Controller;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using Assets.Scripts.Gameplay.View.AnimacaoObjetos;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
{
    public class Totem : MonoBehaviour, IInteracoes
    {
        private AnimTotem animacao;
        private DestaqueInteracao destaque;
        private StatusQueryOptions statusTotem { get; set; } // Enums, adicionar posteriormente

        private bool jaAtivado = false;
        private HashSet<Jogador> jogadoresPresentes = new HashSet<Jogador>();

        public event Action OnAtivado;

        private void Awake()
        {
            animacao = GetComponent<AnimTotem>();
            if (animacao == null) animacao = gameObject.AddComponent<AnimTotem>();
            destaque = GetComponent<DestaqueInteracao>();   // novo
        }

        public void SofrerInteracao(Jogador jogador)
        {
            if (jaAtivado) return;
            if (jogadoresPresentes.Count < 2) return;

            jaAtivado = true;
            destaque.Esconder();

            animacao.AnimacaoAtivando();
            OnAtivado?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ControladorJogador contJogador = other.GetComponent<ControladorJogador>();
            if (contJogador != null) jogadoresPresentes.Add(contJogador.jogador);
            if (destaque != null && !jaAtivado) destaque.Mostrar();   // novo
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ControladorJogador contJogador = other.GetComponent<ControladorJogador>();
            if (contJogador != null) jogadoresPresentes.Remove(contJogador.jogador);
            if (destaque != null && jogadoresPresentes.Count == 0) destaque.Esconder();   // novo
        }

        public bool PodeSofrerInteracao()
        {
            return !jaAtivado && jogadoresPresentes.Count >= 2;
        }

        public void Resetar()
        {
            jaAtivado = false;
            jogadoresPresentes.Clear();
            animacao.AnimacaoDesativado();
        }
    }
}