using UnityEngine;

namespace Assets.Scripts.Geral
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GatilhoTutorial : MonoBehaviour
    {
        [SerializeField] private DadosTutorial dados;

        private BoxCollider2D area;
        private Camera cam;

        private void Awake()
        {
            area = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            // Só funciona na primeira vez que a cena de jogo é aberta na sessão
            if (!ContadorDeCenas.EhPrimeiraVezCenaAtual())
            {
                gameObject.SetActive(false);
                return;
            }

            cam = Camera.main;
        }

        private void Update()
        {
            if (cam == null || dados == null || GerenciadorTutorial.instancia == null) return;
            if (GerenciadorTutorial.aberto) return;

            // Verifica se o ponto central (X, Y) da câmera entrou na área do BoxCollider2D
            if (area.OverlapPoint(cam.transform.position))
            {
                GerenciadorTutorial.instancia.Mostrar(dados);
                gameObject.SetActive(false);
            }
        }
    }
}