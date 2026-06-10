using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Peoes.OficiosPeoes
{
    public abstract class OficiosPersonagem : MonoBehaviour
    {
        //componentes
        protected Rigidbody2D rb;
        protected SpriteRenderer sprite;
        protected BoxCollider2D boxCollider;

        //variaveis de controle
        private float offsetXBase = -0.01f;
        private HashSet<HitBox> golpesRecebidos = new HashSet<HitBox>();
        private Queue<HitBox> golpesPendentes = new Queue<HitBox>();
        private IInteracoes interagivelPendente = null;


        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void LateUpdate()
        {
            CorrigirColliderFlip();
        }



        //movimentos
        public void Locomover(float direcao, float velocidade)
        {
            rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);
            sprite.flipX = direcao < 0;
        }

        //impulsos
        public virtual void AplicarImpulsoAtaque(int forca)
        {
            float direcao = sprite.flipX ? -1f : 1f;
            rb.AddForce(new Vector2(direcao * (forca + 8) * 2, 0), ForceMode2D.Impulse);
        }
        public void AplicarImpulsoGolpeRecebido(HitBox golpe)
        {
            rb.AddForce(new Vector2(golpe.direcao * golpe.dano * 12, 0), ForceMode2D.Impulse);
        }

        //direcao
        public float GetDirecao()
        {
            return sprite.flipX ? -1f : 1f;
        }
        public bool EstaDeCostas(float direcaoGolpe)
        {
            return GetDirecao() == direcaoGolpe;
        }
        public void VirarParaLadoDoGolpe(float direcaoGolpe)
        {
            sprite.flipX = direcaoGolpe > 0;
        }

        //correcao de colliders
        public void TrocarOffsetX(float novoValor)
        {
            offsetXBase = novoValor;
        }
        private void CorrigirColliderFlip()
        {
            if (sprite.flipX) boxCollider.offset = new Vector2(-offsetXBase, boxCollider.offset.y);
            else boxCollider.offset = new Vector2(offsetXBase, boxCollider.offset.y);
        }

        //Detectados
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            HitBox golpe = other.GetComponent<HitBox>();
            if (golpe != null)
            {
                Debug.Log($"OnTriggerEnter2D — golpe detectado. Já no HashSet: {!golpesRecebidos.Contains(golpe)}. Consumida: {golpe.consumida}");
                if (golpe.origem != null && golpesRecebidos.Add(golpe))
                {
                    Debug.Log("Golpe adicionado à fila");
                    golpesPendentes.Enqueue(golpe);
                }
                return;
            }

            IInteracoes interagivel = other.GetComponent<IInteracoes>();
            if (interagivel != null)
                interagivelPendente = interagivel;
        }
        public virtual void OnTriggerExit2D(Collider2D other)
        {
            HitBox golpe = other.GetComponent<HitBox>();
            if (golpe != null && golpe.consumida)
            {
                golpesRecebidos.Remove(golpe);
                return;
            }

            IInteracoes interagivel = other.GetComponent<IInteracoes>();
            if (interagivel != null)
                interagivelPendente = null;
        }

        public HitBox ConsumirGolpePendente()
        {
            if (golpesPendentes.Count > 0) return golpesPendentes.Dequeue();
            return null;
        }

        public IInteracoes GetInteragivelPendente()
        {
            return interagivelPendente;
        }
    }
}