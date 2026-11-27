using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Geral
{
    public static class ContadorDeCenas
    {
        private static readonly Dictionary<string, int> contagem = new Dictionary<string, int>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Inicializar()
        {
            contagem.Clear();
            SceneManager.sceneLoaded -= AoCarregarCena;
            SceneManager.sceneLoaded += AoCarregarCena;
        }

        private static void AoCarregarCena(Scene cena, LoadSceneMode modo)
        {
            if (contagem.ContainsKey(cena.name))
                contagem[cena.name]++;
            else
                contagem[cena.name] = 1;

            Debug.Log($"[ContadorDeCenas] Cena '{cena.name}' carregada ({contagem[cena.name]}x).");
        }

        public static int VezesAberta(string nome)
        {
            return contagem.TryGetValue(nome, out int vezes) ? vezes : 0;
        }

        public static bool EhPrimeiraVez(string nome)
        {
            return VezesAberta(nome) == 1;
        }

        public static int VezesAbertaCenaAtual()
        {
            return VezesAberta(SceneManager.GetActiveScene().name);
        }

        public static bool EhPrimeiraVezCenaAtual()
        {
            return EhPrimeiraVez(SceneManager.GetActiveScene().name);
        }
    }
}