using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Peoes.OficiosPeoes
{
    public class OficiosGuerreiro : OficiosJogador
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

        public void ReceberChoque()
        {
            SpawnarVFX(2);
            rb.AddForce(new Vector2(10, 0), ForceMode2D.Impulse);
        }
        public void AplicarImpulsoCustom(float forca)
        {
            rb.AddForce(new Vector2(forca, 0), ForceMode2D.Impulse);
        }
    }
}