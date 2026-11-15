using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cam;
    public float parallaxEffect;

    [Header("Movimento próprio (independe dos jogadores)")]
    [SerializeField] private bool movimentoAutonomo = false;
    [SerializeField] private bool paraEsquerda = true;

    [Header("Looping")]
    [Tooltip("Cópias criadas de cada lado. Aumente se aparecer buraco na tela.")]
    [SerializeField] private int copiasPorLado = 1;

    private float length;
    private float startPos;
    private float deslocamentoAuto;   // quanto a imagem andou sozinha

    void Start()
    {
        startPos = transform.position.x;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        length = sr.bounds.size.x;

        // cria as cópias laterais para o looping
        for (int i = 1; i <= copiasPorLado; i++)
        {
            CriarCopia(sr, -i);
            CriarCopia(sr, +i);
        }
    }
    
    void LateUpdate()
    {
        // 1) movimento próprio: o parallaxEffect vira velocidade constante (unidades por segundo)
        if (movimentoAutonomo)
        {
            float direcao = paraEsquerda ? -1f : 1f;
            deslocamentoAuto += direcao * parallaxEffect * Time.deltaTime;
            deslocamentoAuto = Mathf.Repeat(deslocamentoAuto, length); // andar 1 largura é invisível
        }

        // 2) movimento causado pela câmera (igual ao seu original)
        float x = startPos + cam.position.x * parallaxEffect + deslocamentoAuto;

        // 3) loop: se a imagem ficou meia largura longe da câmera, pula uma largura inteira
        float distancia = cam.position.x - x;
        x += Mathf.Round(distancia / length) * length;

        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    private void CriarCopia(SpriteRenderer original, int indice)
    {
        GameObject go = new GameObject(name + "_copia" + indice);
        go.transform.SetParent(transform, false);
        go.transform.position = transform.position + new Vector3(indice * length, 0f, 0f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = original.sprite;
        sr.color = original.color;
        sr.flipX = original.flipX;
        sr.flipY = original.flipY;
        sr.sharedMaterial = original.sharedMaterial;
        sr.sortingLayerID = original.sortingLayerID;
        sr.sortingOrder = original.sortingOrder;
        sr.drawMode = original.drawMode;
        if (original.drawMode != SpriteDrawMode.Simple) sr.size = original.size;
    }
}
