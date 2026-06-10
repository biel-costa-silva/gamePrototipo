using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Peoes.UtilitariosPeoes
{
    public interface ICausamDano
    {
        int dano { get; }
        float direcao { get; }
    }
}