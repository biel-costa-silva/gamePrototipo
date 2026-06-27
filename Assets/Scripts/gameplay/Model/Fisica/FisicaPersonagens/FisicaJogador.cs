using Assets.Scripts.Controller;
using Assets.Scripts.Model.Entidades.Objetos;
using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public class FisicaJogador : FisicaPersonagem
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

            AtaqueADistancia ataqueADistancia = atk.GetComponent<AtaqueADistancia>();
            if (ataqueADistancia != null)
            {
                ataqueADistancia.Inicializar(direcao);
            }
        }
    }
}