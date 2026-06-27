using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;

namespace Assets.Scripts.View.EntradaDados
{
	public class ControlesGuerreiro : MonoBehaviour, IControles, IHabilidade
	{
        //controles gerais da interface
        public float ComandoMovimento()
        {            
            if (Input.GetKey(KeyCode.A)) return -1f;
            if (Input.GetKey(KeyCode.D)) return 1f;
            return 0;
        }
        public int ComandoAtaque()
        {
            if(Input.GetKeyDown(KeyCode.J)) return 1;
            return 0;
        }
        public bool ComandoInteracao()
        {
            if (Input.GetKeyDown(KeyCode.E)) return true;
            return false;
        }
        public bool ComandoSaqueArma()
        {
            if (Input.GetKeyDown(KeyCode.Q)) return true;
            return false;
        }
        public bool ComandoAgachar()
        {
            if (Input.GetKey(KeyCode.S)) return true;
            return false;
        }
        public bool Defender()
        {
            if (Input.GetKeyDown(KeyCode.L)) return true;
            return false;
        }
    }
}