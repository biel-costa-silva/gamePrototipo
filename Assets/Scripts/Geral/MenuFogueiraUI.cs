using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Geral
{
    public class MenuFogueiraUI : MonoBehaviour
    {
        [Header("Fade / Painel")]
        [SerializeField] private FadeCanvas fadeCanvas;

        [Header("Botões")]
        [SerializeField] private Button botaoLevantar;
        [SerializeField] private Button botaoVerMemorias;

        [Header("Feedback Visual")]
        [SerializeField] private GameObject iconeBloqueioMemorias;

        public event Action OnLevantarClicado;
        public event Action OnVerMemoriasClicado;

        public bool EstaAberto { get; private set; }

        private void Awake()
        {
            if (fadeCanvas == null)
            {
                fadeCanvas = GetComponent<FadeCanvas>();
            }

            if (botaoLevantar != null)
            {
                botaoLevantar.onClick.AddListener(() => OnLevantarClicado?.Invoke());
            }

            if (botaoVerMemorias != null)
            {
                botaoVerMemorias.onClick.AddListener(() => OnVerMemoriasClicado?.Invoke());
            }
        }

        public void Abrir(bool podeVerMemorias)
        {
            EstaAberto = true;
            AtualizarEstadoMemorias(podeVerMemorias);

            if (fadeCanvas != null)
            {
                fadeCanvas.Mostrar();
            }
            else
            {
                gameObject.SetActive(true);
            }

            // Garante foco inicial no botão Levantar para navegação via teclado/gamepad
            if (botaoLevantar != null && botaoLevantar.interactable)
            {
                botaoLevantar.Select();
            }
        }

        public void AtualizarEstadoMemorias(bool podeVerMemorias)
        {
            if (botaoVerMemorias != null)
            {
                botaoVerMemorias.interactable = podeVerMemorias;
            }

            if (iconeBloqueioMemorias != null)
            {
                iconeBloqueioMemorias.SetActive(!podeVerMemorias);
            }
        }

        public void Fechar()
        {
            EstaAberto = false;

            if (fadeCanvas != null)
            {
                fadeCanvas.Esconder();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
