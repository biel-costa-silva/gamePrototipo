using System.Collections;
using UnityEngine;

public class GerenciadorMusicas : MonoBehaviour
{
    public static GerenciadorMusicas instancia;

    [SerializeField]
    private BibliotecaMusicas bibliotecaMusicas;
    [SerializeField]
    private AudioSource musicaFonte;

    private float volumeOriginal;
    private Coroutine fadeAtual;


    private void Awake()
    {
        if(instancia != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        volumeOriginal = musicaFonte.volume;
    }

    public void TocarMusica(string nomeMusica, float duracaoFade = 0.5f)
    {
        CancelarFade();
        StartCoroutine(TransicaoMusicalFade(bibliotecaMusicas.GetClipNome(nomeMusica), duracaoFade));
    }

    IEnumerator TransicaoMusicalFade(AudioClip proximaTrilha, float duracaoFade = 0.5f)
    {
        float porcento = 0;
        while( porcento < 1)
        {
            porcento += Time.deltaTime * 1 / duracaoFade;
            musicaFonte.volume = Mathf.Lerp(1f, 0, porcento);
            yield return null;
        }

        musicaFonte.clip = proximaTrilha;
        musicaFonte.Play();

        porcento = 0;
        while( porcento < 1)
        {
            porcento += Time.deltaTime * 1 / duracaoFade;
            musicaFonte.volume = Mathf.Lerp(0, 1f, porcento);
            yield return null;
        }
    }

    public void SumirMusica(float duracao)
    {
        CancelarFade();
        fadeAtual = StartCoroutine(RotinaSumirMusica(duracao));
    }

    private IEnumerator RotinaSumirMusica(float duracao)
    {
        float volumeInicial = musicaFonte.volume;

        for (float tempo = 0f; tempo < duracao; tempo += Time.unscaledDeltaTime)
        {
            float restante = 1f - tempo / duracao;
            musicaFonte.volume = volumeInicial * restante * restante; // queda mais suave ao ouvido
            yield return null;
        }

        musicaFonte.volume = 0f;
        musicaFonte.Stop();
        fadeAtual = null;
    }

    private void CancelarFade()
    {
        if (fadeAtual != null)
        {
            StopCoroutine(fadeAtual);
            fadeAtual = null;
        }

        musicaFonte.volume = volumeOriginal;
    }
}
