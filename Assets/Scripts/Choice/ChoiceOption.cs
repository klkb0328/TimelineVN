using System;
using UnityEngine;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 선택지 하나. 화면에 띄울 문구와, 고르면 데려갈 분기를 들고 있다.
	/// 플레이어가 고르면 이 객체가 그대로 선택 결과로 넘어간다
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
		/// 고르면 점프할 분기의 시작 지점.
		/// 참고로 EntryClip의 경우 수동생성은 delete로 제거했을때를 위함이고 보통은 ChoiceShowClip으로 자동생성 권장함!
		/// </summary>
		[SerializeField, HideInInspector]
		private ChoiceEntryClip entry;

		/// <summary>
		/// 선택지 문구
		/// </summary>
		public string Text => text;

		/// <summary>
		/// 데려갈 분기의 시작 지점
		/// </summary>
		public ChoiceEntryClip Entry => entry;

		/// <summary>
		/// 표시할 문구가 있는지 여부
		/// </summary>
		public bool HasText => !string.IsNullOrWhiteSpace(text);

		/// <summary>
		/// 데려갈 분기가 연결되어 있는지 여부
		/// </summary>
		public bool HasEntry => entry != null;

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

		/// <summary>
		/// 데려갈 분기를 지정한다.
		/// </summary>
		public void SetEntry(ChoiceEntryClip entry)
		{
			this.entry = entry;
		}
	}
}
