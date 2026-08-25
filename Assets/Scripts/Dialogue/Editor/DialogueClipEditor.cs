using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineVN.Dialogue.Editor
{
	/// <summary>
	/// 대사 클립을 Timeline 창에 어떻게 보여줄지 정한다
	/// 런타임 쪽 코드 분리 제대로 해야함..
	/// </summary>
	[CustomTimelineEditor(typeof(DialogueClip))]
	public class DialogueClipEditor : ClipEditor
	{
		/// <summary>
		/// 타이핑이 빨라진 클립에 칠할 주황색
		/// </summary>
		private static readonly Color AcceleratedColor = new Color(1f, 0.55f, 0.15f);

		/// <summary>
		/// 클립이 바뀌었을 때 그 대사를 클립 이름에 반영한다
		/// 이렇게 해야 하눈에 볼수있음.
		/// </summary>
		public override void OnClipChanged(TimelineClip clip)
		{
			var dialogueClip = clip.asset as DialogueClip;
			if (dialogueClip == null)
			{
				return;
			}

			// 참고로 화자는 굳이 안넣는것으로 결정함. 아무래도 넣어봤자 내용이 중요하니까
			string clipName = BuildClipName(dialogueClip.Line);
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

			var dialogueClip = clip.asset as DialogueClip;
			if (dialogueClip == null)
			{
				return options;
			}

			TypingData typing = BuildTypingData(clip, dialogueClip);
			options.tooltip = BuildTooltip(dialogueClip.Line, typing);

			// errorText 를 안 쓰고 색으로 알린다. 그걸 채우면 Timeline 이 넣는 오류 표시랑 툴팁이 같이 지워짐
			if (typing.IsAccelerated)
			{
				options.highlightColor = AcceleratedColor;
			}

			return options;
		}

		/// <summary>
		/// 이 클립의 대사가 어떻게 찍힐지 계산한다.
		/// 인스펙터도 같은 값을 보여줘야 해서 밖에서 부를 수 있게 열어둠
		/// </summary>
		public static TypingData BuildTypingData(TimelineClip clip, DialogueClip dialogueClip)
		{
			return new TypingData(dialogueClip.Line.CharacterCount, dialogueClip.Line.SecondsPerCharacter, clip.duration);
		}

		/// <summary>
		/// 클립 이름에 넣을 한 줄짜리 대사를 만든다
		/// </summary>
		private static string BuildClipName(DialogueLine line)
		{
			if (!line.HasText)
			{
				return string.Empty;
			}

			// 자꾸 개행이 나와서 이거 한줄로 표시되게 처리함
			return line.Text.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
		}

		/// <summary>
		/// 클립에 마우스를 올렸을 때 띄울 화자와 대사 전문을 만든다.
		/// 타이핑이 빨라지는 클립이면 왜 그런지도 뒤에 붙인다
		/// </summary>
		private static string BuildTooltip(DialogueLine line, TypingData typing)
		{
			if (!line.HasText)
			{
				return string.Empty;
			}

			string body = line.HasSpeaker ? $"{line.SpeakerName}\n{line.Text}" : line.Text;

			if (!typing.IsAccelerated)
			{
				return body;
			}

			return $"{body}\n\n클립이 짧아서 타이핑이 빨라짐\n{line.CharacterCount}자를 {typing.Duration:0.##}초에 찍는다";
		}
	}
}
