using TimelineVN.Timeline;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelineVN.SpriteSwap
{
	/// <summary>
	/// 스프라이트 트랙의 클립들을 받아 지금 시점에 보여줄 그림을 정한다
	/// </summary>
	public class SpriteTrackMixer : PlayableBehaviour
	{
		/// <summary>
		/// 지금 활성인 클립을 골라 렌더러에 적용시킨다
		/// </summary>
		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (playerData is not SpriteRenderer spriteRenderer)
			{
				return;
			}

			int activeInput = FindActiveInput(playable);

			// 클립이 없는 구간에서는 직전 그림을 그대로 둔다.
			if (activeInput < 0)
			{
				return;
			}

			ScriptPlayable<SpriteClipBehaviour> input = GetClipPlayable(playable, activeInput);

			input.GetBehaviour().Apply(spriteRenderer, new ClipTime(input.GetTime(), input.GetDuration()));
		}

		/// <summary>
		/// 지금 활성인 입력의 인덱스를 찾는다. 없으면 -1
		/// </summary>
		private static int FindActiveInput(Playable playable)
		{
			int inputCount = playable.GetInputCount();
			for (int i = 0; i < inputCount; i++)
			{
				if (playable.GetInputWeight(i) > 0f)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// 입력에 연결된 클립 노드를 꺼낸다
		/// </summary>
		private static ScriptPlayable<SpriteClipBehaviour> GetClipPlayable(Playable playable, int inputIndex)
		{
			return (ScriptPlayable<SpriteClipBehaviour>)playable.GetInput(inputIndex);
		}
	}
}
