using Assets.Scripts.Model.Entidades.Objetos.UtilitariosObjetos;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Objetos
{
    public class Item : MonoBehaviour, IInteracoes
    {
        public bool PodeSofrerInteracao()
        {
            throw new NotImplementedException();
        }

        public void SofrerInteracao(Jogador jogador)
        {

        }
    }
}
