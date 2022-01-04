﻿using Moq;
using NUnit.Framework;

namespace UnityChess.Test {
	[TestFixture]
	public class BoardTests {
		private static Board board;
		private static Piece pawn;
		private static Square pawnStartSquare;

		[SetUp]
		public void Init() {
			pawnStartSquare = new Square(1, 2);
			pawn = new Pawn(Side.White);
			board = new Board((pawnStartSquare, pawn));
		}

		[Test]
		[TestCase(1, 3)]
		[TestCase(3, 5)]
		[TestCase(4, 8)]
		[TestCase(8, 2)]
		[TestCase(3, 4)]
		public void MovePiece_NormalMove_PieceIsMoved(int expectedFile, int expectedRank) {
			Square expectedPosition = new Square(expectedFile, expectedRank);
			Movement move = new Movement(pawnStartSquare, expectedPosition);
			
			board.MovePiece(move);

			Assert.AreEqual(pawn, board[expectedPosition]);
			Assert.AreEqual(null, board[pawnStartSquare]);
		}

		[Test]
		public void MovePiece_SpecialMove_HandleAssocPieceCalled() {
			Mock<SpecialMove> mockSpecialMove = new(
				new Square(1, 2),
				new Square(1, 3)
			);

			board.MovePiece(mockSpecialMove.Object);

			mockSpecialMove.Verify(specialMove => specialMove.HandleAssociatedPiece(board), Times.Exactly(1));
		}

		[Test]
		[TestCase(Side.White)]
		[TestCase(Side.Black)]
		public void GetKingSquare_KingMoved_KingSquareIsUpdated(Side player) {
			King king = new King(player);
			Square kingStartSquare = new Square(5, player.CastlingRank());
			Square kingEndSquare0 = kingStartSquare + new Square(0, player.ForwardDirection());
			Square kingEndSquare1 = kingEndSquare0 + new Square(0, player.ForwardDirection());
			board = new Board((kingStartSquare, king));
			Movement move0 = new Movement(kingStartSquare, kingEndSquare0);
			Movement move1 = new Movement(kingEndSquare0, kingEndSquare1);
			
			board.MovePiece(move0);
			board.GetKingSquare(player);
			board.MovePiece(move1);

			Square actual = board.GetKingSquare(player);
			Assert.AreEqual(kingEndSquare1, actual, $"e: {kingEndSquare1}, a: {actual}");
		}
	}
}