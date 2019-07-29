namespace UnityChess.Networking {
	public struct UnityChessDataPacketHeader {
		public readonly long UserDataSize;

		public UnityChessDataPacketHeader(long userDataSize) {
			UserDataSize = userDataSize;
		}
	}
}