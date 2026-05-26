using Assets.Scripts.Controller;
using Assets.Scripts.Model.Entidades.Peoes.UtilitariosPeoes;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Peoes.OficiosPeoes
{
    public class OficiosJogador : OficiosPersonagem
    {

        [SerializeField] protected GameObject[] ataques;
        [SerializeField] protected Transform posicaoPersonagem;

        public void AplicarGolpe(ControladorJogador dono, int dano, int indice)
        {
            float direcao = GetDirecao();
            GameObject atk = Instantiate(ataques[indice], posicaoPersonagem.position, posicaoPersonagem.rotation);

            Vector3 scale = atk.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direcao;
            atk.transform.localScale = scale;

            HitBox hitbox = atk.GetComponent<HitBox>();
            hitbox.Inicializar(dono, dano, direcao);
        }
    }
}