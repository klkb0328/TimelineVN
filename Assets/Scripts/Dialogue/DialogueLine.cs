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
		/// 타이핑 속도 기본값. 초당 20자다
		/// </summary>
		private const float DefaultSecondsPerCharacter = 0.05f;

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
		/// 한 글자를 찍는 데 걸리는 시간. 클립이 짧으면 이 속도를 못 지키고 빨라진다
		/// </summary>
		[SerializeField]
		private float secondsPerCharacter = DefaultSecondsPerCharacter;

		/// <summary>
		/// 화자 이름
		/// </summary>
		public string SpeakerName => speakerName;

		/// <summary>
		/// 대사 텍스트
		/// </summary>
		public string Text => text;

		/// <summary>
		/// 한 글자를 찍는 데 걸리는 시간.
		/// 0 이면 기본값을 쓴다. 이 필드가 생기기 전에 만든 클립이 0 으로 열려서임
		/// TODO : 대사를 시트에서 가져오게 되면 시트에 없는 값이라 그때 따로 챙겨야 함
		/// </summary>
		public float SecondsPerCharacter => secondsPerCharacter > 0f ? secondsPerCharacter : DefaultSecondsPerCharacter;

		/// <summary>
		/// 화자 이름이 지정되어 있는지 여부
		/// </summary>
		public bool HasSpeaker => !string.IsNullOrWhiteSpace(speakerName);

		/// <summary>
		/// 표시할 대사 내용이 있는지 여부
		/// </summary>
		public bool HasText => !string.IsNullOrWhiteSpace(text);

		/// <summary>
		/// 대사가 몇 글자인지. 타이핑에 걸리는 시간이 이걸로 정해진다
		/// </summary>
		public int CharacterCount => text != null ? text.Length : 0;

		/// <summary>
		/// 인스펙터 빈 대사 초기화용
		/// </summary>
		public DialogueLine()
		{
			speakerName = string.Empty;
			text = string.Empty;
			secondsPerCharacter = DefaultSecondsPerCharacter;
		}

		/// <summary>
		/// 화자 이름과 대사 텍스트로 대사 한 줄을 만든다
		/// </summary>
		public DialogueLine(string speakerName, string text)
		{
			this.speakerName = speakerName;
			this.text = text;
			this.secondsPerCharacter = DefaultSecondsPerCharacter;
		}
	}
}
