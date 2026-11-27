using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Geral
{
    /// <summary>
    /// Conta quantas vezes cada cena foi carregada desde que o jogo abriu.
    /// Classe estática: não precisa de GameObject, funciona mesmo apertando Play direto na cena Jogo.
    /// 
    /// ATENÇÃO: o valor já está atualizado quando o Start() dos objetos roda
    /// (o evento sceneLoaded acontece depois do Awake/OnEnable e antes do Start).
    /// Por isso, leia a contagem no Start(), não no Awake().
    /// </summary>
    public static class ContadorDeCenas
    {
        private static readonly Dictionary<string, int> vezes = new Dictionary<string, int>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Iniciar()
        {
            // Zera a cada Play (necessário se "Enter Play Mode Options" estiver sem Domain Reload)
            vezes.Clear();
            SceneManager.sceneLoaded -= AoCarregarCena;
            SceneManager.sceneLoaded += AoCarregarCena;
        }

        private static void AoCarregarCena(Scene cena, LoadSceneMode modo)
        {
            vezes.TryGetValue(cena.name, out int n);
            vezes[cena.name] = n + 1;
        }

        public static int VezesAberta(string nomeDaCena)
        {
            return vezes.TryGetValue(nomeDaCena, out int n) ? n : 0;
        }

        public static int VezesAbertaCenaAtual() => VezesAberta(SceneManager.GetActiveScene().name);

        public static bool EhPrimeiraVez(string nomeDaCena) => VezesAberta(nomeDaCena) == 1;

        public static bool EhPrimeiraVezCenaAtual() => VezesAbertaCenaAtual() == 1;
    }
}
