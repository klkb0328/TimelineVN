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
	[TrackClipType(typeof(DialogueClip))]
	[TrackBindingType(typeof(DialogueUI))]
	public class DialogueTrack : TrackAsset
	{
		/// <summary>
		/// 이 트랙의 클립들을 받아 대사창을 제어할 믹서를 만든다
		/// </summary>
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<DialogueTrackMixer>.Create(graph, inputCount);
		}
	}
}
