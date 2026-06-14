using System.Collections;
using UnityEngine;

public class GerenciadorMusicas : MonoBehaviour
{
    public static GerenciadorMusicas instancia;

    [SerializeField]
    private BibliotecaMusicas bibliotecaMusicas;
    [SerializeField]
    private AudioSource musicaFonte;

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
    }

    public void TocarMusica(string nomeMusica, float duracaoFade = 0.5f)
    {
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
}
