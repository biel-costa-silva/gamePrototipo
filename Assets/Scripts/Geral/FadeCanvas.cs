using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvas : MonoBehaviour
{
    [SerializeField] private float duracao = 0.25f;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
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
        StartCoroutine(Fade(canvasGroup.alpha, 0f, false, aoTerminar));
    }

    private IEnumerator Fade(float de, float para, bool ativarInteracaoAoFinal, Action aoTerminar)
    {
        // Desliga interação IMEDIATAMENTE, antes mesmo do fade visual começar.
        // É essa linha que resolve o bug do highlight: o botão "sabe" na hora
        // que não deve mais receber eventos de mouse.
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