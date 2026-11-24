using TMPro;
using UnityEngine;

public class MensagemCutscene : MonoBehaviour
{
    [SerializeField] private TMP_Text texto;

    // Animation Event: preencha o campo String com a mensagem
    public void DefinirTexto(string mensagem)
    {
        texto.text = mensagem;
    }
}