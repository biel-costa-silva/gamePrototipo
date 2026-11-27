using UnityEngine;

namespace Assets.Scripts.Geral
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GatilhoTutorial : MonoBehaviour
    {
        [SerializeField] private DadosTutorial dados;

        private BoxCollider2D area;

        private void Awake()
        {
            area = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            int vezes = ContadorDeCenas.VezesAbertaCenaAtual();

            // Só funciona na primeira vez que a cena de jogo é aberta na sessão
            if (!ContadorDeCenas.EhPrimeiraVezCenaAtual())
            {
                Debug.Log($"[GatilhoTutorial] Desativado no Start: cena já foi aberta {vezes}x.");
                gameObject.SetActive(false);
                return;
            }
        }

        private void Update()
        {
            if (dados == null || GerenciadorTutorial.instancia == null) return;
            if (GerenciadorTutorial.aberto) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            // Verifica se o ponto central (X, Y) da câmera entrou na área do BoxCollider2D
            if (area.OverlapPoint(cam.transform.position))
            {
                GerenciadorTutorial.instancia.Mostrar(dados);
                gameObject.SetActive(false);
            }
        }
    }
}