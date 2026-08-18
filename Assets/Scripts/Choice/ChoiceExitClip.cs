using System.ComponentModel;
using TimelineVN.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 분기가 끝나는 자리. 여기 닿으면 복귀 지점으로 점프한다.
	/// 장면 끝을 가리키게 바꾸면 돌아오지 않고 거기서 장면이 끝난다.
	/// JumpTarget 을 안 물려받은 건 일부러다. 여긴 도착지가 아니라서..
	/// </summary>
	[DisplayName("Manual/Choice Exit")]
	public class ChoiceExitClip : PlayableAsset, ITimelineClipAsset
	{
		/// <summary>
		/// 새 클립을 만들 때의 길이. 다른 표식들과 맞춘다
		/// </summary>
		private const double DefaultDuration = 0.5;

		/// <summary>
		/// 분기가 끝난 뒤 돌아갈 자리.
		/// 인스펙터에는 이 필드 대신 드롭다운을 그린다. 감추는 이유는 ChoiceOption.entry 와 같다
		/// </summary>
		[SerializeField, HideInInspector]
		private JumpTarget returnTarget;

		/// <summary>
		/// 분기가 끝난 뒤 돌아갈 자리
		/// </summary>
		public JumpTarget ReturnTarget => returnTarget;

		/// <summary>
		/// 돌아갈 자리가 연결되어 있는지 여부
		/// </summary>
		public bool HasReturnTarget => returnTarget != null;

		/// <summary>
		/// 표식은 겹칠 일이 없어서 블렌딩을 막는다
		/// </summary>
		public ClipCaps clipCaps => ClipCaps.None;

		/// <summary>
		/// 새 클립이 생성될 때의 길이
		/// </summary>
		public override double duration => DefaultDuration;

		/// <summary>
		/// 돌아갈 자리를 지정한다. 분기 추가 버튼과 복귀 지점 목록이 부른다
		/// </summary>
		public void SetReturnTarget(JumpTarget returnTarget)
		{
			this.returnTarget = returnTarget;
		}

		/// <summary>
		/// 재생 그래프에 아무것도 안 만든다. 자리만 표시하는 클립이라서..
		/// </summary>
		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			return Playable.Null;
		}
	}
}
