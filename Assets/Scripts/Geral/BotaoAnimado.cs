using UnityEngine;

// Vai num botão só para dar acesso fácil ao Animator dele a partir do
// OnClick() — sem interceptar nenhum evento de ponteiro, sem travas.
[RequireComponent(typeof(Animator))]
public class BotaoAnimado : MonoBehaviour
{
    public Animator Animator { get; private set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }
}