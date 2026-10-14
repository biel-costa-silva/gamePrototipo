using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public abstract class FisicaPersonagem : MonoBehaviour
    {
        //componentes
        //public VidaUIController vida;
        protected Rigidbody2D rb;
        protected SpriteRenderer sprite;
        protected BoxCollider2D boxColliderP1;
        public Camera camera;

        //variaveis de controle
        [SerializeField] protected bool arteOlhaParaEsquerda = false;
        protected float offsetXBase = -0.01f;

        private HashSet<HitBox> golpesRecebidos = new HashSet<HitBox>();
        private Queue<HitBox> golpesPendentes = new Queue<HitBox>();
        private IInteracoes interagivelPendente = null;


        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            boxColliderP1 = GetComponent<BoxCollider2D>();
        }

        private void LateUpdate()
        {
            CorrigirColliderFlip();
            LimitarNaCamera();
        }

        protected virtual void LimitarNaCamera()
        {
            //personagens não ultrapassam a camera
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, camera.GetLimiteEsquerdo(), camera.GetLimiteDireito());
            transform.position = pos;
        }

        //movimentos
        public void Locomover(float direcao, float velocidade)
        {
            rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);
            DefinirOlhar(direcao < 0);
        }

        //impulsos
        public virtual void AplicarImpulsoAtaque(int forca)
        {
            float direcao = GetDirecao();
            rb.AddForce(new Vector2(direcao * (forca + 4) * 2, 0), ForceMode2D.Impulse);
        }

        public virtual void AplicarImpulsoGolpeRecebido(HitBox golpe)
        {
            rb.AddForce(new Vector2(golpe.direcao * (golpe.dano + 1) * 6, 0), ForceMode2D.Impulse);
        }


        //direcao
        protected void DefinirOlhar(bool paraEsquerda)
        {
            sprite.flipX = arteOlhaParaEsquerda ? !paraEsquerda : paraEsquerda;
        }
        public float GetDirecao()
        {
            bool paraEsquerda = arteOlhaParaEsquerda ? !sprite.flipX : sprite.flipX;
            return paraEsquerda ? -1f : 1f;
        }

        public bool EstaDeCostas(float direcaoGolpe)
        {
            return GetDirecao() == direcaoGolpe;
        }

        public void VirarParaLadoDoGolpe(float direcaoGolpe)
        {
            DefinirOlhar(direcaoGolpe > 0);
        }


        //correcao de colliders
        public void TrocarOffsetX(float novoValor)
        {
            offsetXBase = novoValor;
        }
        protected virtual void CorrigirColliderFlip()
        {
            if (sprite.flipX) boxColliderP1.offset = new Vector2(-offsetXBase, boxColliderP1.offset.y);
            else boxColliderP1.offset = new Vector2(offsetXBase, boxColliderP1.offset.y);
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
        public void EnfileirarGolpe(HitBox golpe)
        {
            if (golpe != null && golpe.origem != null && golpesRecebidos.Add(golpe))
                golpesPendentes.Enqueue(golpe);
        }

        public HitBox ConsumirGolpePendente()
        {
            if (golpesPendentes.Count > 0)
            {
                HitBox g = golpesPendentes.Dequeue();
                if (g == null) Debug.Log("Golpe destruído antes de ser processado");
                return g;
            }
            return null;
        }

        public IInteracoes GetInteragivelPendente()
        {
            return interagivelPendente;
        }
    }
}