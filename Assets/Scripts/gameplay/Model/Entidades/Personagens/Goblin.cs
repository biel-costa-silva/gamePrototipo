using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Personagens
{
    public class Goblin : Inimigo
    {        
        public Goblin()
        {
            nome = "Goblin";
            vida = 20;
            vidaMax = 20;
            dano = 1;
            defesa = 0;
            velocidade = 2.5f;
            velocidadeBase = velocidade;
        }        
    }
}