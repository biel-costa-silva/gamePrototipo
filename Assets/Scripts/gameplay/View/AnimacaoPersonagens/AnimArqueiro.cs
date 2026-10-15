
namespace Assets.Scripts.Gameplay.View.AnimacaoPersonagens
{

    public class AnimArqueiro : ControladorAnim
    {
        // ----------------  Métodos de acionamento ------------------
        public void AnimacaoRolando()
        {
            animacaoTerminou = false;
            animator.SetTrigger("rolar");
        }

        public void AnimacaoPuxando()
        {
            animacaoTerminou = false;
            animator.SetTrigger("puxar");
        }

        public void AnimacaoDispararCarregado()
        {
            animacaoTerminou = false;
            animator.SetTrigger("dispararCarregado");
        }

        public override void ResetarTriggers()
        {
            base.ResetarTriggers();
            animator.ResetTrigger("puxar");
            animator.ResetTrigger("dispararCarregado");

        }

        // ----------------------- Método de controle de tempo de animação -------------------- #

        // ------------------------------------------------------------------------------------ #

        // EVENTOS ESPECIFICOS DO ARQUEIRO!! --------------------------------------------------


    }
}