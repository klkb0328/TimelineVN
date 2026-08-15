using System;
using System.Collections.Generic;
using TimelineVN.Timeline;
using UnityEngine.Timeline;

namespace TimelineVN.Playback
{
	/// <summary>
	/// 타임라인의 정지 시각들을 들고, 매 프레임 그 중 하나를 지나쳤는지 본다.
	/// 여기는 판단만 한다. 실제로 멈추는 건 DirectorTimeControl 이고,
	/// 둘을 이어주는 건 VisualNovelDirector 다
	/// </summary>
	public class StopPointScanner
	{
		/// <summary>
		/// 클립 끝에서 이만큼 앞을 정지 시각으로 잡는다.
		/// 클립 끝 그 자체는 이미 다음 클립 구간이라, 거기서 멈추면 그 대사가 화면에 안 남는다
		/// </summary>
		private const double StopMargin = 0.000001;

		/// <summary>
		/// 밖에서 시간을 옮겼는지 판정하는 기준. 쉽게말해 스크럽같은거 일어났는지 체크용.
		/// 빡빡하게 잡으면 정상 재생을 점프로 오인해 그 프레임의 정지 판정을 건너뛰므로,
		/// 프레임 흔들림을 넉넉히 덮는 값으로 둔다
		/// </summary>
		private const double DiscontinuityThreshold = 0.01;

		/// <summary>
		/// 시간순으로 정렬된 정지 시각들
		/// </summary>
		private readonly double[] stopTimes;

		/// <summary>
		/// 지난 프레임의 재생 시각
		/// </summary>
		private double previousTime;

		/// <summary>
		/// 타임라인을 훑어 정지 시각 목록을 만든다
		/// 이말은 곧 타임라인 어셋(= 시나리오) 바뀌면 다시 불려야함!
		/// </summary>
		public StopPointScanner(TimelineAsset timeline, double startTime)
		{
			stopTimes = CollectStopTimes(timeline);
			previousTime = startTime;
		}

		/// <summary>
		/// 판정을 건너뛰고 기준 시각을 강제로 옮긴다.
		/// 정지 중 스크럽처럼 지나온 구간을 실제로 재생한 것이 아닐 때 쓴다
		/// </summary>
		public void ForceMoveTo(double time)
		{
			previousTime = time;
		}

		/// <summary>
		/// 지난 프레임 다음부터 현재 시각까지 사이에서 지나친 정지 시각을 가져온다.
		/// 여러 개가 걸리면 가장 이른 것을 준다
		/// </summary>
		/// <example>
		/// 정지 시각이 [1.0, 2.0, 3.0] 이고 60fps 로 도는 중이라면
		/// 지난 1.98, 지금 2.00  ->  2.0 을 준다. 평범하게 지나친 경우
		/// 지난 1.90, 지금 2.50  ->  2.0 을 준다. 랙으로 훌쩍 넘겨도 놓치지 않는다
		/// 지난 1.90, 지금 3.50  ->  2.0 을 준다. 두 개가 걸리면 이른 쪽부터 하나씩
		/// 지난 1.90, 지금 8.00  ->  아무것도 안 준다. 스크럽으로 보고 기준만 8.0 으로 옮긴다
		/// </example>
		public bool TryGetPassedStopTime(double currentTime, double expectedDelta, out double stopTime)
		{
			// 정상 재생이면 이번 프레임에 흐른 양이 예상과 맞아야 한다. 크게 어긋났다는 건
			// 스크럽이나 지점 점프처럼 재생이 아닌 방식으로 시간이 옮겨졌다는 뜻이다
			if (Math.Abs(currentTime - previousTime - expectedDelta) > DiscontinuityThreshold)
			{
				// 지나온 구간을 실제로 재생한 것이 아니므로 그 사이의 정지 지점은 잡지 않는다.
				// 잡으면 편집자가 끌어다 놓은 위치에서 앞쪽 정지 지점으로 튕겨 돌아온다
				previousTime = currentTime;
				stopTime = 0.0;

				return false;
			}

			bool found = TryFindInRange(previousTime, currentTime, out stopTime);

			// 멈춘 자리를 다음 프레임의 기준으로 삼는다. 범위 판정이 시작을 열어 두므로
			// 방금 잡은 지점이 재개 직후에 또 걸리지는 않는다
			previousTime = found ? stopTime : currentTime;

			return found;
		}

		/// <summary>
		/// 주어진 시각 뒤에 오는 첫 정지 시각을 가져온다.
		/// 연출을 건너뛰고 대기 지점으로 바로 갈 때 쓴다
		/// </summary>
		/// <example>
		/// 정지 시각이 [1.0, 2.0, 3.0] 일 때
		/// 지금 1.4  ->  2.0 을 준다. 연출 도중 클릭하면 여기로 점프한다
		/// 지금 3.5  ->  앞에 남은 게 없으니 거짓. 마지막 대사를 넘긴 뒤가 이렇다
		/// </example>
		public bool TryGetNextStopTime(double currentTime, out double stopTime)
		{
			for (int i = 0; i < stopTimes.Length; i++)
			{
				if (stopTimes[i] > currentTime)
				{
					stopTime = stopTimes[i];

					return true;
				}
			}

			stopTime = 0.0;

			return false;
		}

		/// <summary>
		/// 구간에 든 정지 시각 중 가장 이른 것을 찾는다.
		/// 시작은 포함하지 않고 끝은 포함한다
		/// </summary>
		/// <example>
		/// 정지 시각이 [1.0, 2.0, 3.0] 일 때
		/// (0.9, 1.1]  ->  1.0
		/// (1.9, 2.5]  ->  2.0
		/// (1.9, 3.5]  ->  2.0. 둘이 걸려도 이른 쪽
		/// (1.0, 1.5]  ->  없음. 시작인 1.0 은 안 친다. 방금 여기서 멈췄을 테니까
		/// </example>
		private bool TryFindInRange(double from, double to, out double stopTime)
		{
			for (int i = 0; i < stopTimes.Length; i++)
			{
				// 시작을 열어 두는 이유는 방금 멈춘 지점을 다음 프레임에 또 잡지 않기 위해서다.
				// 정지하면 기준 시각이 그 지점이 되므로 닫아 두면 무한히 다시 걸린다
				if (stopTimes[i] > from && stopTimes[i] <= to)
				{
					// 목록이 시간순이라 처음 걸리는 것이 곧 가장 이른 것이다.
					// 늦은 쪽을 고르면 그 사이의 대사가 통째로 사라진다
					stopTime = stopTimes[i];

					return true;
				}
			}

			stopTime = 0.0;

			return false;
		}

		/// <summary>
		/// 타임라인의 클립들에서 정지 시각을 뽑아 시간순으로 정렬한다
		/// </summary>
		private static double[] CollectStopTimes(TimelineAsset timeline)
		{
			if (timeline == null)
			{
				return Array.Empty<double>();
			}

			List<double> collected = new List<double>();

			foreach (TrackAsset track in timeline.GetOutputTracks())
			{
				// 뮤트한 트랙은 믹서가 만들어지지 않아 대사가 안 뜬다. 정지 지점만 남으면
				// 빈 대사창인 채로 매번 멈추게 되고 화면에 원인을 알 단서가 없다
				if (track.mutedInHierarchy)
				{
					continue;
				}

				foreach (TimelineClip clip in track.GetClips())
				{
					if (clip.asset is IStopPointClip stopPointClip && stopPointClip.CreatesStopPoint)
					{
						collected.Add(clip.end - StopMargin);
					}
				}
			}

			// 트랙이 여럿이면 뽑은 순서가 시간순이 아니다. 여기서 한 번 정렬해 두면
			// 구간 판정에서 처음 걸리는 것이 곧 가장 이른 것이 된다
			collected.Sort();

			return collected.ToArray();
		}
	}
}
