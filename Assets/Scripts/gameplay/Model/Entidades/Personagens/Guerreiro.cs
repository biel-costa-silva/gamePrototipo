using Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes;
using Assets.Scripts.View.AnimacaoPeoes;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Peoes
{
    public class Guerreiro : Jogador
    {
        
        public Guerreiro()
        {           
            nome = "Guerreiro";
            vida = 6;
            dano = 2;
            defesa = 2;
            velocidade = 5;
            velocidadeBase = velocidade;
        }

        public int CalcularDanoRecebido(int danoRecebido, bool estaDefendendo, bool estaRepelindo)
        {
            if (estaRepelindo) return 0;           
            if (estaDefendendo) return danoRecebido - defesa; 
            return danoRecebido;                 
        }
    }
}