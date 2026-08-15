using UnityEngine;
using UnityEngine.Playables;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 대사 트랙의 클립들을 받아 지금 표시할 대사를 정한다
	/// </summary>
	public class DialogueTrackMixer : PlayableBehaviour
	{
		/// <summary>
		/// 직전 프레임에 활성이던 입력 인덱스. 첫 비교가 성립하도록 -2 로 시작한다
		/// </summary>
		private int lastActiveInput = -2;

		// TODO : 호출 시점 체크용. 체크후 제거필요
		public override void OnPlayableCreate(Playable playable)
		{
			Debug.Log($"[Mixer] f{Time.frameCount} OnPlayableCreate");
		}

		public override void OnGraphStart(Playable playable)
		{
			Debug.Log($"[Mixer] f{Time.frameCount} OnGraphStart isPlaying={Application.isPlaying}");
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			Debug.Log($"[Mixer] f{Time.frameCount} OnBehaviourPlay");
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			Debug.Log($"[Mixer] f{Time.frameCount} OnBehaviourPause");
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			int activeInput = FindActiveInput(playable);
			if (activeInput == lastActiveInput)
			{
				return;
			}

			lastActiveInput = activeInput;
			Debug.Log($"[Mixer] f{Time.frameCount} ProcessFrame playerData={Describe(playerData)} activeInput={activeInput}");
		}

		public override void OnGraphStop(Playable playable)
		{
			Debug.Log($"[Mixer] f{Time.frameCount} OnGraphStop");
		}

		public override void OnPlayableDestroy(Playable playable)
		{
			Debug.Log($"[Mixer] f{Time.frameCount} OnPlayableDestroy");
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
		/// playerData 로 실제 무엇이 들어오는지 로그에 남길 문자열을 만든다
		/// </summary>
		private static string Describe(object playerData)
		{
			if (playerData == null)
			{
				return "null";
			}

			return $"{playerData.GetType().Name}";
		}
	}
}
