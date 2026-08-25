namespace TimelineVN.Timeline
{
	/// <summary>
	/// 클립 안에서 지금 어디쯤 왔는지 알려주는 값.
	/// 트랙 믹서가 재생 그래프에서 읽어다 클립한테 넘겨준다
	/// </summary>
	public readonly struct ClipTime
	{
		/// <summary>
		/// 클립 시작하고 얼마나 지났나?
		/// </summary>
		public double Elapsed { get; }

		/// <summary>
		/// 이 클립이 몇 초짜리인가
		/// </summary>
		public double Duration { get; }

		/// <summary>
		/// 경과 시간과 클립 길이로 만든다
		/// </summary>
		public ClipTime(double elapsed, double duration)
		{
			this.Elapsed = elapsed;
			this.Duration = duration;
		}
	}

	/// <summary>
	/// 한 시점에 하나만 활성인 클립이 구현하는 인터페이스
	/// 자기 데이터를 들고 있다가 트랙 믹서가 부르면 대상에 적용한다
	/// TODO : 추후 필요시 BlendingClip 추가 예정
	/// </summary>
	public interface ISingleClip<TData, TBinding>
	{
		/// <summary>
		/// 클립에서 전달받은 데이터
		/// </summary>
		TData Data { get; }

		/// <summary>
		/// 클립의 데이터를 설정한다
		/// </summary>
		void SetData(TData data);

		/// <summary>
		/// 들고 있는 데이터를 대상에 적용한다.
		/// 클립 구간 어디쯤인지가 필요한 클립은 time 을 쓰고, 아닌 클립은 그냥 무시하면 된다
		/// </summary>
		void Apply(TBinding binding, ClipTime time);
	}
}
