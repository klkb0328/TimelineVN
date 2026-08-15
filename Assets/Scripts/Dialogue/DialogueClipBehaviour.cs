using TimelineVN.Timeline;
using UnityEngine.Playables;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 클립 하나의 대사를 들고 있다가 대사창에 적용한다
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
		/// 들고 있는 대사를 대사창에 표시한다
		/// </summary>
		public void Apply(DialogueUI dialogueUI)
		{
			dialogueUI.Show(Data);
		}
	}
}
