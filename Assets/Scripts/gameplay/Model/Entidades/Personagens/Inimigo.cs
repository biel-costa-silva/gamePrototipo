
namespace Assets.Scripts.Gameplay.Model.Entidades.Personagens
{    public class Inimigo : Personagem
    {
        public void EscalarAtributos(float fatorVida, float fatorDano)
        {
            vidaMax = System.Math.Max(1, (int)System.Math.Round(vidaMax * fatorVida));
            vida = vidaMax;
            dano = System.Math.Max(1, (int)System.Math.Round(dano * fatorDano));
        }
    }

}

