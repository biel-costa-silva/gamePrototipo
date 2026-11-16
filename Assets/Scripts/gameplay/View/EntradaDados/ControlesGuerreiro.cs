using UnityEngine;
using Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens;

namespace Assets.Scripts.Gameplay.View.EntradaDados
{
	public class ControlesGuerreiro : MonoBehaviour, IControles, IHabilidade
	{
        //controles gerais da interface
        public bool bloqueado { get; set; }
        public void Desligar()
        {
            this.enabled = false;
            bloqueado = true;
        }
        public void Ligar()
        {
            this.enabled = true;
            bloqueado = false;
        }


        public float ComandoMovimento()
        {
            if (bloqueado) return 0;
            if (Input.GetKey(KeyCode.A)) return -1f;
            if (Input.GetKey(KeyCode.D)) return 1f;
            return 0;
        }
        public int ComandoAtaque()
        {
            if (bloqueado) return 0;
            if (Input.GetKeyDown(KeyCode.J)) return 1;
            return 0;
        }
        public bool ComandoInteracao()
        {
            if (bloqueado) return false;
            if (Input.GetKeyDown(KeyCode.E)) return true;
            return false;
        }
        public bool ComandoSaqueArma()
        {
            if (bloqueado) return false;
            if (Input.GetKeyDown(KeyCode.Q)) return true;
            return false;
        }

        //--------------
        public bool ComandoAgachar()
        {
            if (bloqueado) return false;
            if (Input.GetKey(KeyCode.S)) return true;
            return false;
        }
        public bool Defender()
        {
            if (bloqueado) return false;
            if (Input.GetKeyDown(KeyCode.L)) return true;
            return false;
        }
    }
}