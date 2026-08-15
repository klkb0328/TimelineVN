using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 대사 클립을 올려 대사창을 제어하는 타임라인 트랙
	/// </summary>
	[DisplayName("Dialogue Track")]
	[TrackColor(0.45f, 0.6f, 0.9f)]
	[TrackClipType(typeof(DialogueClip))]
	[TrackBindingType(typeof(DialogueUI))]
	public class DialogueTrack : TrackAsset
	{
		/// <summary>
		/// 이 트랙의 클립들을 받아 대사창을 제어할 믹서를 만든다
		/// </summary>
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			// TODO : 아래 로그는 호출 시점 실측용이다. 테스트 후 제거할거임
			PlayableDirector director = go.GetComponent<PlayableDirector>();
			DialogueUI binding = director == null ? null : director.GetGenericBinding(this) as DialogueUI;
			Debug.Log($"[Track] f{Time.frameCount} CreateTrackMixer inputCount={inputCount} isPlaying={Application.isPlaying} binding={(binding == null ? "null" : binding.name)}");

			return ScriptPlayable<DialogueTrackMixer>.Create(graph, inputCount);
		}
	}
}
