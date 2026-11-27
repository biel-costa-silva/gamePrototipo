using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Personagens
{
    public class Goblin : Inimigo
    {        
        public Goblin()
        {
            nome = "Goblin";
            vida = 12;
            vidaMax = 12;
            dano = 1;
            defesa = 0;
            velocidade = 3.2f;
            velocidadeBase = velocidade;
        }        
    }
}