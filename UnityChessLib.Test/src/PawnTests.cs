using NUnit.Framework;

namespace UnityChess.Test {
	[TestFixture]
	public class PawnTests {
		[Test]
		public void LegalMoveGeneration_BlockedPawn_PawnHasNoLegalMoves() {
			Game game = new FENSerializer().Deserialize(
				"rnbqk1nr/pppp1ppp/8/4p3/1b1P4/2N5/PPP1PPPP/R1BQKBNR w KQkq - 2 3"
			);
			game.BoardTimeline.TryGetCurrent(out Board board);

			game.TryGetLegalMovesForPiece(board[new Square("c2")], out var legalMovesForBlockedPawn);

			Assert.AreEqual(0, legalMovesForBlockedPawn?.Count ?? 0);
		}
	}
}