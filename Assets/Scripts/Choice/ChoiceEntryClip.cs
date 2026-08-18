using System.ComponentModel;
using TimelineVN.Timeline;
using UnityEngine;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 분기가 시작하는 자리. 선택지 항목을 고르면 재생이 여기로 점프한다.
	/// ChoiceShowClip에서 자동으로 분기 만들면 Entry,Exit 둘다 만들어짐.
	/// </summary>
	[DisplayName("Manual/Choice Entry")]
	public class ChoiceEntryClip : JumpTarget
	{
		/// <summary>
		/// 이 분기가 끝나는 자리. 둘 사이가 분기 구간이다.
		/// 감추는 이유는 ChoiceOption.entry 와 같다
		/// </summary>
		[SerializeField, HideInInspector]
		private ChoiceExitClip exit;

		/// <summary>
		/// 이 분기가 끝나는 자리
		/// </summary>
		public ChoiceExitClip Exit => exit;

		/// <summary>
		/// 분기의 끝이 연결되어 있는지 여부
		/// </summary>
		public bool HasExit => exit != null;

		/// <summary>
		/// 분기의 끝을 지정한다. 분기 추가 버튼이 이어줄 때 부른다
		/// </summary>
		public void SetExit(ChoiceExitClip exit)
		{
			this.exit = exit;
		}
	}
}
