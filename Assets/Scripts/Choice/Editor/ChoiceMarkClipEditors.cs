using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
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
		/// 짝인 분기 끝이 없을 때 클립에 띄울 경고 문구.
		/// 분기 끝 클립을 지우면 이렇게 된다
		/// </summary>
		private const string NoExitWarning = "분기 끝이 없어서 이 분기는 안 돌아옵니다. 분기 끝 클립을 다시 만들어 주세요";

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

		/// <summary>
		/// 클립을 그릴 때 쓸 표시 옵션을 돌려준다.
		/// 분기 색을 얹고, 분기 끝이 연결 안 돼 있으면 경고도 얹는다
		/// </summary>
		public override ClipDrawOptions GetClipOptions(TimelineClip clip)
		{
			// 기본 옵션에 클립 오류 표시가 담겨 있어 새로 만들지 않고 받아서 얹는다
			ClipDrawOptions options = base.GetClipOptions(clip);

			var entry = clip.asset as ChoiceEntryClip;
			if (entry == null)
			{
				return options;
			}

			// 클립이 좁아지면 몸통이 안 그려져서, 그때 남는 아래쪽 선을 같은 색으로 맞춰둔다.
			// 색을 안 정한 클립까지 대입하면 투명이 들어가 원래 나오던 트랙 색 선이 사라진다
			if (entry.HasBranchColor)
			{
				options.highlightColor = entry.BranchColor;
			}

			// 기본 옵션이 이미 오류를 물고 있으면 그게 더 급한 문제라 안 덮어씀
			if (!entry.HasExit && string.IsNullOrEmpty(options.errorText))
			{
				options.errorText = NoExitWarning;
			}

			return options;
		}

		/// <summary>
		/// 클립 몸통을 분기 색으로 칠한다. 짝인 분기 끝과 같은 색이 되어 보기편하게 하기위함임
		/// TODO : 이거 나중에 Entry랑 Exit클립 사이에 있는 경우 다른트랙이더라도 시간으로 체크해서 색통일하는것도 고려.. 일단 성능상 문제있을까봐 제외했지만 사용자입장에선 편할듯
		/// </summary>
		public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
		{
			var entry = clip.asset as ChoiceEntryClip;
			if (entry == null || !entry.HasBranchColor)
			{
				return;
			}

			EditorGUI.DrawRect(region.position, ChoiceBranchPalette.ToBodyColor(entry.BranchColor));
		}
	}

	/// <summary>
	/// 분기 끝 클립을 Timeline 창에 어떻게 보여줄지 정한다
	/// </summary>
	[CustomTimelineEditor(typeof(ChoiceExitClip))]
	public class ChoiceExitClipEditor : ClipEditor
	{
		/// <summary>
		/// 갈 자리를 안 골랐을 때 클립에 띄울 경고 문구.
		/// 여기 닿아도 점프를 안 하고 그냥 흘러가 버린다
		/// </summary>
		private const string NoDestinationWarning = "돌아갈 자리를 안 골랐습니다. 이 클립을 눌러 Inspector 에서 복귀 지점을 골라 주세요";

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

		/// <summary>
		/// 클립을 그릴 때 쓸 표시 옵션을 돌려준다.
		/// 짝인 분기 시작의 색을 얹고, 갈 자리가 안 골라져 있으면 경고도 보여줌
		/// </summary>
		public override ClipDrawOptions GetClipOptions(TimelineClip clip)
		{
			// 기본 옵션에 클립 오류 표시가 담겨 있어 새로 만들지 않고 받아서 얹는다
			ClipDrawOptions options = base.GetClipOptions(clip);

			var exit = clip.asset as ChoiceExitClip;
			if (exit == null)
			{
				return options;
			}

			// 클립이 좁아지면 몸통이 안 그려져서, 그때 남는 아래쪽 선을 같은 색으로 맞춰둔다
			if (TryGetBranchColor(exit, out Color branchColor))
			{
				options.highlightColor = branchColor;
			}

			// 기본 옵션이 이미 오류를 물고 있으면 그게 더 급한 문제라 안 덮어씀
			if (!exit.HasDestination && string.IsNullOrEmpty(options.errorText))
			{
				options.errorText = NoDestinationWarning;
			}

			return options;
		}

		/// <summary>
		/// 클립 몸통을 분기 색으로 칠한다. 짝인 분기 시작과 같은 색임
		/// </summary>
		public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
		{
			var exit = clip.asset as ChoiceExitClip;
			if (exit == null || !TryGetBranchColor(exit, out Color branchColor))
			{
				return;
			}

			EditorGUI.DrawRect(region.position, ChoiceBranchPalette.ToBodyColor(branchColor));
		}

		/// <summary>
		/// 짝인 분기 시작이 든 색을 가져온다.
		/// 색은 분기 시작 하나가 소유하고 여기서는 읽기만 해서, 둘이 어긋날 수가 없다
		/// </summary>
		private static bool TryGetBranchColor(ChoiceExitClip exit, out Color branchColor)
		{
			branchColor = Color.clear;

			if (!exit.HasEntry || !exit.Entry.HasBranchColor)
			{
				return false;
			}

			branchColor = exit.Entry.BranchColor;

			return true;
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
