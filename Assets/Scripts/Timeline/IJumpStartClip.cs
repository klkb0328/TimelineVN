namespace TimelineVN.Timeline
{
	/// <summary>
	/// 클립이 자기 자리를 점프 출발지로 만드는지 알린다.
	/// 재생이 이 클립에 닿으면 목적지로 옮겨가고, 여기 뒤로는 흐르지 않는다.
	/// 도착지인 JumpTarget 의 반대편이다
	/// </summary>
	public interface IJumpStartClip
	{
		/// <summary>
		/// 이 클립에 닿았을 때 옮겨갈 자리. 비어 있을 수 있다
		/// </summary>
		JumpTarget Destination { get; }

		/// <summary>
		/// 옮겨갈 자리가 연결되어 있는지 여부
		/// </summary>
		bool HasDestination { get; }
	}
}
