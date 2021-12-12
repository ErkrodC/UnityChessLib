namespace UnityChess {
	public abstract class SpecialMove : Movement {
		public Piece AssociatedPiece { get; }

		protected SpecialMove(Square piecePosition, Square end, Piece associatedPiece) : base(piecePosition, end) {
			AssociatedPiece = associatedPiece;
		}

		public abstract void HandleAssociatedPiece(Board board);
	}
}