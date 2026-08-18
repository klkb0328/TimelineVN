using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Timeline
{
	/// <summary>
	/// 점프해서 도착할 수 있는 자리. 분기 시작, 복귀 지점, 장면 끝이 여기 속한다.
	/// 인터페이스로 안 하고 추상 클래스로 만든 건 Unity 직렬화 때문임. 그래서 상속으로 처리
	/// </summary>
	public abstract class JumpTarget : PlayableAsset, ITimelineClipAsset
	{
		/// <summary>
		/// 새 클립을 만들 때의 길이. 일반적으로 별다른 기능이 없는 경우가 많아 짧게 처리해둠.
		/// </summary>
		protected virtual double DefaultDuration => 0.5;

		/// <summary>
		/// 표식은 겹칠 일이 없어서 블렌딩을 막는다
		/// </summary>
		public ClipCaps clipCaps => ClipCaps.None;

		/// <summary>
		/// 새 클립이 생성될 때의 길이
		/// </summary>
		public override double duration => DefaultDuration;

		/// <summary>
		/// 재생 그래프에 아무것도 안 만든다. 자리만 표시하는 클립이라 재생 중에 할 일이 없으니깐..
		/// </summary>
		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			return Playable.Null;
		}
	}
}
