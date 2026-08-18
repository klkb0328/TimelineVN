using TimelineVN.Timeline;
using TimelineVN.Timeline.Editor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace TimelineVN.Dialogue.Editor
{
	/// <summary>
	/// 대사 트랙이 만들어질 때 할 일을 정한다. 뭐 에디터적으로 편의성 추가 위함임 별거 없다!
	/// </summary>
	[CustomTimelineEditor(typeof(DialogueTrack))]
	public class DialogueTrackEditor : TrackEditor
	{
		/// <summary>
		/// 대사 트랙을 만들면 MainEndClip 생성해줌. 어느 장면에나 하나씩은 필요하고 현재는 대사클립이 주요 진행요소니까..
		/// </summary>
		public override void OnCreate(TrackAsset track, TrackAsset copiedFrom)
		{
			// 복제로 생긴 트랙은 원본 클립을 그대로 들고 온다. 또 놓으면 두 개가 된다
			if (copiedFrom != null)
			{
				return;
			}

			var clip = track.CreateClip<MainEndClip>();

			clip.start = TimelineClipFinder.FindLastEnd(track.timelineAsset);

			// 코드로 만든 클립에는 ClipEditor 가 안 불려서 이름을 직접 넣는다
			clip.displayName = MainEndClipEditor.ClipName;
		}
	}
}
