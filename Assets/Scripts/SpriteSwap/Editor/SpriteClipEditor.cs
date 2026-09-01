using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace TimelineVN.SpriteSwap.Editor
{
	/// <summary>
	/// 스프라이트 클립을 Timeline 창에 어떻게 보여줄지 정한다
	/// </summary>
	[CustomTimelineEditor(typeof(SpriteClip))]
	public class SpriteClipEditor : ClipEditor
	{
		/// <summary>
		/// 스프라이트를 안 넣은 클립에 띄울 경고
		/// </summary>
		private const string NoSpriteWarning = "스프라이트를 안 골랐습니다.";

		/// <summary>
		/// 클립이 바뀌었을 때 그 스프라이트 이름을 클립 이름에 반영한다.
		/// 클립만 보고 어느 표정인지 알 수 있어야 타임라인을 훑어서 편집할 수 있다
		/// </summary>
		public override void OnClipChanged(TimelineClip clip)
		{
			var spriteClip = clip.asset as SpriteClip;
			if (spriteClip == null)
			{
				return;
			}

			string clipName = spriteClip.Sprite != null ? spriteClip.Sprite.name : string.Empty;
			if (clip.displayName != clipName)
			{
				clip.displayName = clipName;
			}
		}

		/// <summary>
		/// 클립을 그릴 때 쓸 표시 옵션을 돌려준다
		/// </summary>
		public override ClipDrawOptions GetClipOptions(TimelineClip clip)
		{
			// 기본 옵션에 클립 오류 표시가 담겨 있어 새로 만들지 않고 받아서 얹는다
			ClipDrawOptions options = base.GetClipOptions(clip);

			var spriteClip = clip.asset as SpriteClip;
			if (spriteClip == null)
			{
				return options;
			}

			if (spriteClip.Sprite != null)
			{
				// 클립이 짧으면 이름이 잘려서 툴팁으로 한 번 더 보여준다
				options.tooltip = spriteClip.Sprite.name;

				return options;
			}

			// 대입하면 Timeline 이 채워 넣은 오류 표시가 같이 지워진다
			if (string.IsNullOrEmpty(options.errorText))
			{
				options.errorText = NoSpriteWarning;
			}

			return options;
		}
	}
}
