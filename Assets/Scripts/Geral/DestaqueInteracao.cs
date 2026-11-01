using UnityEngine;

public class DestaqueInteracao : MonoBehaviour
{
    [SerializeField] private SpriteRenderer alvo;
    [SerializeField] private ParticleSystem orbes;   // opcional
    [SerializeField] private float velocidadeFade = 6f;

    private static readonly int IdDestaque = Shader.PropertyToID("_Destaque");
    private MaterialPropertyBlock bloco;
    private float atual, desejado;

    private void Awake()
    {
        if (alvo == null) alvo = GetComponent<SpriteRenderer>();
        bloco = new MaterialPropertyBlock();

        atual = 0f;
        desejado = 0f;
        alvo.GetPropertyBlock(bloco);
        bloco.SetFloat(IdDestaque, 0f);
        alvo.SetPropertyBlock(bloco);
    }

    public void Mostrar()
    {
        desejado = 0.7f;
        if (orbes != null && !orbes.isPlaying) orbes.Play();
    }

    public void Esconder()
    {
        desejado = 0f;
        // para de emitir; as partículas que já existem somem sozinhas
        if (orbes != null) orbes.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void Update()
    {
        if (Mathf.Approximately(atual, desejado)) return;
        atual = Mathf.MoveTowards(atual, desejado, velocidadeFade * Time.deltaTime);
        alvo.GetPropertyBlock(bloco);
        bloco.SetFloat(IdDestaque, atual);
        alvo.SetPropertyBlock(bloco);
    }
}