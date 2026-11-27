using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.Scripts.Geral
{
    /// <summary>
    /// Coloque UM na cena Jogo. Funciona como um segundo pause: congela o jogo,
    /// mostra o vídeo em loop, espera o tempo mínimo e então libera a tecla para continuar.
    /// </summary>
    public class GerenciadorTutorial : MonoBehaviour
    {
        public static GerenciadorTutorial instancia;
        public static bool aberto { get; private set; }

        [Header("UI")]
        [SerializeField] private FadeCanvas painel;
        [SerializeField] private TextMeshProUGUI textoTitulo;
        [SerializeField] private TextMeshProUGUI textoDescricao;
        [SerializeField] private GameObject avisoContinuar;   // "Pressione ENTER"
        [SerializeField] private RawImage telaVideo;

        [Header("Vídeo")]
        [SerializeField] private VideoPlayer videoPlayer;     // renderMode = RenderTexture, playOnAwake = off

        [Header("Controle")]
        [SerializeField] private KeyCode teclaContinuar = KeyCode.Return;

        private void Awake()
        {
            instancia = this;
            aberto = false;
        }

        private IEnumerator Start()
        {
            // mesmo padrão dos outros menus: evita engasgo na primeira vez que o painel aparece
            if (painel != null)
                yield return StartCoroutine(painel.PreAquecer());
        }

        public void Mostrar(DadosTutorial dados)
        {
            if (aberto || dados == null) return;
            StartCoroutine(Rotina(dados));
        }

        private IEnumerator Rotina(DadosTutorial dados)
        {
            aberto = true;

            // 1) congela o jogo
            GerenciadorPausa.instancia.PausarParaTutorial();

            // 2) preenche textos e esconde a imagem do vídeo até ele ficar pronto (evita quadro preto)
            textoTitulo.text = dados.titulo;
            textoDescricao.text = dados.descricao;
            avisoContinuar.SetActive(false);
            if (telaVideo != null) telaVideo.enabled = false;

            // 3) ativa e mostra o painel ANTES de preparar o vídeo (VideoPlayer precisa estar ativo)
            painel.Mostrar();

            // 4) prepara e inicia o vídeo usando tempo sem escala (funciona com timeScale = 0)
            bool temVideo = dados.video != null && videoPlayer != null;
            if (temVideo)
            {
                videoPlayer.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
                videoPlayer.clip = dados.video;
                videoPlayer.isLooping = true;
                videoPlayer.Prepare();

                // limite de segurança de 3s para nunca travar o jogo se o vídeo falhar
                float tempoLimite = 3f;
                while (!videoPlayer.isPrepared && tempoLimite > 0f)
                {
                    tempoLimite -= Time.unscaledDeltaTime;
                    yield return null;
                }

                if (videoPlayer.isPrepared)
                {
                    videoPlayer.Play();
                    if (telaVideo != null) telaVideo.enabled = true;
                }
            }

            // 5) tempo mínimo em TEMPO REAL (timeScale está em 0)
            yield return new WaitForSecondsRealtime(dados.tempoMinimo);
            avisoContinuar.SetActive(true);

            // 6) espera o jogador
            while (!Input.GetKeyDown(teclaContinuar)) yield return null;

            // 7) fecha e devolve o jogo
            if (temVideo) videoPlayer.Stop();
            painel.Esconder();
            GerenciadorPausa.instancia.RetomarDoTutorial();
            aberto = false;
        }
    }
}