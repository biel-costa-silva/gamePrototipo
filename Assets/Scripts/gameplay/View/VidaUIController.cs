using Assets.Scripts.Controller;
using Assets.Scripts.Geral;
using Assets.Scripts.Model.Entidades.Peoes;
using Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class VidaUIController : MonoBehaviour
{

    public int vida;
    
    public int vidaMax;

    public Image[] coracao;
    public Sprite cheio;
    public Sprite vazio;

    [SerializeField] private float duracaoFade = 0.25f;

    private CanvasGroup canvasGroup;    
    private bool visivelAtualmente = false;
    private Coroutine fadeAtual;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }


    // Update is called once per frame
    void Update()
    {
        if(GerenciadorPausa.JogoPausado) return;

        LogicaCoracao();      
    }
   

    public void DefinirVisivel(bool visivel)
    {
        if (visivel == visivelAtualmente) return;

        visivelAtualmente = visivel;

        if (fadeAtual != null) StopCoroutine(fadeAtual);
        fadeAtual = StartCoroutine(Fade(visivel ? 1f : 0f));
    }

    private IEnumerator Fade(float alvo)
    {
        float inicio = canvasGroup.alpha;
        float tempoPassado = 0f;

        while (tempoPassado < duracaoFade)
        {
            tempoPassado += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(inicio, alvo, tempoPassado / duracaoFade);
            yield return null;
        }

        canvasGroup.alpha = alvo;
        canvasGroup.interactable = alvo > 0.5f;
        canvasGroup.blocksRaycasts = alvo > 0.5f;
    }


    public void Decrementar(int dano)
    {
        vida = vida - dano;
    }
    public void RestaurarCheia()
    {
        vida = vidaMax;
    }

    void LogicaCoracao()
    {
        if(vida > vidaMax)
        {
            vida = vidaMax;
        }

        for (int i = 0; i < coracao.Length; i++)
        {
            if(i < vida)
            {
                coracao[i].sprite = cheio;
            }
            else
            {
                coracao[i].sprite = vazio;
            }
            if(i < vidaMax)
            {
                coracao[i].enabled = true;
            }
            else
            {
                coracao[i].enabled = true;
            }
        }
    }
}
