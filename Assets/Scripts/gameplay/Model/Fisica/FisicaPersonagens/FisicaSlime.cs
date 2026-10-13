using Assets.Scripts.Gameplay.Controller;
using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public class FisicaSlime : FisicaInimigo
    {
        [Header("Projétil parabólico")]
        [SerializeField] private float velocidadeHorizontal = 6f;
        [SerializeField] private float tempoDeVooMin = 0.6f;
        [SerializeField] private float tempoDeVooMax = 1.4f;
        [SerializeField] private float alturaMira = 0.5f; // mira no corpo, não no pé

        public enum VfxSlime { Andando = 0, GoticulasAr = 1, GolpeAr = 2, GolpeChao = 3 }

        [Header("VFX")]
        [SerializeField] private GameObject[] prefabsVFX;   // mesma ordem do enum
        [SerializeField] private bool vfxOlhamParaEsquerda = false;

        private void SpawnarVFX(VfxSlime tipo, float direcao)
        {
            GameObject prefab = prefabsVFX[(int)tipo];
            if (prefab == null) return;

            GameObject vfx = Instantiate(prefab, posicaoInimigo.position, Quaternion.identity);

            Vector3 s = vfx.transform.localScale;
            s.x = Mathf.Abs(s.x) * direcao * (vfxOlhamParaEsquerda ? -1f : 1f);
            vfx.transform.localScale = s;
        }

        public void VfxAndando()
        {
            Bounds b = boxColliderP1.bounds;
            SpawnarVFX(VfxSlime.Andando, -GetDirecao());
        }

        public void VfxGoticulas()
            => SpawnarVFX(VfxSlime.GoticulasAr, 1f);

        public void VfxGolpe(bool noAr, float direcaoGolpe)
            => SpawnarVFX(noAr ? VfxSlime.GolpeAr : VfxSlime.GolpeChao, direcaoGolpe);

        public void AplicarGolpeProjetil(ControladorInimigo dono, int dano, int indice, Vector2 alvo)
        {
            alvo += Vector2.up * alturaMira;

            Vector3 local = posicaoInimigo.localPosition;            
            float dx = alvo.x - posicaoInimigo.position.x;
            float direcao = dx >= 0f ? 1f : -1f;

            GameObject bola = Instantiate(ataques[indice], posicaoInimigo.position, Quaternion.identity);
            Debug.Log($"[Slime] Bola criada em {posicaoInimigo.position}, alvo {alvo}");

            HitBox hitbox = bola.GetComponent<HitBox>();
            hitbox.Inicializar(dono, dano, direcao);

            float tempoDeVoo = Mathf.Clamp(Mathf.Abs(dx) / velocidadeHorizontal, tempoDeVooMin, tempoDeVooMax);
            bola.GetComponent<ProjetilParabolico>().Lancar(alvo, tempoDeVoo);
        }
        protected override void CorrigirColliderFlip()
        {
            float x = GetDirecao() < 0 ? -offsetXBase : offsetXBase;
            boxColliderP1.offset = new Vector2(x, boxColliderP1.offset.y);
        }
    }
}