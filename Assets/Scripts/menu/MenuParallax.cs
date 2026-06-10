using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    public float multiplicadorDesl = 1f;
    public float tempoSuavizacao = 3f;

    private Vector2 posicaoInicial;
    private Vector3 velocidade;

    private void Start() 
    {
        posicaoInicial = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 deslocamento = UnityEngine.Camera.main.ScreenToViewportPoint(Input.mousePosition);
        transform.position = Vector3.SmoothDamp(transform.position, posicaoInicial + (deslocamento * multiplicadorDesl), ref velocidade, tempoSuavizacao);
    }
}
