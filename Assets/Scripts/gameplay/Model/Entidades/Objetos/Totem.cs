using Assets.Scripts.Gameplay.Controller;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using Assets.Scripts.Gameplay.View.AnimacaoObjetos;
using Assets.Scripts.Geral;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
{
    public class Totem : MonoBehaviour, IInteracoes
    {
        string IInteracoes.nome { get; set; } = "totem";
        private AnimTotem animacao;
        private DestaqueInteracao destaque;
        private BoxCollider2D collider;
                
        private readonly HashSet<ControladorJogador> presentes = new HashSet<ControladorJogador>();
        public event Action OnAtivado;
        private bool jaAtivado = false;


        private void Awake()
        {
            animacao = GetComponent<AnimTotem>();
            if (animacao == null) animacao = gameObject.AddComponent<AnimTotem>();
            destaque = GetComponent<DestaqueInteracao>();   // novo
            collider = GetComponent<BoxCollider2D>();
        }


        public void SofrerInteracao(Jogador jogador)
        {
            if (jaAtivado) return;            

            jaAtivado = true;
            DefinirIconesDeTodos(EstadoIconeInteracao.Nenhum);
            if (destaque != null) destaque.Esconder();

            animacao.AnimacaoAtivando();

            collider.enabled = false;
            OnAtivado?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ControladorJogador c = other.GetComponent<ControladorJogador>();
            if (c == null || jaAtivado) return;

            presentes.Add(c);
            if (destaque != null) destaque.Mostrar();
            AtualizarIcones();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ControladorJogador c = other.GetComponent<ControladorJogador>();
            if (c == null) return;

            DefinirIcone(c, EstadoIconeInteracao.Nenhum);   // esconde o de quem saiu
            presentes.Remove(c);

            if (presentes.Count == 0 && destaque != null) destaque.Esconder();
            AtualizarIcones();
        }

        public bool PodeSofrerInteracao()
        {
            return !jaAtivado && presentes.Count >= 2;
        }

        public void Resetar()
        {
            DefinirIconesDeTodos(EstadoIconeInteracao.Nenhum);
            jaAtivado = false;
            presentes.Clear();
            if (destaque != null) destaque.Esconder();
            animacao.AnimacaoDesativado();
            collider.enabled = true;
        }

        private void AtualizarIcones()
        {
            if (jaAtivado) return;
            EstadoIconeInteracao estado = presentes.Count >= 2 //demonstra necessidade de interação mutua
                ? EstadoIconeInteracao.Possivel
                : EstadoIconeInteracao.Impossivel;
            DefinirIconesDeTodos(estado);
        }

        private void DefinirIconesDeTodos(EstadoIconeInteracao estado)
        {
            foreach (ControladorJogador c in presentes) DefinirIcone(c, estado);
        }

        private void DefinirIcone(ControladorJogador c, EstadoIconeInteracao estado)
        {
            IconeInteracaoJogador icone = c.GetComponentInChildren<IconeInteracaoJogador>();
            if (icone != null) icone.Definir(estado);
        }
    }
}