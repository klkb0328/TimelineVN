using System.ComponentModel;
using TimelineVN.Timeline;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 분기가 끝나고 돌아와 착지하는 자리. 명시적으로 여기에 도착 시킬때 쓴다.
	/// 선택지를 만들면 하나가 딸려 나온다. 다른 데로 보내고 싶으면 원하는 자리에
	/// 하나 더 놓고 분기 끝에서 그걸 고르면 됨. 표식 역할만 해서 다수 존재해도 문제 없음
	/// </summary>
	[DisplayName("Manual/Choice Return")]
	public class ChoiceReturnClip : JumpTarget
	{
	}
}
