using System;

namespace UnityChess {
	/// <summary>Base class for any chess piece.</summary>
	public abstract class Piece {
		public readonly Side Owner;
		public readonly LegalMovesList LegalMoves;
		public Square Position;

		protected Piece(Square startPosition, Side owner) {
			Owner = owner;
			Position = startPosition;
			LegalMoves = new LegalMovesList();
		}

		protected Piece(Piece pieceCopy) {
			Owner = pieceCopy.Owner;
			Position = pieceCopy.Position;
			LegalMoves = pieceCopy.LegalMoves.DeepCopy();
		}

		public abstract void UpdateLegalMoves(Board board, GameConditions gameConditions);

		public Piece DeepCopy() {
			Type derivedType = GetType();
			return (Piece) derivedType.GetConstructor(new []{derivedType})?.Invoke(new object[]{this}); // copy constructor call
		}

		public override string ToString() => $"{Owner} {GetType().Name}";
	}
}