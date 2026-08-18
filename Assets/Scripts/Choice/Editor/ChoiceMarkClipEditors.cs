using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace TimelineVN.Choice.Editor
{
	/// <summary>
	/// 분기 시작 클립을 Timeline 창에 어떻게 보여줄지 정한다
	/// </summary>
	[CustomTimelineEditor(typeof(ChoiceEntryClip))]
	public class ChoiceEntryClipEditor : ClipEditor
	{
		/// <summary>
		/// 어느 선택지의 어느 항목인지를 이름에 채운다
		/// </summary>
		public override void OnClipChanged(TimelineClip clip)
		{
			var entry = clip.asset as ChoiceEntryClip;
			var track = clip.GetParentTrack();

			if (entry == null || track == null)
			{
				return;
			}

			string clipName = ChoiceClipNaming.BuildEntryName(track.timelineAsset, entry);
			if (clip.displayName != clipName)
			{
				clip.displayName = clipName;
			}
		}
	}

	/// <summary>
	/// 분기 끝 클립을 Timeline 창에 어떻게 보여줄지 정한다
	/// </summary>
	[CustomTimelineEditor(typeof(ChoiceExitClip))]
	public class ChoiceExitClipEditor : ClipEditor
	{
		/// <summary>
		/// 어느 선택지의 분기가 어디로 가는지를 이름에 채운다
		/// </summary>
		public override void OnClipChanged(TimelineClip clip)
		{
			var exit = clip.asset as ChoiceExitClip;
			var track = clip.GetParentTrack();

			if (exit == null || track == null)
			{
				return;
			}

			string clipName = ChoiceClipNaming.BuildExitName(track.timelineAsset, exit);
			if (clip.displayName != clipName)
			{
				clip.displayName = clipName;
			}
		}
	}

	/// <summary>
	/// 복귀 지점 클립을 Timeline 창에 어떻게 보여줄지 정한다
	/// </summary>
	[CustomTimelineEditor(typeof(ChoiceReturnClip))]
	public class ChoiceReturnClipEditor : ClipEditor
	{
		/// <summary>
		/// 새로 만들어질 때 이름을 붙인다
		/// </summary>
		public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
		{
			clip.displayName = ChoiceClipNaming.ReturnName;
		}

		/// <summary>
		/// 편집자가 이름을 고쳐도 되돌린다 일단 Return을 강조하기 위해서고
		/// 필요하면 제거할수도 있음!
		/// </summary>
		public override void OnClipChanged(TimelineClip clip)
		{
			if (clip.displayName != ChoiceClipNaming.ReturnName)
			{
				clip.displayName = ChoiceClipNaming.ReturnName;
			}
		}
	}
}
