using UnityEngine;

namespace Assets.Scripts.Geral
{
    /// <summary>
    /// Pausa e retoma os AudioSources deste objeto junto com o pause do jogo.
    /// Coloque na fogueira e no totem (no mesmo GameObject dos AudioSources).
    /// </summary>
    public class PausarAudioComJogo : MonoBehaviour
    {
        [SerializeField] private AudioSource[] fontes;

        private bool estavaPausado;

        private void Reset()
        {
            // Ao adicionar o componente no editor, já preenche com os AudioSources do objeto
            fontes = GetComponents<AudioSource>();
        }

        private void Update()
        {
            bool pausado = GerenciadorPausa.JogoPausado;
            if (pausado == estavaPausado) return;

            estavaPausado = pausado;

            foreach (AudioSource f in fontes)
            {
                if (f == null) continue;

                if (pausado) f.Pause();
                else f.UnPause();
            }
        }

        private void OnDisable()
        {
            // Se o objeto for desativado durante a pausa, não deixa o som preso
            estavaPausado = false;
        }
    }
}