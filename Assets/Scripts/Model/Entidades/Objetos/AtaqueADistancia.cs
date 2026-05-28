using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Objetos
{
    public class AtaqueADistancia : MonoBehaviour
    {
        Rigidbody2D rb;
        public float velocidadeProjetil;
        private float direcao;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();   
        }
        public void Inicializar(float direcao)
        {
            this.direcao = direcao;
        }

        private void Update()
        {
            rb.AddForce(new Vector2(velocidadeProjetil * direcao, 0), ForceMode2D.Impulse);
        }

        private void OnBecameInvisible()
        {            
            Destroy(gameObject);
            Debug.Log("Destruido por estar fora de cena");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {           
            if (other.GetComponent<HitBox>() != null) return;
            Destroy (gameObject);
            Debug.Log("Destruido por bater em algo");
        }
    }
}