using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 대사 하나를 담는 타임라인 클립
	/// </summary>
	public class DialogueClip : PlayableAsset, ITimelineClipAsset
	{
		/// <summary>
		/// 이 클립 구간에 표시할 대사
		/// </summary>
		[SerializeField]
		private DialogueLine line = new DialogueLine();

		/// <summary>
		/// 대사는 섞이지 않으므로 블렌딩을 막아 클립 겹침을 차단한다
		/// </summary>
		public ClipCaps clipCaps => ClipCaps.None;

		/// <summary>
		/// 이 클립의 대사를 실어 나를 재생 노드를 만든다
		/// </summary>
		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<DialogueClipBehaviour> playable = ScriptPlayable<DialogueClipBehaviour>.Create(graph);
			playable.GetBehaviour().SetData(line);

			return playable;
		}
	}
}
