using UnityEngine;

namespace Assets.Scripts.Geral
{
    /// <summary>
    /// Coloque na cena de cutscene. Termina por tempo, por Animation Event/Timeline Signal
    /// (chamando Terminar()) ou ao apertar uma tecla depois de um tempo mínimo.
    /// </summary>
    public class CutsceneFim : MonoBehaviour
    {
        [SerializeField] private float duracao = 5f;          // 0 = só termina por evento/tecla
        [SerializeField] private float tempoMinimoParaPular = 1f;
        [SerializeField] private bool permitirPular = true;

        private float tempo;
        private bool terminou;

        private void Start()
        {
            // Carrega a cena de jogo em segundo plano enquanto a cutscene toca
            if (GerenciadorNivel.instancia != null)
                GerenciadorNivel.instancia.PreCarregar(GerenciadorNivel.proximaCena);
        }

        private void Update()
        {
            if (terminou) return;

            tempo += Time.deltaTime;

            if (duracao > 0f && tempo >= duracao)
            {
                Terminar();
                return;
            }

            if (permitirPular && tempo >= tempoMinimoParaPular && Input.anyKeyDown)
            {
                Terminar();
            }
        }

        // Pode ser chamado por Animation Event (sem parâmetros) ou Timeline Signal
        public void Terminar()
        {
            if (terminou) return;
            terminou = true;

            if (GerenciadorNivel.instancia != null)
                GerenciadorNivel.instancia.IrParaProxima();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(GerenciadorNivel.proximaCena);
        }
    }
}
