using Assets.Scripts.Controller;
using Assets.Scripts.Gameplay.Controller;
using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos
{
    public class HitBox : MonoBehaviour, ICausamDano
    {
        public Personagem origem;
        public bool origemEhInimigo { get; private set; }
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
            Configurar(dono.gameObject, dono.jogador, false, dano, direcao);
        }
        public void Inicializar(ControladorInimigo dono, int dano, float direcao)
        {
            Configurar(dono.gameObject, dono.inimigo, true, dano, direcao);
        }

        public void Configurar(GameObject dono, Personagem modelo, bool ehInimigo, int dano, float direcao)
        {
            origem = modelo;
            origemEhInimigo = ehInimigo;
            this.dano = dano;
            this.direcao = direcao;

            foreach (Collider2D col in dono.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(boxColl, col);
            }
        }



        public void OnTriggerEnter2D(Collider2D other)
        {
            //choque entre ataques
            HitBox outraHitBox = other.GetComponent<HitBox>();
            if (outraHitBox == null) return;

            if (origem == null || outraHitBox.origem == null) return;
            if (outraHitBox.origem == origem) return;
            if (origemEhInimigo && outraHitBox.origemEhInimigo) return;


            Debug.Log("Choque entre ataques");
            consumida = true;
            outraHitBox.consumida = true;

            origem.sofreuChoque = true;
            outraHitBox.origem.sofreuChoque = true;

            Destroy(outraHitBox.gameObject);
            Destroy(gameObject);

        }
        //Evento de Controle para Frame De Dano
        public void AtivarHitbox() => boxColl.enabled = true;
        public void DesativarHitBox() => boxColl.enabled = false;
        public void DestroirObj() => Destroy(gameObject);
    }
}
