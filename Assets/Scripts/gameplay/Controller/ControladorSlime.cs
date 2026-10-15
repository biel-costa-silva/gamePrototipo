using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens.EnumsInimigos;
using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;
using Assets.Scripts.Gameplay.View.AnimacaoPersonagens;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Controller
{
    public class ControladorSlime : ControladorInimigo
    {
        public Slime slime;
        private FisicaSlime fisicaSlime;
        private AnimSlime animSlime;

        protected override void Awake()
        {
            base.Awake();
            fisicaSlime = fisica as FisicaSlime;
            animSlime = animacao as AnimSlime;
            slime = inimigo as Slime;
        }
        private void Start()
        {
            OnEstadoAlterado += (c, estado) => Debug.Log($"Slime -> {estado}");
        }
        protected override Inimigo CriarInimigo()
        {
            return new Slime();
        }

        protected override int EscolherVariacaoDano()
        {
            bool noAr = estadoAtual == EstadoInimigo.Andando && animSlime.noAr;
            return noAr ? 1 : 0;
        }

        public override void AplicarGolpe(int indice)
        {
            Debug.Log($"[Slime] AplicarGolpe indice={indice}");
            if (indice == 1) //ataque parabola
                fisicaSlime.AplicarGolpeProjetil(this, inimigo.GetDano(), indice, comportamento.PosicaoAlvo());
            else
                base.AplicarGolpe(indice);
        }
        protected override void AoReceberGolpe(HitBox golpe, int variacao)
        {
            bool noAr = variacao == 1;
            fisicaSlime.VfxGolpe(noAr, golpe.direcao);
        }
    }
}