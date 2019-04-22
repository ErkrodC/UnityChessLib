using System.Collections;
using System.Collections.Generic;

namespace UnityChess {
	public class LegalMovesList : IEnumerable<Movement> {
		public int Count => list.Count;
		
		private readonly List<Movement> list;

		public LegalMovesList() => list = new List<Movement>();
		private LegalMovesList(List<Movement> list) => this.list = list;

		public void Add(Movement move) => list.Add(move);
		public void Clear() => list.Clear();
		public LegalMovesList DeepCopy() => new LegalMovesList(list.ConvertAll(move => new Movement(move)));

		internal bool TryGetLegalMoveUsingBaseMove(ref Movement move) {
			foreach (Movement validMove in list) {
				if (validMove.Start == move.Start && validMove.End == move.End) {
					move = validMove;
					return true;
				}
			}
			
			return false;
		}
		
		public IEnumerator<Movement> GetEnumerator() => list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
