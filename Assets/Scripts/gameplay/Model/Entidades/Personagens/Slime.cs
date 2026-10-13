using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Personagens
{
    public class Slime : Inimigo
    {

        public Slime()
        {
            nome = "Slime";
            vida = 10;
            vidaMax = 10;
            dano = 1;
            defesa = 0;
            velocidade = 2f;
            velocidadeBase = velocidade;
        }
    }
}