using TimelineVN.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 대사 하나를 담는 타임라인 클립
	/// </summary>
	public class DialogueClip : PlayableAsset, ITimelineClipAsset, IStopPointClip
	{
		/// <summary>
		/// 새 클립을 만들 때의 길이.
		/// 표정 전환이나 짧은 카메라 이동이 들어갈 만큼으로 잡았다
		/// </summary>
		private const double DefaultDuration = 1.0;

		/// <summary>
		/// 이 클립 구간에 표시할 대사
		/// </summary>
		[SerializeField]
		private DialogueLine line = new DialogueLine();

		/// <summary>
		/// 이 대사 끝에서 멈추고 입력을 기다릴지 여부.
		/// 끄면 다음 대사로 저절로 넘어간다
		/// </summary>
		[SerializeField]
		private bool waitForInput = true;

		/// <summary>
		/// 대사는 섞이지 않으므로 블렌딩을 막아 클립 겹침을 차단한다
		/// </summary>
		public ClipCaps clipCaps => ClipCaps.None;

		/// <summary>
		/// 새 클립이 생성될 때의 길이
		/// </summary>
		public override double duration => DefaultDuration;

		/// <summary>
		/// 이 클립 끝에서 재생을 멈추고 입력을 기다릴지 여부.
		/// 읽을 내용이 없으면 멈출 이유도 없으므로 빈 대사는 대기 설정과 무관하게 지나간다
		/// </summary>
		public bool CreatesStopPoint => waitForInput && line.HasText;

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
