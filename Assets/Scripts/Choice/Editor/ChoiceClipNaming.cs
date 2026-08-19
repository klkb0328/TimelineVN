using System.Collections.Generic;
using System.Text;
using TimelineVN.Timeline;
using TimelineVN.Timeline.Editor;
using UnityEngine.Timeline;

namespace TimelineVN.Choice.Editor
{
	/// <summary>
	/// 선택지 관련 클립들이 타임라인에 뜰 이름을 만든다.
	/// 참조가 선택지 -> 분기 한 방향이라, 분기 쪽 이름을 만들 때는 여기서 거꾸로 훑는다
	/// Timeline 창의 클립에 뜰 문자열을 조립 해서 에디터 작업할때 조금더 편하게 할수있도록 하기위한 코드임!
	/// </summary>
	public static class ChoiceClipNaming
	{
		/// <summary>
		/// 복귀 지점 클립에 표시할 이름. 들고 있는 데이터가 없어서 고정이다
		/// </summary>
		public const string ReturnName = "복귀";

		/// <summary>
		/// 문구를 아직 안 적은 항목을 대신 표시할 문구
		/// </summary>
		private const string EmptyOptionLabel = "(빈 항목)";

		/// <summary>
		/// 연결이 끊겼을 때 표시할 문구
		/// </summary>
		private const string MissingLabel = "(연결 없음)";

		/// <summary>
		/// 선택지 클립 이름. 번호와 항목 문구들을 이어 붙인다.
		/// 물론 이거 잘리는건 어느정도 감수해야함.
		/// </summary>
		/// <example>
		/// 1번 선택지에 "따라간다", "모른 척한다"  ->  C1 : 따라간다 / 모른 척한다
		/// 방금 만들어 항목이 없음                 ->  C2
		/// 항목은 있는데 문구를 안 적음            ->  C3 : (빈 항목)
		/// </example>
		public static string BuildShowName(ChoiceShowClip showClip)
		{
			string label = BuildChoiceLabel(showClip);

			if (!showClip.HasOptions)
			{
				return label;
			}

			var builder = new StringBuilder(label);
			builder.Append(" : ");

			for (int i = 0; i < showClip.Options.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(" / ");
				}

				builder.Append(DescribeOption(showClip.Options[i]));
			}

			return builder.ToString();
		}

		/// <summary>
		/// 선택지 클립에 마우스를 올렸을 때 띄울 항목 전문.
		/// 클립이 좁으면 이름이 잘려서 툴팁에는 줄을 나눠 전부 보여준다
		/// </summary>
		public static string BuildShowTooltip(ChoiceShowClip showClip)
		{
			var builder = new StringBuilder(BuildChoiceLabel(showClip));

			foreach (ChoiceOption option in showClip.Options)
			{
				builder.Append('\n');
				builder.Append(DescribeOption(option));
			}

			return builder.ToString();
		}

		/// <summary>
		/// 분기 시작 이름. 몇 번 선택지의 어느 항목인지 보여준다.
		/// 분기는 메인에서 멀리 떨어져 있어서 이게 없으면 어느 게 어느 건지 모른다
		/// </summary>
		/// <example>
		/// 1번 선택지의 "따라간다" 가 데려가는 분기  ->  C1 : 따라간다
		/// 아무도 안 가리키는 분기                   ->  (연결 없음)
		/// </example>
		public static string BuildEntryName(TimelineAsset timeline, ChoiceEntryClip entry)
		{
			if (!TryFindOwner(timeline, entry, out ChoiceShowClip owner, out ChoiceOption option))
			{
				return MissingLabel;
			}

			return $"{BuildChoiceLabel(owner)} : {DescribeOption(option)}";
		}

		/// <summary>
		/// 분기 끝 이름. 어느 선택지의 분기가 어디로 가는지 보여준다
		/// </summary>
		/// <example>
		/// 1번 선택지의 분기가 복귀 지점으로  ->  C1 -> 복귀
		/// 돌아오지 않고 장면을 끝냄          ->  C1 -> 장면 끝
		/// 복귀 지점을 안 골랐음              ->  C1 -> (연결 없음)
		/// </example>
		public static string BuildExitName(TimelineAsset timeline, ChoiceExitClip exit)
		{
			string destination = DescribeTargetKind(exit.Destination);

			if (!TryFindEntry(timeline, exit, out ChoiceEntryClip entry))
			{
				return $"{MissingLabel} -> {destination}";
			}

			if (!TryFindOwner(timeline, entry, out ChoiceShowClip owner, out ChoiceOption _))
			{
				return $"{MissingLabel} -> {destination}";
			}

			return $"{BuildChoiceLabel(owner)} -> {destination}";
		}

		/// <summary>
		/// 복귀 지점 목록에 뜰 문구. 이름만으로는 구분이 안 되니 시각을 붙인다
		/// </summary>
		/// <example>
		/// 복귀 지점이 셋이면 목록에 이렇게 뜬다
		///   복귀  (12.5초)
		///   복귀  (40.0초)
		///   장면 끝  (30.0초)
		/// </example>
		public static string DescribeJumpTarget(TimelineAsset timeline, JumpTarget target)
		{
			if (target == null)
			{
				return MissingLabel;
			}

			string kind = DescribeTargetKind(target);
			TimelineClip clip = TimelineClipFinder.FindClipOf(timeline, target);

			return clip == null ? kind : $"{kind}  ({clip.start:0.0}초)";
		}

		/// <summary>
		/// 이 분기 시작을 데려갈 곳으로 삼고 있는 선택지와 항목을 찾는다
		/// </summary>
		private static bool TryFindOwner(TimelineAsset timeline, ChoiceEntryClip entry, out ChoiceShowClip owner, out ChoiceOption option)
		{
			owner = null;
			option = null;

			if (entry == null)
			{
				return false;
			}

			foreach (ChoiceShowClip showClip in GetShowClips(timeline))
			{
				foreach (ChoiceOption candidate in showClip.Options)
				{
					if (candidate.Entry != entry)
					{
						continue;
					}

					owner = showClip;
					option = candidate;

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 이 분기 끝을 짝으로 삼고 있는 분기 시작을 찾는다
		/// </summary>
		private static bool TryFindEntry(TimelineAsset timeline, ChoiceExitClip exit, out ChoiceEntryClip entry)
		{
			entry = null;

			if (timeline == null || exit == null)
			{
				return false;
			}

			foreach (TrackAsset track in timeline.GetOutputTracks())
			{
				foreach (TimelineClip clip in track.GetClips())
				{
					var candidate = clip.asset as ChoiceEntryClip;
					if (candidate == null || candidate.Exit != exit)
					{
						continue;
					}

					entry = candidate;

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 타임라인에 놓여 있는 선택지 클립들
		/// </summary>
		private static IEnumerable<ChoiceShowClip> GetShowClips(TimelineAsset timeline)
		{
			if (timeline == null)
			{
				yield break;
			}

			foreach (TrackAsset track in timeline.GetOutputTracks())
			{
				foreach (TimelineClip clip in track.GetClips())
				{
					var showClip = clip.asset as ChoiceShowClip;
					if (showClip != null)
					{
						yield return showClip;
					}
				}
			}
		}

		/// <summary>
		/// 선택지 하나를 가리키는 짧은 이름
		/// </summary>
		private static string BuildChoiceLabel(ChoiceShowClip showClip)
		{
			return $"C{showClip.ChoiceId}";
		}

		/// <summary>
		/// 항목 문구. 아직 안 적었으면 빈자리라는 걸 알린다
		/// </summary>
		private static string DescribeOption(ChoiceOption option)
		{
			return option.HasText ? option.Text : EmptyOptionLabel;
		}

		/// <summary>
		/// 점프해서 갈 자리가 어떤 종류인지
		/// </summary>
		private static string DescribeTargetKind(JumpTarget target)
		{
			if (target == null)
			{
				return MissingLabel;
			}

			return target is MainEndClip ? "장면 끝" : ReturnName;
		}
	}
}
