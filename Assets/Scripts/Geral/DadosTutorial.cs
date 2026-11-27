using UnityEngine;
using UnityEngine.Video;

namespace Assets.Scripts.Geral
{
    /// <summary>
    /// Um tutorial = um asset. Botão direito no Project > Create > Tutorial > Dados.
    /// Preenche titulo, descricao e arrasta o VideoClip.
    /// </summary>
    [CreateAssetMenu(menuName = "Tutorial/Dados", fileName = "Tutorial_")]
    public class DadosTutorial : ScriptableObject
    {
        public string titulo;
        [TextArea(2, 5)] public string descricao;
        public VideoClip video;

        [Tooltip("Segundos (tempo real) antes de aparecer o aviso para continuar")]
        public float tempoMinimo = 2f;
    }
}
