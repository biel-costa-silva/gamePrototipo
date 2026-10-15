using UnityEngine;

namespace Assets.Scripts.Gameplay.View.EntradaDados
{
    public interface IComportamento
    {
        float ComandoMovimento();// -1 esq, 0 parado, 1 dir
        int ComandoAtaque();// 0 não ataca, 1..n qual ataque?
        Vector2 PosicaoAlvo();
       
    }
}