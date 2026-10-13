using Assets.Scripts.Gameplay.Controller;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public class FisicaArqueiro : FisicaJogador
    {
        [SerializeField] private GameObject[] prefabsVFX;

        [Header("Rolagem")]
        [SerializeField] private float multiplicadorRolagem = 4f;
        [SerializeField] private AnimationCurve curvaRolagem = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.6f, 1f),   // mantém a velocidade em 60% da rolagem
            new Keyframe(1f, 0f)      // desacelera suavemente até parar
        );
        private readonly List<(Collider2D meu, Collider2D dele)> pares = new List<(Collider2D, Collider2D)>();
        public void AtravessarInimigos(bool atravessar)
        {
            if (atravessar)
            {
                foreach (ControladorInimigo inimigo in ControladorInimigo.Ativos)
                    foreach (Collider2D dele in inimigo.GetComponents<Collider2D>())
                        foreach (Collider2D meu in GetComponents<Collider2D>())
                        {
                            Physics2D.IgnoreCollision(meu, dele, true);
                            pares.Add((meu, dele));
                        }
            }
            else
            {
                foreach (var par in pares)
                    if (par.meu != null && par.dele != null)
                        Physics2D.IgnoreCollision(par.meu, par.dele, false);
                pares.Clear();
            }
        }
        public void SpawnarVFX(int indice)
        {
            float direcao = GetDirecao();
            GameObject vfx = Instantiate(prefabsVFX[indice], posicaoPersonagem.position, posicaoPersonagem.rotation);

            Vector3 scale = vfx.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direcao;
            vfx.transform.localScale = scale;
        }

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