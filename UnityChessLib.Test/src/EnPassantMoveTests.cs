using NUnit.Framework;

namespace UnityChess.Test {
	[TestFixture]
	public class EnPassantMoveTests {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
			board.ClearBoard();
		}

		[Test]
		public void HandleAssociatedPiece_EnPassantMove_AssocPawnIsRemoved() {
			Square capturedPawnSquare = new Square(1, 2);
			board[capturedPawnSquare] = new Pawn(Side.White);
			EnPassantMove enPassantMove = new EnPassantMove(Square.Invalid, Square.Invalid, capturedPawnSquare);

			enPassantMove.HandleAssociatedPiece(board);

			Assert.AreEqual(null, board[capturedPawnSquare]);
		}
	}
}