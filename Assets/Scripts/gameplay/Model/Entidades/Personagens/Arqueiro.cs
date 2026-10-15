
namespace Assets.Scripts.Gameplay.Model.Entidades.Personagens
{
	public class Arqueiro : Jogador
	{
        public Arqueiro()
        {
            nome = "Arqueiro";
            vida = 4;
            vidaMax = 4;
            dano = 2;
            defesa = 0; 
            velocidade = 3.5f;
            velocidadeBase = velocidade;
        }
       
    }
}