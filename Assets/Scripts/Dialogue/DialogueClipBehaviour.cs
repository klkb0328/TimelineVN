using TimelineVN.Timeline;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 클립 하나의 대사 데이터를 재생 그래프에 실어 나른다
	/// </summary>
	public class DialogueClipBehaviour : PlayableBehaviour, IClipData<DialogueLine>
	{
		/// <summary>
		/// 클립이 시작된 뒤 로그를 남길 프레임 수
		/// </summary>
		private const int LogFramesAfterPlay = 2;

		private int processFrameLogRemaining;

		/// <summary>
		/// 클립에서 전달받은 대사 데이터
		/// </summary>
		public DialogueLine Data { get; private set; }

		/// <summary>
		/// 클립의 대사 데이터를 설정한다
		/// </summary>
		public void SetData(DialogueLine data)
		{
			this.Data = data;
		}

		// TODO : 아래 로그는 훅 호출 시점 체크 용임.. 테스트하고 삭제필요
		public override void OnPlayableCreate(Playable playable)
		{
			Debug.Log($"[Clip {Label}] f{Time.frameCount} OnPlayableCreate");
		}

		public override void OnGraphStart(Playable playable)
		{
			Debug.Log($"[Clip {Label}] f{Time.frameCount} OnGraphStart");
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			// 클립이 시작되는 프레임부터 다시 로그를 남기도록 되돌린다
			processFrameLogRemaining = LogFramesAfterPlay;
			Debug.Log($"[Clip {Label}] f{Time.frameCount} OnBehaviourPlay");
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			Debug.Log($"[Clip {Label}] f{Time.frameCount} OnBehaviourPause");
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (processFrameLogRemaining <= 0)
			{
				return;
			}

			processFrameLogRemaining--;
			Debug.Log($"[Clip {Label}] f{Time.frameCount} ProcessFrame playerData={Describe(playerData)}");
		}

		public override void OnGraphStop(Playable playable)
		{
			Debug.Log($"[Clip {Label}] f{Time.frameCount} OnGraphStop");
		}

		public override void OnPlayableDestroy(Playable playable)
		{
			Debug.Log($"[Clip {Label}] f{Time.frameCount} OnPlayableDestroy");
		}

		/// <summary>
		/// 로그에서 클립을 구분하기 위한 표시용 이름
		/// </summary>
		private string Label => Data == null ? "?" : Data.Text;

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
