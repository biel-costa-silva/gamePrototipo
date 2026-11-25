using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Geral
{
    public class GerenciadorAudio : MonoBehaviour
    {
        public static GerenciadorAudio instancia;

        [SerializeField]
        private BibliotecaAudios sfxBiblioteca;
        [SerializeField]
        private AudioSource sfx2DFonte;


        [Header("Sons 3D pontuais")]        
        [SerializeField] private AudioMixer mixer;   // arraste o asset do Mixer
        [SerializeField] private AudioMixerGroup grupoSons;   // o mesmo grupo "Sons" do Mixer



        [Header("Grupo de sons para LowPass")]
        [SerializeField] private string[] parametrosLowPass = { "LowPassMusica", "LowPassAmbiente" };
        private Coroutine rotinaAbafar;

        public void Abafar(bool ligado, float duracao = 0.3f)
        {
            float alvo = ligado ? 700f : 22000f;

            if (rotinaAbafar != null) StopCoroutine(rotinaAbafar);
            rotinaAbafar = StartCoroutine(TransicaoLowPass(alvo, duracao));
        }        

        private IEnumerator TransicaoLowPass(float alvo, float duracao)
        {
            mixer.GetFloat(parametrosLowPass[0], out float atual);
            float logDe = Mathf.Log10(atual);
            float logPara = Mathf.Log10(alvo);

            for (float t = 0; t < 1f; t += Time.unscaledDeltaTime / duracao)
            {
                float valor = Mathf.Pow(10f, Mathf.Lerp(logDe, logPara, t));
                foreach (string p in parametrosLowPass) mixer.SetFloat(p, valor);
                yield return null;
            }

            foreach (string p in parametrosLowPass) mixer.SetFloat(p, alvo);

            mixer.SetFloat(parametrosLowPass[0], alvo);
            mixer.SetFloat(parametrosLowPass[1], alvo);
        }


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
            if (clip == null) return;

            GameObject obj = new GameObject("SFX3D");
            obj.transform.position = pos;

            AudioSource fonte = obj.AddComponent<AudioSource>();
            fonte.clip = clip;
            fonte.outputAudioMixerGroup = grupoSons;   // <- o que faltava
            fonte.spatialBlend = 1f;
            fonte.rolloffMode = AudioRolloffMode.Logarithmic;
            fonte.minDistance = 1f;
            fonte.maxDistance = 500f;
            fonte.Play();

            Destroy(obj, clip.length);
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