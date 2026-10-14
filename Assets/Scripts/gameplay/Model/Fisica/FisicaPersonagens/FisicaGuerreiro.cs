using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public class FisicaGuerreiro : FisicaJogador
    {

        [SerializeField] private GameObject[] prefabsVFX;

        public void SpawnarVFX(int indice)
        {
            float direcao = GetDirecao();
            GameObject vfx = Instantiate(prefabsVFX[indice], posicaoPersonagem.position, posicaoPersonagem.rotation);

            Vector3 scale = vfx.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direcao;
            vfx.transform.localScale = scale;
        }

        public void ReceberChoque(float direcao)
        {
            SpawnarVFX(2);
            rb.AddForce(new Vector2(10 * direcao, 0), ForceMode2D.Impulse);
        }
        public void AplicarImpulsoCustom(float forca, float direcao)
        {
            rb.AddForce(new Vector2(forca * direcao, 0), ForceMode2D.Impulse);
        }

        protected override int CalcularDanoDoAtaque(int dano, int indice)
        {
            int danoFinal = dano;

            if (indice == 0) danoFinal = dano - 1;
            if (indice == 1) danoFinal = dano;
            if (indice == 2) danoFinal = dano + 1;

            return danoFinal;
        }
    }
}