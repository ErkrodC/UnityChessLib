namespace UnityChess {
	/// <summary>Base class for any chess piece.</summary>
	public abstract class Piece {
		public Side Owner { get; protected set; }
		public LegalMovesList LegalMoves { get; protected set; }
		public Square Position { get; protected internal set; }
		
		protected Piece(Square startPosition, Side owner) {
			Owner = owner;
			Position = startPosition;
			LegalMoves = new LegalMovesList();
		}

		public abstract Piece DeepCopy();

		public abstract void UpdateLegalMoves(Board board, GameConditions gameConditions);
		
		public override string ToString() => $"{Owner} {GetType().Name}";
	}

	public abstract class Piece<T> : Piece where T : Piece<T>, new() {
		protected Piece(Square startPosition, Side owner) : base(startPosition, owner) { }
		
		public override Piece DeepCopy() {
			return new T {
				Owner = Owner,
				Position = Position,
				LegalMoves = LegalMoves.DeepCopy()
			};
		}
	}
}