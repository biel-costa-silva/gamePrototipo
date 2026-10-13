using Assets.Scripts.Gameplay.Controller;
using Assets.Scripts.Model.Entidades.Objetos;
using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public class FisicaInimigo : FisicaPersonagem
    {

        [SerializeField] protected GameObject[] ataques;
        [SerializeField] protected Transform posicaoInimigo;
        [SerializeField] private bool prefabsOlhamParaEsquerda = false;

        [Header("Knockback")]
        [SerializeField] private float knockbackPorDano = 1.5f;
        [SerializeField] private float knockbackMaximo = 5f;

        public override void AplicarImpulsoGolpeRecebido(HitBox golpe)
        {
            float velocidade = Mathf.Min(golpe.dano * knockbackPorDano, knockbackMaximo);
            rb.linearVelocity = new Vector2(golpe.direcao * velocidade, rb.linearVelocity.y);
        }

        public void AplicarGolpe(ControladorInimigo dono, int dano, int indice)
        {
            float direcao = GetDirecao();
            GameObject atk = Instantiate(ataques[indice], posicaoInimigo.position, posicaoInimigo.rotation);

            Vector3 scale = atk.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direcao * (prefabsOlhamParaEsquerda ? -1f :1f);
            atk.transform.localScale = scale;

            HitBox hitbox = atk.GetComponent<HitBox>();
            hitbox.Inicializar(dono, dano, direcao);

            AtaqueADistancia ataqueADistancia = atk.GetComponent<AtaqueADistancia>();
            if (ataqueADistancia != null)
            {
                ataqueADistancia.Inicializar(direcao);
            }
        }
        public void Parar()
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        public void VirarPara(float direcao)
        {
            if (direcao == 0f) return;
            DefinirOlhar(direcao < 0f);
        }

        protected override void LimitarNaCamera()
        {
            //não são limitados
        }
    }
}