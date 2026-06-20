using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvas : MonoBehaviour
{
    [SerializeField] private float duracao = 0.25f;
    private CanvasGroup canvasGroup;
    private Animator[] animatorsDosBotoes;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();       
        animatorsDosBotoes = GetComponentsInChildren<Animator>(true);
    }

    private IEnumerator EsperarBotoesChegaremEmNormal()
    {
        bool aindaTransicionando = true;

        while (aindaTransicionando)
        {
            aindaTransicionando = false;

            foreach (Animator anim in animatorsDosBotoes)
            {
                bool estaEmTransicao = anim.IsInTransition(0);
                var estadoAtual = anim.GetCurrentAnimatorStateInfo(0);
                bool estaEstavel = estadoAtual.IsName("Normal") || estadoAtual.IsName("Selected");

                if (estaEmTransicao || !estaEstavel)
                {
                    aindaTransicionando = true;
                }
            }

            if (aindaTransicionando)
                yield return null; // espera o próximo frame e checa de novo
        }
    }

    // aoTerminar é opcional: usado quando alguém precisa SABER que o fade acabou
    // (ex: o GerenciadorDeMenus, pra só então mostrar o próximo painel)
    public void Mostrar(Action aoTerminar = null)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Fade(canvasGroup.alpha, 1f, true, aoTerminar));
    }

    public void Esconder(Action aoTerminar = null)
    {
        StopAllCoroutines();
        StartCoroutine(EsconderSequencial(aoTerminar));
    }

    private IEnumerator EsconderSequencial(Action aoTerminar)
    {
        yield return StartCoroutine(EsperarBotoesChegaremEmNormal());
        yield return StartCoroutine(Fade(canvasGroup.alpha, 0f, false, aoTerminar));
    }

    private IEnumerator Fade(float de, float para, bool ativarInteracaoAoFinal, Action aoTerminar)
    {       
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float tempoPassado = 0f;
        while (tempoPassado < duracao)
        {
            tempoPassado += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(de, para, tempoPassado / duracao);
            yield return null;
        }

        canvasGroup.alpha = para;

        // Só reativa interação no painel que está aparecendo (Mostrar).
        // No painel que está desaparecendo (Esconder), permanece desativado.
        canvasGroup.interactable = ativarInteracaoAoFinal;
        canvasGroup.blocksRaycasts = ativarInteracaoAoFinal;

        if (!ativarInteracaoAoFinal)
            gameObject.SetActive(false);

        aoTerminar?.Invoke();
    }
}