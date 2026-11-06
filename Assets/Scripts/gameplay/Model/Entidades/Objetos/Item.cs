using Assets.Scripts.Gameplay.Model.Entidades.Personagens;
using Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos;
using System;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos
{
    public class Item : MonoBehaviour, IInteracoes
    {
        // Precisa de um Collider2D com "Is Trigger" marcado NO MESMO GameObject (igual ao Totem).
        public event Action<Item> OnColetado;
        private bool coletado = false;

        string IInteracoes.nome { get; set; } = "item";

        // Não acessa nada da Unity de propósito: o jogador pode manter uma referência
        // a este item mesmo depois dele ser destruído.
        public bool PodeSofrerInteracao()
        {
            return !coletado;
        }

        public void SofrerInteracao(Jogador jogador)
        {
            if (coletado) return;
            coletado = true;

            // aqui entra o efeito do item no futuro (cura, buff, upgrade...)

            OnColetado?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
