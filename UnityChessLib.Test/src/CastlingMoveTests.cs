using NUnit.Framework;

namespace UnityChess.Test {
	[TestFixture]
	public class CastlingMoveTests {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
		}

		[Test]
		[TestCase(6, 8)] //Kingside castle
		[TestCase(4, 1)] //Queenside castle
		public void HandleAssociatedPiece_CastlingMove_RookMovedAsExpected(int expected, int rookStartingFile) {
			Square startingSquare = new Square(rookStartingFile, 1);
			Rook rook = new Rook(startingSquare, Side.White);
			board[startingSquare] = rook;
			CastlingMove mcm = new CastlingMove(new Square(5, 1), new Square(7, 1), rook);

			mcm.HandleAssociatedPiece(board);

			Assert.True(rook.HasMoved);
			Assert.AreEqual(expected, rook.Position.File);
		}
	}
}