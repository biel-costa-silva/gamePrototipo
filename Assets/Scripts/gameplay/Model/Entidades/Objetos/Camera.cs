using UnityEngine;
public class Camera : MonoBehaviour
{
    // Atributos da câmera
    private UnityEngine.Camera cam;
    private float metadeLargura;

    public Transform player1;
    public Transform player2;

    [Header("Limtes do mapa")]
    public float minX, maxX;

    public float suavidade = 2f;
    internal static object main;

    private bool travada = false;
    private Vector3 destinoTravado;


    public void Awake()
    {
        cam = UnityEngine.Camera.main;        
    }

    public void Start()
    {
        float altura = cam.orthographicSize * 2f;// orthographicSize = metade da altura da câmera
        float largura = altura * cam.aspect; // aspect = largura da câmera
        metadeLargura = largura / 2f;
    } 
    
    
    void LateUpdate()
    {
        Vector3 destino;

        if (travada)
        {
            destino = destinoTravado;
        }
        else
        {
            Vector2 p1 = player1.position;
            Vector2 p2 = player2.position;
           
            Vector2 centro = (p1 + p2) / 2f;

            //para onde a câmera se dirige 
            destino = new Vector3(centro.x, centro.y, transform.position.z);
            destino.x = Mathf.Clamp(destino.x, minX, maxX);
        }
        
        transform.position = Vector3.Lerp(transform.position, destino, suavidade * Time.deltaTime);        
    }
    public void TravarCamera()
    {
        Vector2 p1 = player1.position;
        Vector2 p2 = player2.position;

        Vector2 centro = (p1 + p2) / 2f;

        Vector3 destino = new Vector3(centro.x, centro.y, transform.position.z);
        destino.x = Mathf.Clamp(destino.x, minX, maxX);       

        destinoTravado = destino;
        travada = true;
    }

    public void DestravarCamera()
    {
        travada = false;
    }

    public float GetLimiteEsquerdo()
    {
        return transform.position.x - metadeLargura;
    }
    public float GetLimiteDireito()
    {
        return transform.position.x + metadeLargura;
    }
}
