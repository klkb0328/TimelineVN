using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.SpriteSwap
{
	/// <summary>
	/// 이 구간 동안 보여줄 스프라이트 하나를 담는 타임라인 클립
	/// </summary>
	public class SpriteClip : PlayableAsset, ITimelineClipAsset
	{
		/// <summary>
		/// 새 클립을 만들 때의 길이
		/// </summary>
		private const double DefaultDuration = 1;

		[SerializeField]
		private Sprite sprite;

		/// <summary>
		/// 이 클립 구간에 보여줄 스프라이트
		/// </summary>
		public Sprite Sprite => sprite;

		/// <summary>
		/// 스프라이트는 섞이지 않으므로 블렌딩을 막아 클립 겹침을 차단한다
		/// </summary>
		public ClipCaps clipCaps => ClipCaps.None;

		/// <summary>
		/// 새 클립이 생성될 때의 길이
		/// </summary>
		public override double duration => DefaultDuration;

		/// <summary>
		/// 이 클립의 스프라이트를 실어 나를 재생 노드 만듬
		/// </summary>
		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<SpriteClipBehaviour> playable = ScriptPlayable<SpriteClipBehaviour>.Create(graph);
			playable.GetBehaviour().SetData(sprite);

			return playable;
		}
	}
}
