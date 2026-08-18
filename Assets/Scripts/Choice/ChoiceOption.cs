using System;
using UnityEngine;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 선택지 하나의 데이터를 담는다. ChoiceClip에서 이거가지고 선택지 ui 설정함.
	/// TODO : 추후 여기에 점프할 ChoiceClipEntry같은거 가지고 있게 할거임
	/// </summary>
	[Serializable]
	public class ChoiceOption
	{
		/// <summary>
		/// 선택지 버튼에 표시할 문구
		/// </summary>
		[SerializeField]
		private string text;

		/// <summary>
		/// 선택지 문구
		/// </summary>
		public string Text => text;

		/// <summary>
		/// 표시할 문구가 있는지 여부
		/// </summary>
		public bool HasText => !string.IsNullOrWhiteSpace(text);

		/// <summary>
		/// 인스펙터 빈 항목 초기화용
		/// </summary>
		public ChoiceOption()
		{
			text = string.Empty;
		}

		/// <summary>
		/// 선택지 문구로 항목 하나를 만든다
		/// </summary>
		public ChoiceOption(string text)
		{
			this.text = text;
		}
	}
}
