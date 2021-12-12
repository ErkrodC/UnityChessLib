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
			Square capturedPawnStartSquare = new Square(1, 2);
			Pawn capturedPawn = new Pawn(capturedPawnStartSquare, Side.White);
			board[capturedPawnStartSquare] = capturedPawn;
			
			
			EnPassantMove mepm = new EnPassantMove(Square.Invalid, Square.Invalid, capturedPawn);

			mepm.HandleAssociatedPiece(board);

			Assert.AreNotEqual(board[capturedPawn.Position], capturedPawn);
		}
	}
}