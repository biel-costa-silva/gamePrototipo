using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Geral
{
    public enum RespostaConfirmacao { Aguardando, Sim, Nao }
    public class GerenciadorInteracoes : MonoBehaviour
    {
        public static GerenciadorInteracoes instancia;
        public static bool menuAberto { get; private set; }
        public static RespostaConfirmacao resposta { get; private set; }

        public static bool totemInteragido { get; set; } = false;
        public static bool confirmacaoDeInteracao { get; private set; } = false;

        [SerializeField] private FadeCanvas menuInteracaoTotem;
        [SerializeField] private FadeCanvas menuInteracaoFogueira;


        private void Awake()
        {
            instancia = this;
        }

        private IEnumerator Start()
        {
            if (menuInteracaoTotem != null)
                yield return StartCoroutine(menuInteracaoTotem.PreAquecer());

            if (menuInteracaoFogueira != null)
                yield return StartCoroutine(menuInteracaoFogueira.PreAquecer());
        }

        private void Update()
        {
            if (GerenciadorPartida.jogoAcabou) return;
            if (menuAberto && Input.GetKeyDown(KeyCode.Escape)) Responder(RespostaConfirmacao.Nao);            
        }        

        public void PedirConfirmacao()
        {
            resposta = RespostaConfirmacao.Aguardando;
            menuAberto = true;
            menuInteracaoTotem.Mostrar();
        }

        private void Responder(RespostaConfirmacao r)
        {
            if (!menuAberto) return;
            resposta = r;                // fica guardada até o próximo pedido
            menuAberto = false;
            menuInteracaoTotem.Esconder();
        }

        public void BotaoSim() { Responder(RespostaConfirmacao.Sim); }
        public void BotaoNao() { Responder(RespostaConfirmacao.Nao); }
    }
}