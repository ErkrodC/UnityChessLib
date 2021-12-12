using NUnit.Framework;

namespace UnityChess.Test {
	[TestFixture]
	public class PromotionMoveTests {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
			board.ClearBoard();
		}

		[Test]
		[TestCase(ElectedPiece.Knight)]
		[TestCase(ElectedPiece.Bishop)]
		[TestCase(ElectedPiece.Rook)]
		[TestCase(ElectedPiece.Queen)]
		public void HandleAssociatedPiece_PromotionMove_ElectedPieceGenerated(ElectedPiece election) {
			Square expectedPosition = new Square(1, 8);
			MockPromotionMove mpm = new MockPromotionMove(expectedPosition, election);

			mpm.HandleAssociatedPiece(board);

			Assert.AreEqual($"UnityChess.{election.ToString()}", board[expectedPosition].GetType().ToString());
		}
	}

	public class MockPromotionMove : PromotionMove {
		public MockPromotionMove(Square end, ElectedPiece election) : base(new Square(1, 7), end) { }
	}
}