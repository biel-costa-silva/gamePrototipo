using UnityEngine;
using System.Collections;

namespace Assets.Scripts.View.EntradaDados
{
    public class ControlesArqueiro : MonoBehaviour, IControles
    {      
        // controle do Arqueiro
        public float ComandoMovimento()
        {
            if (Input.GetKey(KeyCode.LeftArrow)) return -1f;
            if (Input.GetKey(KeyCode.RightArrow)) return 1f;
            return 0f;
        }
        public int ComandoAtaque()
        {
            if(Input.GetKeyDown(KeyCode.Mouse0)) return 1;
            return 0;
        }
       
        public bool ComandoInteracao()
        {
            if (Input.GetKeyDown(KeyCode.RightControl)) return true;
            return false;
        }

        public bool ComandoSaqueArma()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow)) return true;
            return false;
        }
    }
}