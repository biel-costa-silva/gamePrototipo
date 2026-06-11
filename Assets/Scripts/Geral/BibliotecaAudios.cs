using UnityEngine;

[System.Serializable]
public struct EfeitoSonoro
{
    public string grupoID;
    public AudioClip[] clips;
}

public class BibliotecaAudios : MonoBehaviour
{
    public EfeitoSonoro[] efeitosSonoros;

    public AudioClip GetClipNome(string nome)
    {
        foreach(var efeitoSonoro in efeitosSonoros)
        {
            if(efeitoSonoro.grupoID == nome)
            {
                return efeitoSonoro.clips[Random.Range(0, efeitoSonoro.clips.Length)];
            }
        }
        return null;
    }
}
