namespace TimelineVN.Timeline
{
	/// <summary>
	/// 클립이 들고 있는 데이터를 그래프로 세팅하기 위한 인터페이스
	/// </summary>
	public interface IClipData<T>
	{
		/// <summary>
		/// 클립에서 전달받은 데이터
		/// </summary>
		T Data { get; }

		/// <summary>
		/// 클립의 데이터를 설정한다
		/// </summary>
		void SetData(T data);
	}
}
