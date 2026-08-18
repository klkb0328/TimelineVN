using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Timeline
{
	/// <summary>
	/// 점프해서 도착할 수 있는 도착지다. 분기 시작, 복귀 지점, 장면 끝이 여기 속한다.
	/// 인터페이스가 아니라 추상 클래스인 건 Unity 직렬화 때문임. 그래서 상속으로 처리
	/// TODO : 아직 이거 구체적인 점프되어서 이쪽으로 이동하는건 안되어있다.
	/// </summary>
	public abstract class JumpTarget : PlayableAsset, ITimelineClipAsset
	{
		/// <summary>
		/// 새 클립을 만들 때의 길이. 몸통이 죽은 시간이라 클릭할 만큼만 짧게 잡는다
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
		/// 재생 그래프에 아무것도 안 만든다. 자리만 표시하는 클립이라서..
		/// </summary>
		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			return Playable.Null;
		}
	}
}
