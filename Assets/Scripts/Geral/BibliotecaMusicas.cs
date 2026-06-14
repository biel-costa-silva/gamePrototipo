using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct TrilhaSonora
{
    public string nomeTrilha;
    public AudioClip clip;
}
public class BibliotecaMusicas : MonoBehaviour
{
    public TrilhaSonora[] trilhas;
    public AudioClip GetClipNome(string nomeTrilha)
    {
        foreach(var trilha in trilhas)
        {
            if(trilha.nomeTrilha == nomeTrilha)
            {
                return trilha.clip;
            }
        }
        return null;
    }
}
