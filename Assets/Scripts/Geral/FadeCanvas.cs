using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvas : MonoBehaviour
{
    [SerializeField] private float duracao = 0.25f;
    private CanvasGroup grupo;
    private CanvasGroup canvasGroup => grupo != null ? grupo : (grupo = GetComponent<CanvasGroup>());

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
    }
    public IEnumerator PreAquecer()
    {
        gameObject.SetActive(true);              // Awake/OnEnable/layout rodam aqui
        canvasGroup.alpha = 0.01f;               // quase invisível, mas ainda desenha
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        Canvas.ForceUpdateCanvases();
        yield return null;                       // 1 frame para o render montar tudo
        canvasGroup.alpha = 0f;                  // corrige o bug 1
        gameObject.SetActive(false);
    }

    public void Mostrar(Action aoTerminar = null)
    {
        if (gameObject.activeInHierarchy && canvasGroup.interactable)
        {            
            aoTerminar?.Invoke();
            return;
        }
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Fade(canvasGroup.alpha, 1f, true, aoTerminar));
        
    }

    public void Esconder(Action aoTerminar = null)
    {
        if (!gameObject.activeInHierarchy)
        {
            aoTerminar?.Invoke();
            return;
        }
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        StopAllCoroutines();
        StartCoroutine(Fade(canvasGroup.alpha, 0f, false, aoTerminar));
    }
    public void MostrarImediato()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void EsconderImediato()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }


    private IEnumerator Fade(float de, float para, bool ativarInteracaoAoFinal, Action aoTerminar)
    {
        Debug.Log($"Alpha no início do Fade: {de}");
        canvasGroup.blocksRaycasts = false;

        float tempoPassado = 0f;
        while (tempoPassado < duracao)
        {
            tempoPassado += Time.unscaledDeltaTime;
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