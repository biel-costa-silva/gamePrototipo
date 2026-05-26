using UnityEngine;
using System.Collections;

namespace Assets.Scripts.View.EntradaDados
{
	public interface IControles
	{
		// Jogadores e inimigos
		public float ComandoMovimento();//para o método Andar
		public int ComandoAtaque();//para o método Atacar
		public bool ComandoInteracao();//para o método Interagir

		//Jogadores
		public bool ComandoSaqueArma();
		
		//Guerreiro

		//Arqueiro
	}
}