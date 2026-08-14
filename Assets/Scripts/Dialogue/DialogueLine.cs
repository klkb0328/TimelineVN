using System;
using UnityEngine;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 대사 한 줄의 데이터를 담는다
	/// </summary>
	[Serializable]
	public class DialogueLine
	{
		/// <summary>
		/// 화자 이름. 비워두면 나레이션으로 표시된다
		/// </summary>
		[SerializeField]
		private string speakerName;

		/// <summary>
		/// 화면에 표시할 대사 텍스트
		/// </summary>
		[SerializeField, TextArea(2, 4)]
		private string text;

		/// <summary>
		/// 화자 이름
		/// </summary>
		public string SpeakerName => speakerName;

		/// <summary>
		/// 대사 텍스트
		/// </summary>
		public string Text => text;

		/// <summary>
		/// 화자 이름이 지정되어 있는지 여부
		/// </summary>
		public bool HasSpeaker => !string.IsNullOrWhiteSpace(speakerName);

		/// <summary>
		/// 인스펙터 빈 대사 초기화용
		/// </summary>
		public DialogueLine()
		{
			speakerName = string.Empty;
			text = string.Empty;
		}

		/// <summary>
		/// 화자 이름과 대사 텍스트로 대사 한 줄을 만든다
		/// </summary>
		public DialogueLine(string speakerName, string text)
		{
			this.speakerName = speakerName;
			this.text = text;
		}
	}
}
