using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using System;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
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
