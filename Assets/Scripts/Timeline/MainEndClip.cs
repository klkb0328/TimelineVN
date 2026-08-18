using System.ComponentModel;

namespace TimelineVN.Timeline
{
	/// <summary>
	/// 장면이 여기서 끝난다는 표식. 클립 몸통이 마지막 대사 뒤 여백 역할을 한다.
	/// JumpTarget으로 이녀석 연결해두면 바로 끝내기 가능
	/// TODO : 지금은 대사 트랙에 얹어 뒀다. 연출 층이 따로 생기면 이 클립을 옮길 예정..
	/// </summary>
	[DisplayName("Main End")]
	public class MainEndClip : JumpTarget
	{
		/// <summary>
		/// 다른 표식보다 길다. 몸통 자체가 여백이라서
		/// </summary>
		protected override double DefaultDuration => 2.0;
	}
}
