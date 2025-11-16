using System.Threading.Tasks;

namespace UnityChess.Core {
	public interface IUCIEngine {
		void Start();

		void ShutDown();

		Task SetupNewGame(Game game);

		Task<Movement> GetBestMove(int timeoutMS);
	}
}