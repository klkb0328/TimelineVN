using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Timeline.Editor
{
	/// <summary>
	/// 타임라인을 훑어서 클립을 찾아준다.
	/// 클립을 자동으로 놓을 자리를 잡거나, 참조만 든 클립이 몇 초에 있는지 알아낼 때 쓴다
	/// </summary>
	public static class TimelineClipFinder
	{
		/// <summary>
		/// 가장 뒤에 있는 클립의 끝 시각. 클립이 없으면 0.
		/// 모든 트랙을 뒤져서 그중 가장 뒤에있는 클립을 찾아줌.
		/// TODO : 음 나중에는 특정 클립들만 포함하도록 플래그 처리같은거 해볼까?
		/// </summary>
		/// <example>
		/// 대사 트랙 끝이 5.0, 선택지 트랙 끝이 3.5 이면  ->  5.0
		/// </example>
		public static double FindLastEnd(TimelineAsset timeline)
		{
			if (timeline == null)
			{
				return 0.0;
			}

			double lastEnd = 0.0;

			// 모든 트랙 다뒤져서 그트랙들의 클립들중 가장 뒤에있는거 찾아서 반환해버림
			foreach (TrackAsset track in timeline.GetOutputTracks())
			{
				foreach (TimelineClip clip in track.GetClips())
				{
					if (clip.end > lastEnd)
					{
						lastEnd = clip.end;
					}
				}
			}

			return lastEnd;
		}

		/// <summary>
		/// PlayableAsset 을 가지고 있는 클립을 찾는다. 예를 들면 DialogueClip 을 찾으며,
		/// 이때 TimelineClip 은 이 PlayableAsset 을 가지고 있는 유니티가 제공해주는 껍데기다.
		/// 즉 쉽게 말해서 ScriptPlayable 이라는 껍데기(그래프의 노드)에 PlayableBehaviour 를
		/// 연결한 것과 같은 구조라고 보면 된다
		/// </summary>
		/// <example>
		/// ChoiceExitClip 은 ChoiceReturnClip 을 참조로 들지만 그게 몇 초인지는 모른다.
		/// 시간 같은 건 스크립트인 ChoiceReturnClip 이 아니라 그걸 담고 있는 껍데기가
		/// 가지고 있어서, 그 껍데기를 찾는 것이다
		///   FindClipOf(timeline, 그 ChoiceReturnClip)  ->  담고 있는 TimelineClip
		///   거기서 start 를 읽어 목록에 "복귀 (12.5초)" 라고 띄운다
		/// </example>
		public static TimelineClip FindClipOf(TimelineAsset timeline, PlayableAsset asset)
		{
			if (timeline == null || asset == null)
			{
				return null;
			}

			foreach (TrackAsset track in timeline.GetOutputTracks())
			{
				foreach (TimelineClip clip in track.GetClips())
				{
					// 에셋 하나는 클립 하나에만 실려서 처음 걸리는 게 곧 답이다
					if (clip.asset == asset)
					{
						return clip;
					}
				}
			}

			return null;
		}
	}
}
