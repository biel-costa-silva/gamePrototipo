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

    public void Mostrar(Action aoTerminar = null)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Fade(canvasGroup.alpha, 1f, true, aoTerminar));
        
    }

    public void Esconder(Action aoTerminar = null)
    {        
        canvasGroup.blocksRaycasts = false;
        StopAllCoroutines();
        StartCoroutine(Fade(canvasGroup.alpha, 0f, false, aoTerminar));
    }

    private IEnumerator Fade(float de, float para, bool ativarInteracaoAoFinal, Action aoTerminar)
    {
        Debug.Log($"Alpha no início do Fade: {de}");
        canvasGroup.blocksRaycasts = false;

        float tempoPassado = 0f;
        while (tempoPassado < duracao)
        {
            tempoPassado += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(de, para, tempoPassado / duracao);
            yield return null;
        }

        canvasGroup.alpha = para;
        canvasGroup.interactable = ativarInteracaoAoFinal;
        canvasGroup.blocksRaycasts = ativarInteracaoAoFinal;

        if (!ativarInteracaoAoFinal)
            gameObject.SetActive(false);

        aoTerminar?.Invoke();
    }
}