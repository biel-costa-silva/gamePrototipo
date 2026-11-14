using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Vai em cada painel que tem botões.
public class SelecionarAoAbrir : MonoBehaviour
{
    [SerializeField] private Selectable primeiroBotao;
    private bool estavaUtilizavel;

    private void OnEnable()
    {
        estavaUtilizavel = false;
    }

    private void Update()
    {
        if (primeiroBotao == null || EventSystem.current == null) return;

        bool utilizavel = primeiroBotao.gameObject.activeInHierarchy && primeiroBotao.IsInteractable();
        if (utilizavel && !estavaUtilizavel)
        {
            EventSystem.current.SetSelectedGameObject(null);                       // limpa uma seleção antiga
            EventSystem.current.SetSelectedGameObject(primeiroBotao.gameObject);

            if (CursorMenu.instancia != null) CursorMenu.instancia.Sincronizar();  // cursor vai até o botão
        }

        estavaUtilizavel = utilizavel;
    }
}