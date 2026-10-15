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
            Debug.Log($"[Bola] tocou em: {other.name}");
            if (other.GetComponent<HitBox>() != null) return;                 // choque é resolvido no HitBox
            if (other.GetComponentInParent<FisicaInimigo>() != null) return;  // não explode em outros inimigos
            StartCoroutine(DestruirNoProximoFrame());
        }

        private IEnumerator DestruirNoProximoFrame()
        {
            yield return null;
            Destroy(gameObject);
        }
    }
}