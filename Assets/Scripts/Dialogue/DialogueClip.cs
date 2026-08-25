using System;
using TimelineVN.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 대사 하나를 이 클립에서 어떻게 찍을지 계산해둔 것.
	/// 재생이랑 에디터 표시가 같은 식을 써야 해서 여기 몰아뒀음
	/// </summary>
	public readonly struct TypingData
	{
		/// <summary>
		/// 다 찍는 데 필요한 글자 수
		/// </summary>
		private readonly int totalCharacters;

		/// <summary>
		/// 이 대사를 처음부터 끝까지 찍는 데 걸리는 시간.
		/// 글자당 시간으로 계산하되 그게 클립보다 길면 클립 길이에 맞춘다
		/// </summary>
		public double Duration { get; }

		/// <summary>
		/// 클립이 짧아서 글자당 시간을 못 지키고 빨라졌는지 체크
		/// DialogueClipEditor 가 클립을 주황색으로 칠하고
		/// DialogueClipInspector 가 인스펙터에 경고를 띄우는 데 쓴다
		/// </summary>
		public bool IsAccelerated { get; }

		/// <summary>
		/// 글자 수와 글자당 시간, 클립 길이로 계산한다.
		/// </summary>
		public TypingData(int totalCharacters, float secondsPerCharacter, double clipDuration)
		{
			this.totalCharacters = totalCharacters;

			double wanted = totalCharacters * (double)secondsPerCharacter;

			// 클립 안에 못 담으면 클립 길이에 맞춰 빨리 찍는다. 잘려서 못 읽는 것보단 나음
			this.Duration = wanted < clipDuration ? wanted : clipDuration;
			this.IsAccelerated = wanted > clipDuration;
		}

		/// <summary>
		/// 클립 시작하고 이만큼 지났을 때 몇 글자까지 보여야 하는지 계산함
		/// </summary>
		public int GetVisibleCount(double elapsed)
		{
			if (totalCharacters <= 0)
			{
				return 0;
			}

			// 속도를 0 으로 두면 타이핑 없이 바로 다 뜨는 셈이 된다
			if (Duration <= 0 || elapsed >= Duration)
			{
				return totalCharacters;
			}

			if (elapsed <= 0)
			{
				return 0;
			}

			// 올림이라 정지 지점처럼 아주 조금 모자란 시점에도 마지막 글자가 나온다
			return (int)Math.Ceiling(totalCharacters * elapsed / Duration);
		}
	}

	/// <summary>
	/// 대사 하나를 담는 타임라인 클립
	/// </summary>
	public class DialogueClip : PlayableAsset, ITimelineClipAsset, IStopPointClip
	{
		/// <summary>
		/// 새 클립을 만들 때의 길이.
		/// 타이핑이 들어가면서 늘렸다. 0.05초 기준으로 60자까지는 안 빨라지고 다 찍힌다
		/// </summary>
		private const double DefaultDuration = 3.0;

		/// <summary>
		/// 이 클립 구간에 표시할 대사
		/// </summary>
		[SerializeField]
		private DialogueLine line = new DialogueLine();

		/// <summary>
		/// 이 대사 끝에서 멈추고 입력을 기다릴지 여부.
		/// 끄면 다음 대사로 저절로 넘어간다
		/// </summary>
		[SerializeField]
		private bool waitForInput = true;

		/// <summary>
		/// 이 클립 구간에 표시할 대사
		/// </summary>
		public DialogueLine Line => line;

		/// <summary>
		/// 대사는 섞이지 않으므로 블렌딩을 막아 클립 겹침을 차단한다
		/// </summary>
		public ClipCaps clipCaps => ClipCaps.None;

		/// <summary>
		/// 새 클립이 생성될 때의 길이
		/// </summary>
		public override double duration => DefaultDuration;

		/// <summary>
		/// 이 클립 끝에서 재생을 멈추고 입력을 기다릴지 여부.
		/// 읽을 내용이 없으면 멈출 이유도 없으므로 빈 대사는 대기 설정과 무관하게 지나간다
		/// </summary>
		public bool CreatesStopPoint => waitForInput && line.HasText;

		/// <summary>
		/// 이 클립의 대사를 실어 나를 재생 노드를 만든다
		/// </summary>
		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<DialogueClipBehaviour> playable = ScriptPlayable<DialogueClipBehaviour>.Create(graph);
			playable.GetBehaviour().SetData(line);

			return playable;
		}
	}
}
