using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public class VFXController : MonoBehaviour
    {
        [SerializeField] private GameObject ataqueDerivado;

        [Header("Dano do ataque derivado")]
        [Tooltip("0 = usa o dano herdado de quem gerou. Maior que 0 = usa este valor fixo.")]
        [SerializeField] private int danoFixo = 0;

        private Personagem origem;
        private bool origemEhInimigo;
        private int dano;
        private float direcao;

        public void Inicializar(HitBox hbOrigem)
        {
            origem = hbOrigem.origem;
            origemEhInimigo = hbOrigem.origemEhInimigo;
            dano = hbOrigem.dano;
            direcao = hbOrigem.direcao;
        }

        public void DestruiObjVFX()
        {
            Destroy(gameObject);
        }

        public void GerarAtaqueDerivado()
        {
            if (ataqueDerivado == null) return;

            GameObject ataque = Instantiate(ataqueDerivado, transform.position, transform.rotation);

            HitBox hb = ataque.GetComponent<HitBox>();
            if (hb == null) return;   // ataque derivado sem HitBox: só visual

            if (origem == null)
            {
                Debug.LogWarning("[VFXController] sem origem: o ataque derivado não vai causar dano.");
                return;
            }

            int danoFinal = danoFixo > 0 ? danoFixo : dano;
            hb.Configurar(gameObject, origem, origemEhInimigo, danoFinal, direcao);
        }
        //Evento de dispaross
    }
}