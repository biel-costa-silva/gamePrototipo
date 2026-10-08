using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public class FisicaArqueiro : FisicaJogador
    {
        [Header("Rolagem")]
        [SerializeField] private float multiplicadorRolagem = 4f;
        [SerializeField] private AnimationCurve curvaRolagem = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.6f, 1f),   // mantém a velocidade em 60% da rolagem
            new Keyframe(1f, 0f)      // desacelera suavemente até parar
        );

        public override void AplicarImpulsoAtaque(int forca)
        {
            float direcao = sprite.flipX ? -1f : 1f;
            rb.AddForce(new Vector2(-direcao * (forca + 1) * 2, 0), ForceMode2D.Impulse);
        }

        public void AplicarVelocidadeRolagem(float velocidade, float t)
        {
            float direcao = sprite.flipX ? -1f : 1f;
            float v = velocidade * multiplicadorRolagem * curvaRolagem.Evaluate(t);
            rb.linearVelocity = new Vector2(direcao * v, rb.linearVelocity.y);
        }

        public void PararRolagem()
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }
}