using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public class FisicaArqueiro : FisicaJogador
    {
        //impulso
        public override void AplicarImpulsoAtaque(int forca)
        {
            float direcao = sprite.flipX ? -1f : 1f;
            rb.AddForce(new Vector2(direcao * (forca + 5) * 2, 0), ForceMode2D.Impulse);
        }
    }
}