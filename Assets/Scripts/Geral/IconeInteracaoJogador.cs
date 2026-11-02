using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Geral
{
    public enum EstadoIconeInteracao { Nenhum, Possivel, Impossivel }
    public class IconeInteracaoJogador : MonoBehaviour
    {
        [SerializeField] private GameObject iconePossivel;
        [SerializeField] private GameObject iconeImpossivel;

        [Header("Tremor (tentou interagir sem poder)")]
        [SerializeField] private float duracaoTremor = 0.3f;
        [SerializeField] private float intensidadeTremor = 0.06f;  // em unidades do mundo
        [SerializeField] private float velocidadeTremor = 60f;

        private Coroutine tremorAtual;

        private void Awake()
        {
            Definir(EstadoIconeInteracao.Nenhum);
        }

        public void Definir(EstadoIconeInteracao estado)
        {
            if (iconePossivel != null) iconePossivel.SetActive(estado == EstadoIconeInteracao.Possivel);
            if (iconeImpossivel != null) iconeImpossivel.SetActive(estado == EstadoIconeInteracao.Impossivel);
        }

        public void Tremer()
        {
            // só treme se o ícone de "impossível" estiver visível e se já não estiver tremendo
            Debug.Log($"[Icone] Tremer chamado. impossivel ativo: {(iconeImpossivel != null && iconeImpossivel.activeInHierarchy)}");
            if (iconeImpossivel == null || !iconeImpossivel.activeInHierarchy) return;
            if (tremorAtual != null) return;
            tremorAtual = StartCoroutine(RotinaTremer());
        }

        private IEnumerator RotinaTremer()
        {
            Vector3 origem = transform.localPosition;
            float t = 0f;

            while (t < duracaoTremor)
            {
                t += Time.deltaTime;
                float forca = 1f - (t / duracaoTremor);   // o tremor vai diminuindo
                float deslocamento = Mathf.Sin(t * velocidadeTremor) * intensidadeTremor * forca;
                transform.localPosition = origem + new Vector3(deslocamento, 0f, 0f);
                yield return null;
            }

            transform.localPosition = origem;
            tremorAtual = null;
        }
    }
}