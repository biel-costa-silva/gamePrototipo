using Assets.Scripts.Gameplay.Controller;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using Assets.Scripts.Gameplay.View.AnimacaoObjetos;
using Assets.Scripts.Geral;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
{
    public class Fogueira : MonoBehaviour, IInteracoes
    {
        [SerializeField] private GameObject efeitoFogueira; // filho da Fogueira na hierarquia, animações separadas
        [SerializeField] private MenuFogueiraUI menuUI;

        private AnimEfeitoFogueira animacao;
        private DestaqueInteracao destaque;

        private readonly HashSet<ControladorJogador> presentes = new HashSet<ControladorJogador>();
        private readonly List<ControladorJogador> sentados = new List<ControladorJogador>();

        public event Action OnAbrirMemorias;

        string IInteracoes.nome { get; set; } = "fogueira";

        [SerializeField] private ControladorJogador[] jogadoresIniciais; // arraste os 2 jogadores no Inspector
        [SerializeField] private bool comecarInteragida = false;

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

        private IEnumerator Start()
        {
            ConectarMenu();

            if (comecarInteragida)
            {
                yield return null; // espera todos os Start da cena rodarem
                IniciarSentados();
            }
        }
        private void IniciarSentados()
        {
            foreach (ControladorJogador c in jogadoresIniciais)
            {
                if (c == null) continue;

                presentes.Add(c);   // para os ícones e o destaque funcionarem
                Interagir(c);       // adiciona em sentados, abre/atualiza o menu
            }
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
            AtualizarDestaque();
            AtualizarIcones();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ControladorJogador c = other.GetComponent<ControladorJogador>();
            if (c == null) return;

            // Se o jogador está sentado na fogueira, ele não "saiu" da interação
            if (sentados.Contains(c)) return;

            DefinirIcone(c, EstadoIconeInteracao.Nenhum);
            presentes.Remove(c);

            AtualizarDestaque();
            AtualizarIcones();
        }

        private void AtualizarDestaque()
        {
            if (destaque == null) return;

            // Se já temos 2 sentados, ninguém mais pode interagir
            if (sentados.Count >= 2)
            {
                destaque.Esconder();
                return;
            }

            // O destaque só brilha se houver pelo menos um jogador presente que ainda NÃO sentou
            bool temJogadorApto = false;
            foreach (ControladorJogador c in presentes)
            {
                if (c != null && !sentados.Contains(c))
                {
                    temJogadorApto = true;
                    break;
                }
            }

            if (temJogadorApto)
            {
                destaque.Mostrar();
            }
            else
            {
                destaque.Esconder();
            }
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

                // Vira o jogador para olhar na direção da fogueira sem teleportar
                controlador.OlharPara(transform.position.x);

                // Dispara o efeito de inspecionar apenas no primeiro jogador a sentar
                if (sentados.Count == 1 && animacao != null)
                {
                    animacao.animacaoInspencionar();
                }
            }

            AtualizarDestaque();
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
            if (sentados.Count == 0) return;

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

            AtualizarDestaque();
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
            AtualizarDestaque();
        }

        

    }
}
