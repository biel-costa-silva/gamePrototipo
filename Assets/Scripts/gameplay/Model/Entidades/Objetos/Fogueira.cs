using Assets.Scripts.Gameplay.Controller;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using Assets.Scripts.Gameplay.View.AnimacaoObjetos;
using Assets.Scripts.Geral;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
{
	public class Fogueira : MonoBehaviour, IInteracoes
	{
        [SerializeField]private GameObject efeitoFogueira;//filho da Fogueira na hierarquia, animações separadas
        AnimEfeitoFogueira animacao;//componente do filho
        //private int id { get; set; } tirar 
        private DestaqueInteracao destaque;
        private bool jaAtivado = false;
        private readonly HashSet<ControladorJogador> presentes = new HashSet<ControladorJogador>();

        string IInteracoes.nome { get; set; } = "fogueira";

        private void Awake()
        {
            animacao = efeitoFogueira.GetComponent<AnimEfeitoFogueira>();

            if(animacao == null) animacao = efeitoFogueira.AddComponent<AnimEfeitoFogueira>();
            destaque = GetComponent<DestaqueInteracao>();
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

        private void AtualizarIcones()
        {
            if (jaAtivado) return;
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




        public void SofrerInteracao(Jogador jogador)
        {           
            animacao.animacaoInspencionar();//chamar 
        }
        public void SairInteracao()
        {
            animacao.animacaoVoltar();
        }

        public bool PodeSofrerInteracao()
        {
            return !jaAtivado && presentes.Count >= 1;
        }
        //Curar()

        //SalvarJogo()
    }
}