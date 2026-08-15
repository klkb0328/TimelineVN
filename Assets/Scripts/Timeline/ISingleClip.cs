namespace TimelineVN.Timeline
{
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
		/// 들고 있는 데이터를 대상에 적용한다
		/// </summary>
		void Apply(TBinding binding);
	}
}
