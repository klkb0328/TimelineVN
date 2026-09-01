using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.SpriteSwap
{
	/// <summary>
	/// 스프라이트 클립을 올려 SpriteRenderer 의 그림을 갈아끼우는 타임라인 트랙.
	/// 표정 전환이 첫 용도지만 트랙 자체는 표정을 모른다. 스프라이트면 뭐든 받는다
	/// </summary>
	[DisplayName("Sprite Track")]
	[TrackClipType(typeof(SpriteClip))]
	[TrackBindingType(typeof(SpriteRenderer))]
	public class SpriteTrack : TrackAsset
	{
		/// <summary>
		/// 이 트랙의 클립들을 받아 스프라이트를 갈아끼울 믹서를 만든다
		/// </summary>
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<SpriteTrackMixer>.Create(graph, inputCount);
		}
	}
}
