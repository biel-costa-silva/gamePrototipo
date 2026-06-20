using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class ClickSeguro : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    private static bool entradaBloqueada = false;

    private bool ponteiroDesceuAqui = false;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {       
        entradaBloqueada = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (entradaBloqueada)
        {
            return; 
        }

        entradaBloqueada = true;
        ponteiroDesceuAqui = true;

        animator.SetTrigger("Selected");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (!ponteiroDesceuAqui)
        {
            eventData.eligibleForClick = false;
        }

        ponteiroDesceuAqui = false;
    }
}