using Assets.Scripts.Gameplay.Controller;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using Assets.Scripts.Gameplay.View.AnimacaoObjetos;
using Assets.Scripts.Geral;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
{
    public class Item : MonoBehaviour, IInteracoes
    {
        [Header("Cuspida")]
        [SerializeField] private Vector2 impulso = new Vector2(3f, 6f);   // X para o lado, Y para cima
        [SerializeField] private float quique = 0.5f;                    // 0 a 1: quanto da velocidade vertical volta
        [SerializeField] private float atritoNoChao = 0.7f;              // reduz a velocidade horizontal a cada quique
        [SerializeField] private float velocidadeMinimaQuique = 1f;

        private Rigidbody2D rb;
        [SerializeField]private DestaqueInteracao destaque;
        private readonly HashSet<ControladorJogador> presentes = new HashSet<ControladorJogador>();
        private float yChao;
        private bool voando = false;

        public event Action<Item> OnColetado;
        private bool coletado = false;


        string IInteracoes.nome { get; set; } = "item";

        private void Awake()
        {            
            destaque = GetComponent<DestaqueInteracao>();
        }

        public void Cuspir(float direcao, float yChaoMundo)
        {
            rb = GetComponent<Rigidbody2D>();
            yChao = yChaoMundo;
            rb.linearVelocity = new Vector2(direcao * impulso.x, impulso.y);
            voando = true;
        }

        private void FixedUpdate()
        {
            if (!voando) return;
            if (rb.position.y > yChao || rb.linearVelocity.y > 0f) return;   // ainda no ar, ou subindo

            // tocou o chão
            Vector2 v = rb.linearVelocity;
            rb.position = new Vector2(rb.position.x, yChao);

            if (Mathf.Abs(v.y) * quique < velocidadeMinimaQuique)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;   // parou de vez
                voando = false;
            }
            else
            {
                rb.linearVelocity = new Vector2(v.x * atritoNoChao, -v.y * quique);
            }
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            ControladorJogador c = other.GetComponent<ControladorJogador>();
            if (c == null || coletado) return;

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


        private void AtualizarIcones()
        {
            if (coletado) return;
            EstadoIconeInteracao estado = presentes.Count >= 1
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



        public bool PodeSofrerInteracao()
        {
            return !coletado && presentes.Count >= 1;
        }

        public void SofrerInteracao(Jogador jogador)
        {
            if (coletado) return;
            coletado = true;

            // aqui entra o efeito do item no futuro (cura, buff, upgrade...)

            OnColetado?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
