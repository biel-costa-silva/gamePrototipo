using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Model.Fisica.FisicaPersonagens
{
    public interface ICausamDano
    {
        int dano { get; }
        float direcao { get; }
    }
}