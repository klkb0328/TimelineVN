using TimelineVN.Timeline.Editor;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace TimelineVN.Choice.Editor
{
	/// <summary>
	/// 선택지 클립을 Timeline 창에 어떻게 보여줄지, 만들어질 때 뭘 딸려 만들지 정한다
	/// </summary>
	[CustomTimelineEditor(typeof(ChoiceShowClip))]
	public class ChoiceShowClipEditor : ClipEditor
	{
		/// <summary>
		/// 선택지가 만들어질 때 번호를 발급하고 복귀 지점을 딸려 만든다
		/// </summary>
		public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
		{
			var showClip = clip.asset as ChoiceShowClip;
			var choiceTrack = track as ChoiceTrack;

			if (showClip == null || choiceTrack == null)
			{
				return;
			}

			showClip.SetChoiceId(choiceTrack.TakeNextChoiceId());

			// 바로 하면 제대로된 위치에 ReturnClip생성이 안되서 dealycall 에 연결함.
			EditorApplication.delayCall += () =>
			{
				ChoiceClipHelper.CreateDefaultReturn(choiceTrack, showClip);

				// 이거 강제로 갱신 해줘야 잘나옴..
				TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
			};
		}

		/// <summary>
		/// 선택지 내용이 바뀌면 클립 이름에 반영한다
		/// </summary>
		public override void OnClipChanged(TimelineClip clip)
		{
			var showClip = clip.asset as ChoiceShowClip;
			if (showClip == null)
			{
				return;
			}

			string clipName = ChoiceClipNaming.BuildShowName(showClip);
			if (clip.displayName != clipName)
			{
				clip.displayName = clipName;
			}

			TrackAsset track = clip.GetParentTrack();
			RefreshBranchNames(track == null ? null : track.timelineAsset, showClip);
		}

		/// <summary>
		/// 클립을 그릴 때 쓸 표시 옵션을 돌려준다
		/// </summary>
		public override ClipDrawOptions GetClipOptions(TimelineClip clip)
		{
			// 기본 옵션에 클립 오류 표시가 담겨 있어 새로 만들지 않고 받아서 얹는다
			var options = base.GetClipOptions(clip);

			var showClip = clip.asset as ChoiceShowClip;
			if (showClip == null)
			{
				return options;
			}

			options.tooltip = ChoiceClipNaming.BuildShowTooltip(showClip);

			return options;
		}

		/// <summary>
		/// 이 선택지에 딸린 분기 클립들의 이름도 다시 만든다.
		/// 분기 이름에 문구가 들어가는데 분기 쪽은 자기가 바뀐 게 아니라 훅이 안 불림
		/// </summary>
		private static void RefreshBranchNames(TimelineAsset timeline, ChoiceShowClip showClip)
		{
			if (timeline == null)
			{
				return;
			}

			foreach (ChoiceOption option in showClip.Options)
			{
				// 아직 분기를 안 만든 항목. 이름을 고칠 클립도 없다
				if (!option.HasEntry)
				{
					continue;
				}

				// 분기 시작 이름에 이 항목 문구가 그대로 들어간다
				var entryClip = TimelineClipFinder.FindClipOf(timeline, option.Entry);
				
				if (entryClip != null)
				{
					entryClip.displayName = ChoiceClipNaming.BuildEntryName(timeline, option.Entry);
				}

				if (!option.Entry.HasExit)
				{
					continue;
				}

				// 분기 끝에는 몇 번 선택지인지가 들어가서  번호가 바뀌면 여기도 같이 바꿔야 함
				var exitClip = TimelineClipFinder.FindClipOf(timeline, option.Entry.Exit);
				if (exitClip != null)
				{
					exitClip.displayName = ChoiceClipNaming.BuildExitName(timeline, option.Entry.Exit);
				}
			}
		}
	}
}
