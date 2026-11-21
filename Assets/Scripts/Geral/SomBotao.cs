using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Geral
{
    public class SomBotao : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private string somHover = "hover";
        [SerializeField] private string somClick = "click";

        public void OnPointerEnter(PointerEventData eventData)
        {
            GerenciadorAudio.instancia?.TocarAudio2D(somHover);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GerenciadorAudio.instancia?.TocarAudio2D(somClick);
        }
    }
}