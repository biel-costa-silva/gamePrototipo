using Assets.Scripts.Nucleo.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Jogabilidade.Mundo
{
    public class HitBox : MonoBehaviour, ICausamDano
    {
        public Personagem origem;
        public int dano { get; set; }
        public float direcao { get; set; }

        //variaveis de controle       
        private BoxCollider2D boxColl;
        private HitBox outraHitBoxColidida;
        private Personagem outroPersonagemColidido;
        public bool consumida = false;
        private bool processando = false;

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
            HitBox outraHitBox = other.GetComponent<HitBox>();

            if (outraHitBox != null && outraHitBox.origem != origem)
            {                
                Debug.Log("Choque entre ataques");
                consumida = true;
                outraHitBox.consumida = true;

                origem.sofreuChoque = true;
                outraHitBox.origem.sofreuChoque = true;

                Destroy(outraHitBox.gameObject);
                Destroy(gameObject);
                return;
            }
            Personagem personagem = other.GetComponent<Personagem>();
            if (personagem != null && personagem != origem)
            {
                StartCoroutine(ProcessarColisoes());
            }          
            
        }        
        private IEnumerator ProcessarColisoes()
        {
            if (processando) yield break;
            processando = true;

            yield return null;
            
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