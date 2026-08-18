using TimelineVN.Timeline.Editor;
using UnityEditor;
using UnityEngine.Timeline;

namespace TimelineVN.Choice.Editor
{
	/// <summary>
	/// 선택지 클립 생성을 자동화해서 작업자 편의를 제공한다.
	/// 클립을 만들고, 자리를 잡고, 서로 잇는 것까지 여기서 대신한다
	/// 물론 수동으로도 가능함!
	/// </summary>
	public static class ChoiceClipHelper
	{
		/// <summary>
		/// 선택지 클립과 복귀 지점 사이를 띄울 간격
		/// </summary>
		private const double ReturnGap = 0.5;

		/// <summary>
		/// 마지막 클립 끝에서 새 분기까지 띄울 간격
		/// </summary>
		private const double BranchGap = 1.0;

		/// <summary>
		/// 분기 몸통 길이. 짧게 잡고 대사를 넣으면서 늘린다.
		/// 처음부터 넓게 벌려두면 화면에 한눈에 안 들어온다
		/// </summary>
		private const double BranchBodyDuration = 0.5;

		/// <summary>
		/// 선택지를 놓으면 복귀 지점이 알아서 따라 나오게 해준다.
		/// 선택지 클립이 만들어진 다음 프레임에 불린다
		/// </summary>
		/// <example>
		/// 선택지를 3.0 ~ 4.0 에 놓으면 ReturnGap이 0.5니까 4.5에 생성됨
		/// </example>
		public static void CreateDefaultReturn(TrackAsset track, ChoiceShowClip showClip)
		{
			if (track == null || showClip == null)
			{
				return;
			}

			TimelineClip showTimelineClip = TimelineClipFinder.FindClipOf(track.timelineAsset, showClip);
			if (showTimelineClip == null)
			{
				return;
			}

			TimelineClip returnClip = track.CreateClip<ChoiceReturnClip>();
			returnClip.start = showTimelineClip.end + ReturnGap;

			// 코드로 만든 클립에는 ClipEditor 가 안 불려서 이름을 직접 넣는다
			returnClip.displayName = ChoiceClipNaming.ReturnName;

			showClip.SetDefaultReturn(returnClip.asset as ChoiceReturnClip);
			EditorUtility.SetDirty(showClip);
		}

		/// <summary>
		/// 버튼 한 번으로 분기 한 벌을 만들어 준다. 선택지 항목과 분기 시작, 끝이
		/// 한꺼번에 생기고 셋이 이어진다.
		/// 분기는 메인 뒤쪽 빈 자리에 놓인다. 재생이 거기까지 흘러가지는 않고 점프로만 들어간다
		/// TODO : 일단 현재는 MainEndClip에서 안막히게 되어있는데, 작업 안되엇음.
		/// </summary>
		/// <example>
		/// 누르기 전 :  [C1][복귀]     ||
		/// 누른 뒤   :  [C1][복귀]     ||   [C1 : (빈 항목)]  V   [C1 -> 복귀]
		///              <메인 구간>    ||         <여기부터 분기.>  V 사이에 대사나 연출을 채워야함.
		/// </example>
		public static void AddBranch(TrackAsset track, ChoiceShowClip showClip)
		{
			if (track == null || showClip == null)
			{
				return;
			}

			TimelineAsset timeline = track.timelineAsset;

			var entryClip = track.CreateClip<ChoiceEntryClip>();
			entryClip.start = TimelineClipFinder.FindLastEnd(timeline) + BranchGap;

			var exitClip = track.CreateClip<ChoiceExitClip>();
			exitClip.start = entryClip.end + BranchBodyDuration;

			var entry = entryClip.asset as ChoiceEntryClip;
			var exit = exitClip.asset as ChoiceExitClip;

			entry.SetExit(exit);

			exit.SetReturnTarget(showClip.DefaultReturn);


			var option = new ChoiceOption(string.Empty);
			option.SetEntry(entry);
			showClip.Options.Add(option);

			// 이름에 소속 선택지가 들어가서 연결을 다 끝낸 뒤에 만든다
			entryClip.displayName = ChoiceClipNaming.BuildEntryName(timeline, entry);
			exitClip.displayName = ChoiceClipNaming.BuildExitName(timeline, exit);

			var showTimelineClip = TimelineClipFinder.FindClipOf(timeline, showClip);
			if (showTimelineClip != null)
			{
				showTimelineClip.displayName = ChoiceClipNaming.BuildShowName(showClip);
			}

			// 이거안하면 변경사항 적용안됨
			EditorUtility.SetDirty(entry);
			EditorUtility.SetDirty(exit);
			EditorUtility.SetDirty(showClip);
		}
	}
}
