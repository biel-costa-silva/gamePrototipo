using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ProjetilParabolico : MonoBehaviour
    {
        [SerializeField] private float margemDeVida = 1f;
        private Rigidbody2D rb;
        [SerializeField] private Transform posicaoBola;
        [SerializeField] private GameObject vfxExplosaoAr;
        [SerializeField] private GameObject vfxExplosaoChao;

        [Header("Espinho no chão")]
        [SerializeField] private GameObject ataqueDerivado;

        private bool explodiu = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Calcula a velocidade inicial que faz a bola chegar em 'alvo' depois de 'tempoDeVoo'
        public void Lancar(Vector2 alvo, float tempoDeVoo)
        {
            Vector2 delta = alvo - (Vector2)transform.position;
            float g = Physics2D.gravity.y * rb.gravityScale;

            float vx = delta.x / tempoDeVoo;
            float vy = (delta.y - 0.5f * g * tempoDeVoo * tempoDeVoo) / tempoDeVoo;

            rb.linearVelocity = new Vector2(vx, vy);
            Destroy(gameObject, margemDeVida); // rede de segurança caso não bata em nada
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (explodiu) return;
            if (other.GetComponentInParent<FisicaInimigo>() != null) return;

            explodiu = true;

            if (other.CompareTag("Chao")) StartCoroutine(RotinaExplodirNoChao());
            else StartCoroutine(RotinaExplodirNoAr());
        }
        private IEnumerator RotinaExplodirNoAr()
        {
            yield return null;
            CriarExplosaoAr();
            Destroy(gameObject);
        }
        private IEnumerator RotinaExplodirNoChao()
        {
            yield return null;
            CriarExplosaoChao();            
            Destroy(gameObject);
        }
        private void CriarExplosaoAr()
        {
            if (vfxExplosaoAr != null)
                Instantiate(vfxExplosaoAr, posicaoBola.position, posicaoBola.rotation);
        }
        private void CriarExplosaoChao()
        {
            if (vfxExplosaoChao != null) ;
            // 1. cria a explosão e guarda a referência a ela (fx)
            GameObject fx = Instantiate(vfxExplosaoChao, posicaoBola.position, posicaoBola.rotation);

            // 2. pega o VFXController da explosão e o HitBox desta bola
            VFXController vfx = fx.GetComponent<VFXController>();
            HitBox hb = GetComponent<HitBox>();

            // 3. entrega os dados da bola para a explosão
            if (vfx != null && hb != null) vfx.Inicializar(hb);
        }
    }
}