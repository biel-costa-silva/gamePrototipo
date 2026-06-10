using Assets.Scripts.Model.Entidades.Peoes.UtilitariosPeoes;
using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEditor.SceneManagement;
using UnityEngine;

public abstract class Personagem
{
    //atributos da classe
    protected string nome;
    protected int vida;
    protected int dano;
    protected int defesa;
    protected float velocidade;
    protected float velocidadeBase;

    //flags
    public bool sofreuAtaque { get; protected set; }
    public bool sofreuChoque { get; set; }
    public bool deCostas { get; set; }

    //getters
    public string GetNome() => nome;    
    public int GetVida() => vida;
    public int GetDano() => dano;    
    public int GetDefesa() => defesa;
    public float GetVelocidade() => velocidade;
    public float GetVelocidadeBase() => velocidadeBase;
    
    //setters
    public void SetVelocidade(float v) => velocidade = v;
    public void SetVelocidadeBase(float v) => velocidadeBase = v;
    public void SetSofreuAtaque() => sofreuAtaque = true;

    public virtual void ReceberDano(int danoRecebido)
    {
        vida -= danoRecebido;
        sofreuAtaque = true;
    }
    public void LimparFlagAtaque()
    {
        sofreuAtaque = false;
    }
    public void LimparFlagChoque()
    {
        sofreuChoque = false;
    }

    public virtual bool EstaMorto() => vida <= 0;

}