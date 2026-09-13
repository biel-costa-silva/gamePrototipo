using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.Geral
{
    public class MenuPausaUI : MonoBehaviour
    {
        [Header("Paineis")]
        [SerializeField] private FadeCanvas painelPrincipal;
        [SerializeField] private FadeCanvas painelOpcoes;
        [SerializeField] private FadeCanvas painelVolumes;
        [SerializeField] private FadeCanvas painelControles;

        [Header("Fundo preto (esticar)")]
        [SerializeField] private RectTransform fundoPreto;
        [SerializeField] private float larguraPrincipal = 300f;
        [SerializeField] private float larguraOpcoes = 500f;
        [SerializeField] private float duracaoResize = 0.25f;

        private Coroutine resizeAtual;

        //Principal <-> Opções
        public void AbrirOpcoes()
        {
            painelPrincipal.Esconder(() => painelOpcoes.Mostrar());
            AnimarLargura(larguraOpcoes);
        }

        public void FecharOpcoes()
        {
            painelOpcoes.Esconder(() => painelPrincipal.Mostrar());
            AnimarLargura(larguraPrincipal);
        }

        //Volume <-> Controles

        public void AbrirVolume()
        {
            painelControles.Esconder(() => painelVolumes.Mostrar());
        }
        public void AbrirControles()
        {
            painelVolumes.Esconder(() => painelControles.Mostrar());
        }

        public void ResetarParaPrincipal()
        {
            if (resizeAtual != null) StopCoroutine(resizeAtual);

            painelOpcoes.EsconderImediato();
            painelVolumes.EsconderImediato();
            painelControles.EsconderImediato();
            painelPrincipal.MostrarImediato();

            fundoPreto.sizeDelta = new Vector2(larguraPrincipal, fundoPreto.sizeDelta.y);
        }

        private void AnimarLargura(float larguraAlvo)
        {
            if (resizeAtual != null) StopCoroutine(resizeAtual);
            resizeAtual = StartCoroutine(Resize(larguraAlvo));
        }

        private IEnumerator Resize(float larguraAlvo)
        {
            float larguraInicial = fundoPreto.sizeDelta.x;
            float tempoPassado = 0f;

            while (tempoPassado < duracaoResize)
            {
                tempoPassado += Time.unscaledDeltaTime; // funciona com timeScale = 0
                float t = tempoPassado / duracaoResize;
                t = t * t * (3f - 2f * t); // smoothstep: acelera e desacelera suavemente

                float largura = Mathf.Lerp(larguraInicial, larguraAlvo, t);
                fundoPreto.sizeDelta = new Vector2(largura, fundoPreto.sizeDelta.y);
                yield return null;
            }

            fundoPreto.sizeDelta = new Vector2(larguraAlvo, fundoPreto.sizeDelta.y);
        }

    }
}
