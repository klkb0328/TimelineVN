using System.Collections.Generic;
using TimelineVN.Dialogue;
using UnityEngine;

namespace TimelineVN.Test
{
	/// <summary>
	/// Timeline 연동 전에 대사창 표시를 눈으로 확인하는 임시 컴포넌트
	/// TODO : 테스트 후 반드시 제거해야함
	/// </summary>
	public class DialogueUITester : MonoBehaviour
	{
		/// <summary>
		/// 대사를 표시할 대상 대사창
		/// </summary>
		[SerializeField]
		private DialogueUI dialogueUI;

		/// <summary>
		/// 확인에 사용할 샘플 대사 목록
		/// </summary>
		[SerializeField]
		private List<DialogueLine> lines = new();

		/// <summary>
		/// 다음에 표시할 대사의 위치
		/// </summary>
		private int nextIndex;

		/// <summary>
		/// 다음 샘플 대사를 표시한다
		/// </summary>
		[ContextMenu("다음 대사")]
		public void ShowNext()
		{
			if (lines.Count == 0)
			{
				Debug.LogWarning("표시할 샘플 대사가 없습니다", this);
				return;
			}

			dialogueUI.Show(lines[nextIndex]);
			nextIndex = (nextIndex + 1) % lines.Count;
		}

		/// <summary>
		/// 다음에 표시할 대사를 첫 줄로 되돌린다
		/// </summary>
		[ContextMenu("처음으로")]
		public void ResetIndex()
		{
			nextIndex = 0;
		}
	}
}
