using Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes;
using Assets.Scripts.View.AnimacaoPeoes;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Peoes
{
	public class Arqueiro : Jogador
	{
        public Arqueiro()
        {
            nome = "Arqueiro";
            vida = 4;
            dano = 3;
            defesa = 0; 
            velocidade = 5f;
            velocidadeBase = velocidade;
        }
       
    }
}