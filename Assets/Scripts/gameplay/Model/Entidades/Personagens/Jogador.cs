
namespace Assets.Scripts.Gameplay.Model.Entidades.Personagens
{
    public class Jogador : Personagem
    {

        public virtual void RestaurarVida() // Se Morrer(método herdado) acontece recebe true e faça
        {
            vida = vidaMax;
        }
    }
}
