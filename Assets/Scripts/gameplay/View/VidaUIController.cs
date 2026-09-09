using Assets.Scripts.Model.Entidades.Peoes;
using UnityEngine;
using UnityEngine.UI;

public class VidaUIController : MonoBehaviour
{

    public int vida;
    
    public int vidaMax;

    public Image[] coracao;
    public Sprite cheio;
    public Sprite vazio;

    
    // Update is called once per frame
    void Update()
    {
        LogicaCoracao();
    }
    public void Decrementar(int dano)
    {
        vida = vida - dano;
    }

    void LogicaCoracao()
    {
        if(vida > vidaMax)
        {
            vida = vidaMax;
        }

        for (int i = 0; i < coracao.Length; i++)
        {
            if(i < vida)
            {
                coracao[i].sprite = cheio;
            }
            else
            {
                coracao[i].sprite = vazio;
            }
            if(i < vidaMax)
            {
                coracao[i].enabled = true;
            }
            else
            {
                coracao[i].enabled = true;
            }
        }
    }
}
