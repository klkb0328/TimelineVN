using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 선택지 클립들을 올려 선택지 UI 를 제어하는 트랙.
	/// 선택지에서 시작하도록 유도하려고 나머지 클립은 우클릭 메뉴 한 단 아래로 넣었다
	/// </summary>
	/// <example>
	/// 대충 사용법은 다음과 같다! (아마 클립 추가 해보면 쉬울것이다)
	/// 트랙 위에서 우클릭하면
	///   Add Choice Show      여기서 시작하면 복귀 지점이 딸려 나온다
	///   Add Manual >         지웠을 때 되살리거나, 복귀 지점을 더 놓을 때 쓴다
	///     Choice Entry
	///     Choice Exit
	///     Choice Return
	/// </example>
	[DisplayName("Choice Track")]
	[TrackClipType(typeof(ChoiceShowClip))]
	[TrackClipType(typeof(ChoiceEntryClip))]
	[TrackClipType(typeof(ChoiceExitClip))]
	[TrackClipType(typeof(ChoiceReturnClip))]
	[TrackBindingType(typeof(ChoiceUI))]
	public class ChoiceTrack : TrackAsset
	{
		/// <summary>
		/// 다음 선택지에 줄 번호. 편집자가 만질 값이 아니라 감춘다
		/// </summary>
		[SerializeField, HideInInspector]
		private int nextChoiceId = 1;

		/// <summary>
		/// 새 선택지에 줄 번호를 하나 떼어 준다. 한 번 나간 번호는 다시 안 쓴다.
		/// 지웠다고 번호를 돌려쓰면, 남아 있는 분기와 새 선택지가 같은 번호로 보인다
		/// </summary>
		/// <example>
		/// 대충 선택지를 셋 만들면      ->  1, 2, 3
		/// 2번을 지우고 하나 더 만들면  ->  4가 나옴. 2로 안되돌아감!
		/// </example>
		public int TakeNextChoiceId()
		{
			// 솔직히 "터질 가능성이 0"는 아닌데 현실적으로 넘기 힘들거라 생각함.
			int taken = nextChoiceId;
			nextChoiceId++;

			return taken;
		}

		/// <summary>
		/// 이 트랙의 클립들을 받아 선택지 UI 를 제어할 믹서를 만든다
		/// </summary>
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<ChoiceTrackMixer>.Create(graph, inputCount);
		}
	}
}
