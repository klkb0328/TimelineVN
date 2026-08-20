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
		/// Timeline 창에서 이 분기를 칠할 색. 짝인 분기 끝도 같은 색으로 칠해진다.
		/// 알파 0 은 색을 아직 안 정했다는 뜻이다. 색은 선택지 항목 줄에서 고르므로 여기서는 감춘다
		/// </summary>
		[SerializeField, HideInInspector]
		private Color branchColor;

		/// <summary>
		/// 이 분기가 끝나는 자리
		/// </summary>
		public ChoiceExitClip Exit => exit;

		/// <summary>
		/// Timeline 창에서 이 분기를 칠할 색
		/// </summary>
		public Color BranchColor => branchColor;

		/// <summary>
		/// 분기의 끝이 연결되어 있는지 여부
		/// </summary>
		public bool HasExit => exit != null;

		/// <summary>
		/// 칠할 색이 정해져 있는지 여부
		/// </summary>
		public bool HasBranchColor => branchColor.a > 0f;

		/// <summary>
		/// 분기의 끝을 지정한다. 분기 추가 버튼이 이어줄 때 부른다
		/// </summary>
		public void SetExit(ChoiceExitClip exit)
		{
			this.exit = exit;
		}

		/// <summary>
		/// 칠할 색을 정한다. 분기 추가 버튼이 팔레트에서 선택가능!
		/// </summary>
		public void SetBranchColor(Color branchColor)
		{
			this.branchColor = branchColor;
		}
	}
}
