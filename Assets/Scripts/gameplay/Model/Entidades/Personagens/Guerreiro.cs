
namespace Assets.Scripts.Gameplay.Model.Entidades.Personagens
{
    public class Guerreiro : Jogador
    {
        
        public Guerreiro()
        {           
            nome = "Guerreiro";
            vida = 6;
            vidaMax = 6;
            dano = 2;
            defesa = 2;            
            velocidade = 3.5f;
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