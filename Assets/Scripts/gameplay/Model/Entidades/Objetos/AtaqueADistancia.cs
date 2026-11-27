using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
{
    public class AtaqueADistancia : MonoBehaviour
    {
        Rigidbody2D rb;
        public float velocidadeProjetil;
        private float direcao;
        private bool destruido = false;

        [SerializeField] private GameObject[] prefabsVFX;

        public void SpawnarVFX(int indice)
        {
            if (prefabsVFX == null || indice >= prefabsVFX.Length || prefabsVFX[indice] == null) return;

            GameObject vfx = Instantiate(prefabsVFX[indice], transform.position, transform.rotation);

            Vector3 scale = vfx.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direcao;
            vfx.transform.localScale = scale;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Inicializar(float direcao)
        {
            this.direcao = direcao;
            rb.linearVelocity = new Vector2(velocidadeProjetil * direcao, 0f);
        }

        // Destruição "normal": toca o VFX uma única vez. Também é chamada pelo HitBox no choque entre ataques.
        public void DestruirComVFX()
        {
            if (destruido) return;
            destruido = true;

            SpawnarVFX(0);
            Destroy(gameObject);
        }

        private void OnBecameInvisible()
        {
            if (destruido) return;
            destruido = true;
            Destroy(gameObject); // fora da tela: sem VFX
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Choque com outra HitBox é resolvido pelo HitBox (que chama DestruirComVFX)
            if (other.GetComponent<HitBox>() != null) return;
            StartCoroutine(DestruirNoProximoFrame());
        }

        private IEnumerator DestruirNoProximoFrame()
        {
            yield return null;
            DestruirComVFX();
        }
    }
}