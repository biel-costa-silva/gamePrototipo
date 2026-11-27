using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Geral
{
    /// <summary>
    /// Singleton persistente que troca de cena com fade para preto.
    /// Deve ser o objeto RAIZ da hierarquia (DontDestroyOnLoad só vale para raízes),
    /// com o Canvas de fade como filho.
    /// </summary>
    public class GerenciadorNivel : MonoBehaviour
    {
        public static GerenciadorNivel instancia;

        // Destino final quando a cena de cutscene terminar
        public static string proximaCena = "Jogo";

        [SerializeField] private FadeCanvas fadeTransicao;   // Canvas preto (filho deste objeto)
        [SerializeField] private string cenaCutscene1 = "Cutscene";
        [SerializeField] private string cenaCutscene2 = "PosMorte";

        private bool carregando;

        // Cena carregada em segundo plano, esperando a ativação
        private AsyncOperation preCarregada;
        private string nomePreCarregada;


        private void Awake()
        {
            if (instancia != null)
            {
                Destroy(gameObject);
                return;
            }

            instancia = this;
            DontDestroyOnLoad(gameObject);
        }


        /// <summary>Vai direto para uma cena (ex.: "menu").</summary>
        public void IrPara(string cena)
        {
            if (carregando) return;
            StartCoroutine(Rotina(cena));
        }


        /// <summary>Passa pela cena de cutscene e depois vai para o destino (ex.: "jogo").</summary>
        public void IrParaViaCutscene1(string destino)
        {
            if (carregando) return;
            proximaCena = destino;
            StartCoroutine(Rotina(cenaCutscene1));
        }

        public void IrParaViaCutscene2(string destino)
        {
            if (carregando) return;
            proximaCena = destino;
            StartCoroutine(Rotina(cenaCutscene2));
        }

        /// <summary>
        /// Começa a carregar a cena em segundo plano SEM ativá-la.
        /// Chame durante a cutscene; a ativação acontece em IrPara/IrParaProxima.
        /// </summary>
        public void PreCarregar(string cena)
        {
            if (preCarregada != null) return;

            nomePreCarregada = cena;
            preCarregada = SceneManager.LoadSceneAsync(cena);
            if (preCarregada != null) preCarregada.allowSceneActivation = false;
        }


        /// <summary>Chamado pela cutscene ao terminar.</summary>
        public void IrParaProxima()
        {
            IrPara(proximaCena);
        }

        private IEnumerator Rotina(string cena)
        {
            carregando = true;

            // 1) Fade para preto (a tela fica bloqueada para cliques durante a transição)
            bool escurecido = false;
            fadeTransicao.Mostrar(() => escurecido = true);
            while (!escurecido) yield return null;

            Time.timeScale = 1f;

            // 2) Carrega a cena escondido pelo preto
            AsyncOperation op;
            if (preCarregada != null && nomePreCarregada == cena)
            {
                op = preCarregada;          // já está em 90%, só falta ativar
                preCarregada = null;
                nomePreCarregada = null;
                op.allowSceneActivation = true;
            }
            else
            {
                op = SceneManager.LoadSceneAsync(cena);
            }

            if (op == null)
            {
                Debug.LogError($"Cena '{cena}' não encontrada. Confira o nome e o Build Settings.");
                fadeTransicao.Esconder();
                carregando = false;
                yield break;
            }

            while (!op.isDone) yield return null;

            // 3) Um frame para os Awake/Start da nova cena rodarem, e depois clareia
            yield return new WaitForSeconds(1.3f);
            fadeTransicao.Esconder();

            carregando = false;
        }
    }
}
