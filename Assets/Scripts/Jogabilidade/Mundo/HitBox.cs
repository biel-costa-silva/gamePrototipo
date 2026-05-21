using Assets.Scripts.Nucleo.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Jogabilidade.Mundo
{
    public class HitBox : MonoBehaviour, ICausamDano
    {
        public Personagem origem;
        public int dano { get; set; }
        public float direcao { get; set; }

        //variaveis de controle       
        private BoxCollider2D boxColl;      

        private void Awake()
        {
            boxColl = GetComponent<BoxCollider2D>();
        }

        public void Inicializar(Personagem dono, int dano, float direcao)
        {
            origem = dono;
            this.dano = dano;
            this.direcao = direcao;

            foreach (Collider2D col in origem.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(boxColl, col);
            }
        }


        public void OnTriggerEnter2D(Collider2D other)
        {
           HitBox outraHitBoox = other.GetComponent<HitBox>(); 

            if (outraHitBoox != null && outraHitBoox.origem != origem)
            {
               
                Debug.Log("Choque entre ataques");
                origem.sofreuChoque = true;
                outraHitBoox.origem.sofreuChoque = true;

                Destroy(outraHitBoox.gameObject);
                Destroy(gameObject);
            }            
        }

        //Evento de Controle para Frame De Dano
        public void AtivarHitbox()
        {
            boxColl.enabled = true;
        }
        public void DesativarHitBox()
        {
            boxColl.enabled = false;
        }

        //EVENTOS
        public void DestroirObj()
        {
            Destroy(gameObject);
        }
    }
}