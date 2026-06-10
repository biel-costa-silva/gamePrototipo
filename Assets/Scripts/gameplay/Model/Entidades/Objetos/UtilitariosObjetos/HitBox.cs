using Assets.Scripts.Controller;
using Assets.Scripts.Model.Entidades.Peoes.UtilitariosPeoes;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos
{
    public class HitBox : MonoBehaviour, ICausamDano
    {
        public ControladorJogador origem;
        public int dano { get; set; }
        public float direcao { get; set; }

        //variaveis de controle       
        private BoxCollider2D boxColl;       
        public bool consumida = false;
       
        private void Awake()
        {
            boxColl = GetComponent<BoxCollider2D>();
        }

        public void Inicializar(ControladorJogador dono, int dano, float direcao)
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
            //choque entre ataques
            HitBox outraHitBox = other.GetComponent<HitBox>();
            if (outraHitBox != null && outraHitBox.origem != origem)
            {
                Debug.Log("Choque entre ataques");
                consumida = true;
                outraHitBox.consumida = true;

                origem.jogador.sofreuChoque = true;
                outraHitBox.origem.jogador.sofreuChoque = true;

                Destroy(outraHitBox.gameObject);
                Destroy(gameObject);
                return;
            }            
        }       
        //Evento de Controle para Frame De Dano
        public void AtivarHitbox() => boxColl.enabled = true;
        public void DesativarHitBox() => boxColl.enabled = false;
        public void DestroirObj() => Destroy(gameObject);
    }
}
