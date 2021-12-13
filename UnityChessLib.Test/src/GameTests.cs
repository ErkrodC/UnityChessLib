using NUnit.Framework;
using Moq;

namespace UnityChess.Test {
	[TestFixture]
	public class GameTests {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
			board.ClearBoard();
		}

		[Test]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(10)]
		[TestCase(40)]
		[TestCase(64)]
		public void UpdateAllPiecesValidMoves_PiecesOnBoard_UpdateValidMovesCalled(int numberOfPieces) {
			Mock<Piece> mockPiece = new Mock<Piece>(Square.Invalid, Side.White);
			PopulateBoard(numberOfPieces, mockPiece);

			Game.UpdateAllPiecesLegalMoves(board, GameConditions.NormalStartingConditions);

			mockPiece.Verify(
				piece => piece.UpdateLegalMoves(board, GameConditions.NormalStartingConditions),
				Times.Exactly(numberOfPieces)
			);
		}

		private void PopulateBoard(int numberOfPieces, Mock<Piece> mockPiece) {
			for (int i = 0; i < numberOfPieces; i++) {
				int file = i / 8 + 1;
				int rank = i % 8 + 1; 
				board[file, rank] = mockPiece.Object;
			}
		}
	}
}