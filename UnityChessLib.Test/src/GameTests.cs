using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Reflection;

namespace UnityChess.Test {
	[TestFixture]
	public class GameTests : Game {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
			board.ClearBoard();
		}

		[Test, TestCase(0), TestCase(1), TestCase(10), TestCase(40), TestCase(64)]
		public void UpdateAllPiecesValidMoves_PiecesOnBoard_UpdateValidMovesCalled(int numberOfPieces) {
			Mock<MockPiece> mockPiece = new Mock<MockPiece>();
			PopulateBoard(numberOfPieces, mockPiece);

			MethodInfo methodInfo = typeof(Game).GetMethod(
				"UpdateAllPiecesLegalMoves",
				BindingFlags.NonPublic | BindingFlags.Static
			);
			object[] parameters = { board, Square.Invalid, Side.White};
			methodInfo.Invoke(null, parameters);
			
			Game.UpdateAllPiecesLegalMoves(board, Square.Invalid, Side.White);

			mockPiece.Verify(piece => piece.UpdateLegalMoves(board, Square.Invalid), Times.Exactly(numberOfPieces));
		}

		private void PopulateBoard(int numberOfPieces, Mock<MockPiece> mockPiece) {
			for (int i = 0; i < numberOfPieces; i++) {
				int file = i / 8 + 1;
				int rank = i % 8 + 1; 
				board[file, rank] = mockPiece.Object;
			}
		}

		public GameTests(Mode mode, GameConditions startingConditions) : base(mode, startingConditions) {
		}
	}

	public abstract class MockPiece : Piece {
		protected MockPiece() : base(new Square(0, 0), Side.White) { }

		public override void UpdateLegalMoves(Board board, Square enPassantEligibleSquare) {
			throw new System.NotImplementedException();
		}
	}
}