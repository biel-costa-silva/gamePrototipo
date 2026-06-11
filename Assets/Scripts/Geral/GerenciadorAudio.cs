using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Geral
{
    public class GerenciadorAudio : MonoBehaviour
    {
        public static GerenciadorAudio instancia;

        [SerializeField]
        private BibliotecaAudios sfxBiblioteca;
        [SerializeField]
        private AudioSource sfx2DFonte;

        private void Awake()
        {
            if(instancia != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instancia = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void TocarAudio3D(AudioClip clip, Vector3 pos)
        {
            if(clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, pos);
            }
        }
        public void TocarAudio3D(string nomeAudio, Vector3 pos)
        {
            TocarAudio3D(sfxBiblioteca.GetClipNome(nomeAudio), pos);
        }

        public void TocarAudio2D(string nomeAudio)
        {
            sfx2DFonte.PlayOneShot(sfxBiblioteca.GetClipNome(nomeAudio));
        }
    }
}