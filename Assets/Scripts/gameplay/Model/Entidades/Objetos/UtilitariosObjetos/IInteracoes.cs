
using Assets.Scripts.Gameplay.Model.Entidades.Personagens;

namespace Assets.Scripts.Gameplay.Model.Entidades.Objetos.UtilitariosObjetos
{
	public interface IInteracoes
	{
		string nome { get; set; }
		public void SofrerInteracao(Jogador jogador);
		public bool PodeSofrerInteracao();
	}
}