using System.Collections.Generic;

namespace UnityChess {
	/// <summary>Base class for any chess piece.</summary>
	public abstract class Piece {
		public Side Owner { get; protected set; }
		public Square Position { get; protected internal set; }
		
		protected Piece(Square startPosition, Side owner) {
			Owner = owner;
			Position = startPosition;
		}

		public abstract Piece DeepCopy();

		public abstract Dictionary<(Square, Square), Movement> CalculateLegalMoves(
			Board board,
			GameConditions gameConditions,
			Square position
		);
		
		public override string ToString() => $"{Owner} {GetType().Name}";
	}

	public abstract class Piece<T> : Piece where T : Piece<T>, new() {
		protected Piece(Square startPosition, Side owner) : base(startPosition, owner) { }
		
		public override Piece DeepCopy() {
			return new T {
				Owner = Owner,
				Position = Position
			};
		}
	}
}