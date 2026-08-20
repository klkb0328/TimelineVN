using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineVN.Choice.Editor
{
	/// <summary>
	/// 분기를 구분하는 색을 관리한다. 분기를 추가할 때 아직 안 쓴 색을 하나 떼어 주고,
	/// Timeline 창 클립 몸통에 얹을 반투명 색을 만들어 준다
	/// </summary>
	public static class ChoiceBranchPalette
	{
		/// <summary>
		/// 분기에 배정할 색들이고 현재 선택지 5개가 최대긴한데, 삭제하거나 이럴때 중복될까봐 여분으로 만들어둠
		/// </summary>
		private static readonly Color[] Colors =
		{
			new Color(0.95f, 0.33f, 0.33f),
			new Color(0.95f, 0.64f, 0.33f),
			new Color(0.95f, 0.85f, 0.33f),
			new Color(0.74f, 0.95f, 0.33f),
			new Color(0.33f, 0.95f, 0.54f),
			new Color(0.33f, 0.95f, 0.90f),
			new Color(0.33f, 0.69f, 0.95f),
			new Color(0.33f, 0.44f, 0.95f),
			new Color(0.64f, 0.33f, 0.95f),
			new Color(0.95f, 0.33f, 0.74f)
		};

		/// <summary>
		/// 클립 몸통에 얹을 때의 알파. 불투명하게 덮으면 기본 배경이 안 비쳐서 반투명 해야함
		/// </summary>
		private const float BodyAlpha = 0.25f;

		/// <summary>
		/// 이 타임라인에서 아직 안 쓴 색을 하나 고른다.
		/// 팔레트를 다 쓰면 처음부터 돌리므로 그때부터는 겹치는 색이 나온다
		/// </summary>
		public static Color PickUnusedColor(TimelineAsset timeline)
		{
			var usedColors = CollectUsedColors(timeline);

			foreach (Color candidate in Colors)
			{
				if (!IsUsed(usedColors, candidate))
				{
					return candidate;
				}
			}

			return Colors[usedColors.Count % Colors.Length];
		}

		/// <summary>
		/// 분기 색을 클립 몸통에 얹을 색으로 바꾼다
		/// </summary>
		public static Color ToBodyColor(Color branchColor)
		{
			return new Color(branchColor.r, branchColor.g, branchColor.b, BodyAlpha);
		}

		/// <summary>
		/// 타임라인에 놓인 분기들이 지금 쓰고 있는 색
		/// </summary>
		private static List<Color> CollectUsedColors(TimelineAsset timeline)
		{
			var usedColors = new List<Color>();

			if (timeline == null)
			{
				return usedColors;
			}

			foreach (TrackAsset track in timeline.GetOutputTracks())
			{
				foreach (TimelineClip clip in track.GetClips())
				{
					var entry = clip.asset as ChoiceEntryClip;
					if (entry != null && entry.HasBranchColor)
					{
						usedColors.Add(entry.BranchColor);
					}
				}
			}

			return usedColors;
		}
		
		private static bool IsUsed(List<Color> usedColors, Color candidate)
		{
			foreach (Color usedColor in usedColors)
			{
				if (usedColor == candidate)
				{
					return true;
				}
			}

			return false;
		}
	}
}
