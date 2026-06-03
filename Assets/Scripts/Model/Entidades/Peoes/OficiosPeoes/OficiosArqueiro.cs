using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Peoes.OficiosPeoes
{
    public class OficiosArqueiro : OficiosJogador
    {
        //impulso
        public override void AplicarImpulsoAtaque(int forca)
        {
            float direcao = sprite.flipX ? -1f : 1f;
            rb.AddForce(new Vector2(direcao * (forca + 5) * 2, 0), ForceMode2D.Impulse);
        }
    }
}