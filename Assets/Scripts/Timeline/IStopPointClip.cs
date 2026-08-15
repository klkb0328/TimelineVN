namespace TimelineVN.Timeline
{
	/// <summary>
	/// 클립이 자기 끝을 재생 정지 지점으로 만드는지 알린다
	/// </summary>
	public interface IStopPointClip
	{
		/// <summary>
		/// 이 클립 끝에서 재생을 멈추고 입력을 기다릴지 여부
		/// </summary>
		bool CreatesStopPoint { get; }
	}
}
