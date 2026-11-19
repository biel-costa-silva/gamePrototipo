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
    public class Fogueira : MonoBehaviour, IInteracoes
    {
        [SerializeField] private GameObject efeitoFogueira; // filho da Fogueira na hierarquia, animações separadas
        [SerializeField] private MenuFogueiraUI menuUI;
        [SerializeField] private Transform[] pontosDeAssento; // Opcional: posições ao lado do fogo para cada jogador

        private AnimEfeitoFogueira animacao;
        private DestaqueInteracao destaque;

        private readonly HashSet<ControladorJogador> presentes = new HashSet<ControladorJogador>();
        private readonly List<ControladorJogador> sentados = new List<ControladorJogador>();

        public event Action OnAbrirMemorias;

        string IInteracoes.nome { get; set; } = "fogueira";

        private void Awake()
        {
            if (efeitoFogueira != null)
            {
                animacao = efeitoFogueira.GetComponent<AnimEfeitoFogueira>();
                if (animacao == null) animacao = efeitoFogueira.AddComponent<AnimEfeitoFogueira>();
            }

            destaque = GetComponent<DestaqueInteracao>();

            if (menuUI == null)
            {
                menuUI = FindFirstObjectByType<MenuFogueiraUI>();
            }
        }

        private void Start()
        {
            ConectarMenu();
        }

        private void OnEnable()
        {
            ConectarMenu();
        }

        private void OnDisable()
        {
            DesconectarMenu();
        }

        private void ConectarMenu()
        {
            if (menuUI != null)
            {
                menuUI.OnLevantarClicado -= LevantarTodos;
                menuUI.OnLevantarClicado += LevantarTodos;

                menuUI.OnVerMemoriasClicado -= TentarVerMemorias;
                menuUI.OnVerMemoriasClicado += TentarVerMemorias;
            }
        }

        private void DesconectarMenu()
        {
            if (menuUI != null)
            {
                menuUI.OnLevantarClicado -= LevantarTodos;
                menuUI.OnVerMemoriasClicado -= TentarVerMemorias;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ControladorJogador c = other.GetComponent<ControladorJogador>();
            if (c == null) return;

            presentes.Add(c);
            if (destaque != null) destaque.Mostrar();
            AtualizarIcones();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ControladorJogador c = other.GetComponent<ControladorJogador>();
            if (c == null) return;

            DefinirIcone(c, EstadoIconeInteracao.Nenhum);
            presentes.Remove(c);

            if (presentes.Count == 0 && destaque != null) destaque.Esconder();
            AtualizarIcones();
        }

        private void AtualizarIcones()
        {
            foreach (ControladorJogador c in presentes)
            {
                if (c == null) continue;

                if (sentados.Contains(c) || sentados.Count >= 2)
                {
                    DefinirIcone(c, EstadoIconeInteracao.Nenhum);
                }
                else
                {
                    DefinirIcone(c, EstadoIconeInteracao.Possivel);
                }
            }
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

        // Chamado pela interface genérica IInteracoes
        public void SofrerInteracao(Jogador jogador)
        {
            // Mantido por compatibilidade com a interface
        }

        // Chamado quando um ControladorJogador interage com a fogueira
        public void Interagir(ControladorJogador controlador)
        {
            if (controlador == null) return;

            if (!sentados.Contains(controlador))
            {
                sentados.Add(controlador);

                // Opcional: posiciona o jogador no ponto de assento correspondente
                int indiceAssento = sentados.Count - 1;
                if (pontosDeAssento != null && indiceAssento < pontosDeAssento.Length && pontosDeAssento[indiceAssento] != null)
                {
                    Vector3 posAssento = pontosDeAssento[indiceAssento].position;
                    controlador.transform.position = new Vector3(posAssento.x, controlador.transform.position.y, controlador.transform.position.z);
                }
            }

            if (animacao != null) animacao.animacaoInspencionar();

            AtualizarIcones();

            bool ambosSentados = sentados.Count >= 2;

            if (menuUI != null)
            {
                if (!menuUI.EstaAberto)
                {
                    menuUI.Abrir(ambosSentados);
                }
                else
                {
                    menuUI.AtualizarEstadoMemorias(ambosSentados);
                }
            }
        }

        public void LevantarTodos()
        {
            List<ControladorJogador> listaParaLevantar = new List<ControladorJogador>(sentados);
            sentados.Clear();

            foreach (ControladorJogador c in listaParaLevantar)
            {
                if (c != null)
                {
                    c.LevantarDaFogueira();
                }
            }

            if (menuUI != null)
            {
                menuUI.Fechar();
            }

            if (animacao != null)
            {
                animacao.animacaoVoltar();
            }

            AtualizarIcones();
        }

        public void TentarVerMemorias()
        {
            if (sentados.Count < 2)
            {
                Debug.LogWarning("[Fogueira] Ambos os jogadores precisam estar na fogueira para ver memórias!");
                return;
            }

            Debug.Log("[Fogueira] Ambos os jogadores presentes! Disparando evento de memórias.");
            OnAbrirMemorias?.Invoke();
        }

        public bool PodeSofrerInteracao()
        {
            return sentados.Count < 2;
        }

        public void Resetar()
        {
            LevantarTodos();
            presentes.Clear();
            if (destaque != null) destaque.Esconder();
        }
    }
}
