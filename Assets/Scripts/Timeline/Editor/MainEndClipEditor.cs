using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace TimelineVN.Timeline.Editor
{
	/// <summary>
	/// 장면 끝 클립을 Timeline 창에 어떻게 보여줄지 정한다
	/// </summary>
	[CustomTimelineEditor(typeof(MainEndClip))]
	public class MainEndClipEditor : ClipEditor
	{
		/// <summary>
		/// 들고 있는 데이터가 없어서 이름이 고정이다.
		/// 코드로 클립을 만들 때는 이 클래스가 안 불려서 만드는 쪽에서 직접 넣어야 함
		/// 영어로 하려다가 일단.. 한글로
		/// </summary>
		public const string ClipName = "연출끝";

		/// <summary>
		/// 새로 만들어질 때 이름을 붙인다
		/// </summary>
		public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
		{
			clip.displayName = ClipName;
		}

		/// <summary>
		/// 일단 혹시라도 실수로 이름 바꾸는거 방지용.
		/// TODO : 일단 써보다가 불편하면 뭐.. 없애야지
		/// </summary>
		public override void OnClipChanged(TimelineClip clip)
		{
			if (clip.displayName != ClipName)
			{
				clip.displayName = ClipName;
			}
		}
	}
}
