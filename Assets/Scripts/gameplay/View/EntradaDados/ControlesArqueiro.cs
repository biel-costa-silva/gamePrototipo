using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Gameplay.View.EntradaDados
{
    public class ControlesArqueiro : MonoBehaviour, IControles
    {      
        // controle do Arqueiro
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
            if (Input.GetKey(KeyCode.LeftArrow)) return -1f;
            if (Input.GetKey(KeyCode.RightArrow)) return 1f;
            return 0f;
        }
        public int ComandoAtaque()
        {
            if (bloqueado) return 0;
            if (Input.GetKeyDown(KeyCode.Mouse0)) return 1;//botao esquerdo mouse
            return 0;
        }
       
        public bool ComandoInteracao()
        {
            if (bloqueado) return false;
            if (Input.GetKeyDown(KeyCode.RightControl)) return true;
            return false;
        }

        public bool ComandoSaqueArma()
        {
            if (bloqueado) return false;
            if (Input.GetKeyDown(KeyCode.Mouse1)) return true;//botao direito mouse
            return false;
        }

        //------------
        public bool ComandoRolar()
        {
            if (bloqueado) return false;
            if (Input.GetKeyDown(KeyCode.DownArrow)) return true;
            return false;
        }

        public bool ComandoAtaqueCarregando()
        {
            if (bloqueado) return false;
            return Input.GetKey(KeyCode.Mouse0);
        }

        public bool ComandoAtaqueSolto()
        {
            if (bloqueado) return false;
            return Input.GetKeyUp(KeyCode.Mouse0);
        }
    }
}