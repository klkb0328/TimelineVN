using TMPro;
using UnityEngine;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 대사창에 화자 이름과 대사 텍스트를 표시한다
	/// </summary>
	public class DialogueUI : MonoBehaviour
	{
		/// <summary>
		/// 대사창 전체의 표시를 제어하는 캔버스 그룹
		/// </summary>
		[SerializeField]
		private CanvasGroup canvasGroup;

		/// <summary>
		/// 화자 이름을 표시할 텍스트
		/// </summary>
		[SerializeField]
		private TMP_Text speakerLabel;

		/// <summary>
		/// 대사 본문을 표시할 텍스트
		/// </summary>
		[SerializeField]
		private TMP_Text dialogueLabel;

		/// <summary>
		/// 대사 한 줄을 대사창에 표시한다.
		/// visibleCharacters 는 앞에서부터 몇 글자까지 보일지다. 타이핑 효과가 이걸로 굴러간다
		/// TODO : 일단 당분간 작업할때는 직접 받아서 처리하는 형식이고 추후 백로그로 시트를 쓰던 하는 방식으로 가져올듯?
		/// </summary>
		public void Show(DialogueLine line, int visibleCharacters)
		{
			if (line == null)
			{
				Debug.LogError("Line is null");
				return;
			}

			// 텍스트는 통째로 넣고 보이는 글자 수만 줄인다.
			// 잘라서 넣으면 매 프레임 새 문자열이 생기고 줄바꿈도 다시 계산된다
			dialogueLabel.text = line.Text;
			dialogueLabel.maxVisibleCharacters = visibleCharacters;

			// 나레이션은 화자 이름을 빈 문자열로 두어 아무것도 그리지 않는다
			speakerLabel.text = line.HasSpeaker ? line.SpeakerName : string.Empty;
		}

		/// <summary>
		/// 대사창 전체를 보이거나 감춘다
		/// </summary>
		public void SetVisible(bool visible)
		{
			canvasGroup.alpha = visible ? 1f : 0f;
		}
	}
}
