using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.Entidades.Peoes.EnumsPeoes
{
    public enum EstadoJogador 
    {
        Ocupado,

        Parado,
        Andando,        
        Interagindo,
        SacandoArma,
        GuardandoArma,

        ModoAtaque,
        Atacando,
        AndandoArmado,
    }
}