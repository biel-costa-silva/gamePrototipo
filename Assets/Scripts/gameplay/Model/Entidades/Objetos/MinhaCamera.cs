using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
{
    public class MinhaCamera : MonoBehaviour
    {
        // Atributos da câmera
        private Camera cam;
        private float metadeLargura;

        public Transform player1;
        public Transform player2;

        public Transform totem;

        [Header("Limtes do mapa")]
        public float minX, maxX;

        public float suavidade = 2f;
        internal static object main;


        [Header("Zoom dinâmico")]
        [Tooltip("Zoom mais fechado (jogadores bem próximos).")]
        [SerializeField] private float tamanhoMinimo = 4f;
        [Tooltip("Zoom mais aberto (jogadores bem afastados).")]
        [SerializeField] private float tamanhoMaximo = 6f;
        [Tooltip("Folga entre cada jogador e a borda da tela, em unidades.")]
        [SerializeField] private float margemLateral = 3f;
        [Tooltip("Velocidade ao abrir o zoom (alta, para não espremer os jogadores contra a borda).")]
        [SerializeField] private float suavidadeAbrir = 4f;
        [Tooltip("Velocidade ao fechar o zoom (mais baixa fica mais suave).")]
        [SerializeField] private float suavidadeFechar = 1.5f;
        [Tooltip("Tamanho usado enquanto a câmera está travada no totem (arena fixa).")]
        [SerializeField] private float tamanhoTravado = 6f;


        private bool travada = false;
        private Vector3 destinoTravado;

        private float yBase;

        public void Awake()
        {
            cam = Camera.main;
        }

        public void Start()
        {
            metadeLargura = cam.orthographicSize * cam.aspect;
            yBase = transform.position.y - cam.orthographicSize;
        }

        void LateUpdate()
        {
            AtualizarZoom();
            Vector3 destino;

            if (travada)
            {
                destino = destinoTravado;
            }
            else
            {
                Vector2 p1 = player1.position;
                Vector2 p2 = player2.position;

                Vector2 centro = (p1 + p2) / 2f;

                //para onde a câmera se dirige 
                destino = new Vector3(centro.x, transform.position.y, transform.position.z);
                destino.x = Mathf.Clamp(destino.x, minX, maxX);
            }

            Vector3 pos = Vector3.Lerp(transform.position, destino, suavidade * Time.deltaTime);
            pos.y = yBase + cam.orthographicSize;   // o chão continua no mesmo lugar da tela
            transform.position = pos;
        }

        private void AtualizarZoom()
        {
            float alvo;

            if (travada)
            {
                alvo = tamanhoTravado;
            }
            else
            {
                // tamanho necessário para os dois jogadores caberem com folga
                float distancia = Mathf.Abs(player1.position.x - player2.position.x);
                alvo = (distancia * 0.5f + margemLateral) / cam.aspect;
                alvo = Mathf.Clamp(alvo, tamanhoMinimo, tamanhoMaximo);
            }

            float velocidade = alvo > cam.orthographicSize ? suavidadeAbrir : suavidadeFechar;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, alvo, velocidade * Time.deltaTime);

            // os limites da tela (usados pelos jogadores e pelo Spawner) acompanham o zoom
            metadeLargura = cam.orthographicSize * cam.aspect;
        }
            
        public void TravarCamera()//mudar isso
        {
            Vector3 destino = new Vector3(totem.position.x, transform.position.y, transform.position.z);
            destino.x = Mathf.Clamp(destino.x, minX, maxX);

            destinoTravado = destino;
            travada = true;
        }

        public void DestravarCamera()
        {
            travada = false;
        }

        public float GetLimiteEsquerdo()
        {
            return transform.position.x - metadeLargura;
        }
        public float GetLimiteDireito()
        {
            return transform.position.x + metadeLargura;
        }
    }
}
