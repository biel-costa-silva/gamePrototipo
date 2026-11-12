using Assets.Scripts.Geral;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
{
    public class SpawnerInimigos : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private MinhaCamera cameraController;
        [SerializeField] private Transform posicaoCanvasRound;
        [SerializeField] private GameObject[] animacoesRound;
        [Tooltip("Distância além da borda da câmera onde o inimigo nasce. O Y do spawn é o do próprio GameObject do Spawner.")]
        [SerializeField] private float margemForaDaTela = 1.5f;

        [Header("Inimigos (ordem do Quadro 3)")]
        [SerializeField] private GameObject slime;
        [SerializeField] private GameObject goblin;
        [SerializeField] private GameObject goblinEscudo;
        
        [Header("Item de fim de round")]
        [Tooltip("Prefab com o script Item e um Collider2D (Is Trigger). Dropado quando o último inimigo morre.")]
        [SerializeField] private GameObject itemDoRound;
        [SerializeField] private float margemItemNaTela = 1.5f;


        [Header("Round 1")]
        [SerializeField] private float intervaloGeracao = 5f;
        [SerializeField] private int quantInimigosGerar = 5;

        [Header("Evolução a cada round")]
        [Tooltip("O intervalo é multiplicado por isso a cada round (menor que 1 = mais rápido).")]
        [SerializeField, Range(0.1f, 1f)] private float fatorIntervalo = 0.9f;
        [SerializeField] private float intervaloMinimo = 1f;
        [Tooltip("Quantos inimigos a mais são gerados a cada round.")]
        [SerializeField] private int acrescimoQuantidade = 2;
        [Tooltip("Um tipo novo de inimigo entra a cada X rounds.")]
        [SerializeField] private int roundsParaNovoInimigo = 2;
        [SerializeField] private int inimigosIniciais = 2; // slime + goblin
        [Tooltip("Pausa entre pegar o item e começar o próximo round.")]
        [SerializeField] private float pausaEntreRounds = 3f;

        public int rodadaAtual { get; private set; } = 0;

        private readonly List<GameObject> ordem = new List<GameObject>();   // todos os tipos, na ordem
        private readonly List<GameObject> vivos = new List<GameObject>();

        private bool ativo = false;
        private bool aguardandoProximoRound = false;
        private bool aguardandoItem = false;
        private Item itemAtual;
        private Vector3 ultimaPosicaoInimigo;
        private float timer;
        private float intervaloAtual;
        private int quantidadeDoRound;
        private int geradosNoRound;
        private int tiposDisponiveis;

        private void Awake()
        {
            // para adicionar um inimigo novo: crie o campo acima e coloque aqui, na ordem de entrada
            AdicionarSeExistir(slime);
            AdicionarSeExistir(goblin);
            AdicionarSeExistir(goblinEscudo);
        }

        private void AdicionarSeExistir(GameObject prefab)
        {
            if (prefab != null) ordem.Add(prefab);
        }

        // ------------------- chamados pelo GerenciadorPartida -------------------

        public void Iniciar()
        {
            if (ativo) return;
            ativo = true;
            rodadaAtual = 0;
            aguardandoProximoRound = true;   // espera a pausa e começa o round 1
            timer = pausaEntreRounds;
        }

        public void Parar()
        {
            ativo = false;
        }

        public void Resetar()
        {
            ativo = false;
            aguardandoItem = false;
            rodadaAtual = 0;
            LimparItem();
            foreach (GameObject inimigo in vivos)
            {
                if (inimigo != null) Destroy(inimigo);
            }
            vivos.Clear();
        }

        // ------------------------------------------------------------------------

        private void Update()
        {
            if (!ativo || GerenciadorPausa.JogoPausado) return;

            if (aguardandoItem) return; // round acabou: espera alguém pegar o item
            // guarda onde os inimigos estão; o último a morrer define onde o item cai
            foreach (GameObject i in vivos)
            {
                if (i != null) ultimaPosicaoInimigo = i.transform.position;
            }

            vivos.RemoveAll(i => i == null);
            timer -= Time.deltaTime;

            if (aguardandoProximoRound)
            {
                if (timer <= 0f) IniciarRound(rodadaAtual + 1);
                return;
            }

            if (geradosNoRound < quantidadeDoRound)
            {
                if (timer <= 0f)
                {
                    GerarInimigo();
                    geradosNoRound++;
                    timer = intervaloAtual;
                }
            }
            else if (vivos.Count == 0)
            {
                // gerou tudo e os jogadores limparam a tela: dropa o item
                DroparItem();
            }
        }

        private void DroparItem()
        {
            if (itemDoRound == null)
            {
                Debug.LogWarning("[Spawner] Nenhum item configurado; indo direto para o próximo round.");
                PrepararProximoRound();
                return;
            }

            // os jogadores não saem da tela, então o item precisa cair dentro dela
            float x = Mathf.Clamp(ultimaPosicaoInimigo.x,
                cameraController.GetLimiteEsquerdo() + margemItemNaTela,
                cameraController.GetLimiteDireito() - margemItemNaTela);

            GameObject go = Instantiate(itemDoRound, new Vector3(x, transform.position.y, transform.position.z), Quaternion.identity);
            itemAtual = go.GetComponent<Item>();

            if (itemAtual == null)
            {
                Debug.LogWarning("[Spawner] O prefab do item não tem o script Item.");
                Destroy(go);
                PrepararProximoRound();
                return;
            }

            aguardandoItem = true;
            itemAtual.OnColetado += AoItemColetado;
        }

        private void AoItemColetado(Item item)
        {
            item.OnColetado -= AoItemColetado;
            itemAtual = null;
            aguardandoItem = false;
            PrepararProximoRound();
        }

        private void PrepararProximoRound()
        {
            aguardandoProximoRound = true;
            timer = pausaEntreRounds;
        }

        private void LimparItem()
        {
            if (itemAtual == null) return;
            itemAtual.OnColetado -= AoItemColetado;
            Destroy(itemAtual.gameObject);
            itemAtual = null;
        }

        private void IniciarRound(int round)
        {
            rodadaAtual = round;
            if (round - 1 < animacoesRound.Length)
                Instantiate(animacoesRound[round - 1], posicaoCanvasRound.position, Quaternion.identity);

            aguardandoProximoRound = false;
            geradosNoRound = 0;
            timer = 0f;

            // atributos do round
            intervaloAtual = Mathf.Max(intervaloMinimo, intervaloGeracao * Mathf.Pow(fatorIntervalo, round - 1));
            quantidadeDoRound = quantInimigosGerar + acrescimoQuantidade * (round - 1);

            // quais inimigos: começa com 2 e ganha 1 novo a cada 'roundsParaNovoInimigo' rounds
            int novos = (round - 1) / Mathf.Max(1, roundsParaNovoInimigo);
            tiposDisponiveis = Mathf.Min(inimigosIniciais + novos, ordem.Count);

            Debug.Log($"[Spawner] Round {round}: {quantidadeDoRound} inimigos, a cada {intervaloAtual:0.0}s, {tiposDisponiveis} tipos");
        }

        private void GerarInimigo()
        {
            if (tiposDisponiveis <= 0) return;

            GameObject prefab = ordem[Random.Range(0, tiposDisponiveis)];

            bool esquerda = Random.value < 0.5f;
            float x = esquerda
                ? cameraController.GetLimiteEsquerdo() - margemForaDaTela
                : cameraController.GetLimiteDireito() + margemForaDaTela;

            Vector3 pos = new Vector3(x, transform.position.y, transform.position.z);
            vivos.Add(Instantiate(prefab, pos, Quaternion.identity));
        }
    }
}