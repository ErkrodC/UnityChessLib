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
			Square rookStartSquare = new Square(rookStartingFile, 1);
			Rook rook = new Rook(Side.White);
			board[rookStartSquare] = rook;
			CastlingMove castlingMove = new CastlingMove(new Square(5, 1), new Square(7, 1), rookStartSquare);

			castlingMove.HandleAssociatedPiece(board);

			Assert.AreEqual(rook, board[castlingMove.GetRookEndSquare()]);
		}
	}
}