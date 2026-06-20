using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FundoEscuroController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void IrParaOpcoes()
    {
        animator.SetTrigger("irParaOpcoes");
    }

    public void IrParaPrincipal()
    {
        animator.SetTrigger("irParaPrincipal");
    }
}