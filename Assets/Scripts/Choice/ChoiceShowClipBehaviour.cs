using System.Collections.Generic;
using TimelineVN.Timeline;
using UnityEngine.Playables;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 클립 하나의 선택지들을 들고 있다가 선택지 UI 에 띄운다
	/// </summary>
	public class ChoiceShowClipBehaviour : PlayableBehaviour, ISingleClip<List<ChoiceOption>, ChoiceUI>
	{
		/// <summary>
		/// 클립에서 전달받은 선택지들
		/// </summary>
		public List<ChoiceOption> Data { get; private set; }

		/// <summary>
		/// 클립의 선택지들을 설정한다
		/// </summary>
		public void SetData(List<ChoiceOption> data)
		{
			this.Data = data;
		}

		/// <summary>
		/// 들고 있는 선택지들을 화면에 띄운다.
		/// 선택지는 구간 어디쯤이든 똑같이 떠 있으면 되니까 time 은 안 쓴다
		/// </summary>
		public void Apply(ChoiceUI choiceUI, ClipTime time)
		{
			choiceUI.Show(Data);
		}
	}
}
