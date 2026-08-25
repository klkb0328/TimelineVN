using TimelineVN.Timeline;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 클립 하나의 대사를 들고 있다가 대사창에 적용한다.
	/// 지금 몇 글자까지 찍혔는지도 여기서 계산함
	/// </summary>
	public class DialogueClipBehaviour : PlayableBehaviour, ISingleClip<DialogueLine, DialogueUI>
	{
		/// <summary>
		/// 클립에서 전달받은 대사
		/// </summary>
		public DialogueLine Data { get; private set; }

		/// <summary>
		/// 클립의 대사를 설정한다
		/// </summary>
		public void SetData(DialogueLine data)
		{
			this.Data = data;
		}

		/// <summary>
		/// 들고 있는 대사를 대사창에 표시한다.
		/// 클립 안에서 얼마나 흘렀는지를 보고 몇 글자까지 보일지 정한다
		/// </summary>
		public void Apply(DialogueUI dialogueUI, ClipTime time)
		{
			if (Data == null)
			{
				Debug.LogError("Data is null");
				return;
			}

			var typing = new TypingData(Data.CharacterCount, Data.SecondsPerCharacter, time.Duration);

			dialogueUI.Show(Data, typing.GetVisibleCount(time.Elapsed));
		}
	}
}
