using System;
using System.Collections.Generic;
using TimelineVN.Timeline;
using UnityEngine.Timeline;

namespace TimelineVN.Playback
{
	/// <summary>
	/// 시간축 위의 한 점에서 무슨 일이 일어나는지를 나타낸다
	/// 원래는 Stop만 했는데 이제는 점프도 되고 연출끝도 따로 시간으로 관리하니까 이렇게 했다
	/// </summary>
	public enum VNTimePointKind
	{
		/// <summary>
		/// 멈추고 플레이어 입력을 기다린다
		/// </summary>
		Stop,

		/// <summary>
		/// 다른 자리로 옮겨간다
		/// </summary>
		Jump,

		/// <summary>
		/// 장면이 끝난다
		/// </summary>
		SceneEnd,
	}

	/// <summary>
	/// 시간축 위의 한 점. 재생이 여기를 지나치면 무언가 일어난다.
	/// 종류마다 필요한 정보가 달라서 한 덩어리로 묶어 둔다.
	/// </summary>
	/// <example>
	/// 2.5초   멈춘다
	/// 16초    9초로 옮겨간다
	/// 25초    끝난다
	/// </example>
	public readonly struct VNTimePoint
	{
		/// <summary>
		/// 여기를 지나치면 무슨 일이 일어나는지
		/// </summary>
		public VNTimePointKind Kind { get; }

		/// <summary>
		/// 이 점의 시각
		/// </summary>
		public double Time { get; }

		/// <summary>
		/// 이 점을 만든 클립이 시작하는 시각. 스킵 판정에 쓴다.
		/// 스킵은 지금 들어와 있는 클립 안에서만 되므로 어디서부터가 그 클립인지 알아야 한다.
		/// Stop 에만 의미가 있다
		/// </summary>
		public double ClipStart { get; }

		/// <summary>
		/// 옮겨갈 자리. Jump 에만 의미가 있다
		/// TODO : 이것만 이렇게 한게 조금 아쉽긴한데 일단 당장 수정할건 없어보여서 이렇게함.
		/// </summary>
		public JumpTarget Destination { get; }

		private VNTimePoint(VNTimePointKind kind, double time, double clipStart, JumpTarget destination)
		{
			Kind = kind;
			Time = time;
			ClipStart = clipStart;
			Destination = destination;
		}

		/// <summary>
		/// 멈추는 점. 스킵이 클립 구간을 알아야 해서 시작 시각도 같이 받는다
		/// </summary>
		public static VNTimePoint CreateStop(double time, double clipStart)
		{
			return new VNTimePoint(VNTimePointKind.Stop, time, clipStart, null);
		}

		/// <summary>
		/// 옮겨가는 점. 목적지 없이는 만들 이유가 없어서 부르는 쪽이 미리 거른다
		/// </summary>
		public static VNTimePoint CreateJump(double time, JumpTarget destination)
		{
			return new VNTimePoint(VNTimePointKind.Jump, time, 0.0, destination);
		}

		/// <summary>
		/// 끝나는 점. 시각 말고는 들 것이 없다
		/// </summary>
		public static VNTimePoint CreateSceneEnd(double time)
		{
			return new VNTimePoint(VNTimePointKind.SceneEnd, time, 0.0, null);
		}
	}

	/// <summary>
	/// 타임라인에서 뽑아낸 시각들을 들고, 매 프레임 그 중 하나를 지나쳤는지 본다.
	/// 여기는 판단만 한다. 실제로 멈추고 옮기는 건 DirectorTimeControl 이고,
	/// 둘을 이어주는 건 VisualNovelDirector 다
	/// </summary>
	public class VNTimeScanner
	{
		/// <summary>
		/// 클립 끝에서 이만큼 앞을 판정 시각으로 잡는다.
		/// 클립 끝 그 자체는 이미 다음 클립 구간이라, 거기서 멈추면 그 대사가 화면에 안 남는다
		/// </summary>
		private const double StopMargin = 0.000001;

		/// <summary>
		/// 밖에서 시간을 옮겼는지 판정하는 기준. 쉽게말해 스크럽같은거 일어났는지 체크용.
		/// 빡빡하게 잡으면 정상 재생을 점프로 오인해 그 프레임의 판정을 건너뛰므로,
		/// 프레임 흔들림을 넉넉히 덮는 값으로 둔다
		/// </summary>
		private const double DiscontinuityThreshold = 0.01;

		/// <summary>
		/// 시간순으로 정렬된 점들
		/// </summary>
		private readonly VNTimePoint[] points;

		/// <summary>
		/// 도착지 클립이 몇 초에 있는지.
		/// 선택지를 고른 뒤 그 분기가 몇 초인지 알아야 해서 미리 만들어 둔다
		/// </summary>
		private readonly Dictionary<JumpTarget, double> jumpTimes;

		/// <summary>
		/// 지난 프레임의 재생 시각
		/// </summary>
		private double previousTime;

		/// <summary>
		/// 타임라인을 훑어 점 목록과 도착지 시각을 만든다.
		/// 이말은 곧 타임라인 어셋(= 시나리오) 바뀌면 다시 불려야함!
		/// </summary>
		public VNTimeScanner(TimelineAsset timeline, double startTime)
		{
			var collectedPoints = new List<VNTimePoint>();
			var collectedJumpTimes = new Dictionary<JumpTarget, double>();

			Collect(timeline, collectedPoints, collectedJumpTimes);

			// 트랙이 여럿이면 뽑은 순서가 시간순이 아니다. 여기서 한 번 정렬해 두면
			// 구간 판정에서 처음 걸리는 것이 곧 가장 이른 것이 된다
			collectedPoints.Sort((left, right) => left.Time.CompareTo(right.Time));

			points = collectedPoints.ToArray();
			jumpTimes = collectedJumpTimes;
			previousTime = startTime;
		}

		/// <summary>
		/// 판정을 건너뛰고 기준 시각을 강제로 옮긴다.
		/// 점프나 스크럽처럼 지나온 구간을 실제로 재생한 것이 아닐 때 쓴다
		/// </summary>
		public void ForceMoveTo(double time)
		{
			previousTime = time;
		}

		/// <summary>
		/// 지난 프레임 다음부터 현재 시각까지 사이에서 지나친 점을 가져온다.
		/// 여러 개가 걸리면 가장 이른 것을 준다
		/// </summary>
		/// <example>
		/// 점이 [1.0 멈춤, 2.0 멈춤, 3.0 옮김] 이고 60fps 로 도는 중이라면
		/// 지난 1.98, 지금 2.00  ->  2.0 멈춤. 평범하게 지나친 경우
		/// 지난 1.90, 지금 2.50  ->  2.0 멈춤. 랙으로 훌쩍 넘겨도 놓치지 않는다
		/// 지난 1.90, 지금 3.50  ->  2.0 멈춤. 두 개가 걸리면 이른 쪽부터 하나씩
		/// 지난 1.90, 지금 8.00  ->  아무것도 안 준다. 스크럽으로 보고 기준만 8.0 으로 옮긴다
		/// </example>
		public bool TryGetPassedPoint(double currentTime, double expectedDelta, out VNTimePoint point)
		{
			// 정상 재생이면 이번 프레임에 흐른 양이 예상과 맞아야 한다. 크게 어긋났다는 건
			// 스크럽이나 지점 점프처럼 재생이 아닌 방식으로 시간이 옮겨졌다는 뜻이다
			if (Math.Abs(currentTime - previousTime - expectedDelta) > DiscontinuityThreshold)
			{
				// 지나온 구간을 실제로 재생한 것이 아니므로 그 사이의 점은 잡지 않는다.
				// 잡으면 편집자가 끌어다 놓은 위치에서 앞쪽 점으로 튕겨 돌아온다
				previousTime = currentTime;
				point = default;

				return false;
			}

			bool found = TryFindInRange(previousTime, currentTime, out point);

			// 걸린 자리를 다음 프레임의 기준으로 삼는다. 범위 판정이 시작을 열어 두므로
			// 방금 잡은 점이 재개 직후에 또 걸리지는 않는다
			previousTime = found ? point.Time : currentTime;

			return found;
		}

		/// <summary>
		/// 지금 들어와 있는 클립의 정지 시각을 가져온다.
		/// 연출을 건너뛰고 그 대사의 대기 지점으로 바로 갈 때 쓴다.
		/// 클립 밖이면 건너뛸 게 없어서 거짓임.
		/// 시간축 전체에서 다음 걸 찾는 식으로 바꾸면 분기 출구를 뛰어넘어서 샌다.. 조심
		/// </summary>
		/// <example>
		/// 대사1 이 0~2, 대사2 가 4~6 이면 정지 시각은 각각 1.999999, 5.999999 다
		/// 지금 0.5  ->  1.999999. 대사1 의 남은 연출을 건너뛴다
		/// 지금 3.0  ->  거짓. 클립 사이 간격이라 이미 넘긴 대사다
		/// 지금 4.5  ->  5.999999
		/// </example>
		public bool TryGetSkipTarget(double currentTime, out double skipTime)
		{
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].Kind != VNTimePointKind.Stop)
				{
					continue;
				}

				// 정지 시각 자체는 뺀다. 거기 서 있으면 이미 대기 중이라 건너뛸 연출이 없다
				if (currentTime >= points[i].ClipStart && currentTime < points[i].Time)
				{
					skipTime = points[i].Time;

					return true;
				}
			}

			skipTime = 0.0;

			return false;
		}

		/// <summary>
		/// 도착지 클립이 몇 초에 있는지 가져온다.
		/// 클립 자신은 자기 시각을 모른다. 시각은 그 클립을 담고 있는 껍데기가 갖는다
		/// </summary>
		public bool TryGetJumpTime(JumpTarget target, out double time)
		{
			// 참조가 비어 있는 것은 편집 중에 흔하다. 여기서 걸러 재생이 죽지 않게 한다
			if (target == null)
			{
				time = 0.0;

				return false;
			}

			return jumpTimes.TryGetValue(target, out time);
		}

		/// <summary>
		/// 구간에 든 점 중 가장 이른 것을 찾는다.
		/// 시작은 포함하지 않고 끝만 포함함!!!!
		/// </summary>
		/// <example>
		/// 점이 [1.0, 2.0, 3.0] 일 때
		/// (0.9, 1.1]  ->  1.0
		/// (1.9, 2.5]  ->  2.0
		/// (1.9, 3.5]  ->  2.0. 둘이 걸려도 이른 쪽
		/// (1.0, 1.5]  ->  없음. 시작인 1.0 은 안 친다. 방금 여기서 멈췄을 테니까..
		/// </example>
		private bool TryFindInRange(double from, double to, out VNTimePoint point)
		{
			for (int i = 0; i < points.Length; i++)
			{
				// 시작을 열어 두는 이유는 방금 처리한 점을 다음 프레임에 또 잡지 않기 위해서다.
				// 정지하면 기준 시각이 그 지점이 되므로 닫아 두면 무한히 다시 걸린다
				if (points[i].Time > from && points[i].Time <= to)
				{
					// 목록이 시간순이라 처음 걸리는 것이 곧 가장 이른 것이다.
					// 늦은 쪽을 고르면 그 사이의 대사가 통째로 사라진다
					point = points[i];

					return true;
				}
			}

			point = default;

			return false;
		}

		/// <summary>
		/// 타임라인의 클립들에서 점과 도착지 시각을 뽑는다.
		/// 한 번의 순회로 둘 다 채운다
		/// </summary>
		private static void Collect(TimelineAsset timeline, List<VNTimePoint> points,
			Dictionary<JumpTarget, double> jumpTimes)
		{
			if (timeline == null)
			{
				return;
			}

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
					CollectFromClip(clip, points, jumpTimes);
				}
			}
		}

		/// <summary>
		/// 클립 하나에서 뽑을 것을 전부 뽑는다.
		/// 한 클립이 둘 이상에 해당할 수 있어서 검사를 else 로 잇지 않는다.
		/// MainEndClip 이 그렇다. 도착지이면서 장면 끝이다
		/// </summary>
		private static void CollectFromClip(TimelineClip clip, List<VNTimePoint> points,
			Dictionary<JumpTarget, double> jumpTimes)
		{
			// 도착지는 지나쳐도 아무 일이 없다. 목적지로 지목됐을 때 몇 초인지만 필요하다
			if (clip.asset is JumpTarget jumpTarget)
			{
				jumpTimes[jumpTarget] = clip.start;
			}

			if (clip.asset is IStopPointClip stopPointClip && stopPointClip.CreatesStopPoint)
			{
				points.Add(VNTimePoint.CreateStop(clip.end - StopMargin, clip.start));
			}

			// 목적지가 없으면 옮겨갈 데가 없으니 점을 만들지 않는다. 그냥 지나간다
			if (clip.asset is IJumpStartClip jumpStartClip && jumpStartClip.HasDestination)
			{
				points.Add(VNTimePoint.CreateJump(clip.start, jumpStartClip.Destination));
			}

			// 클립 끝에서 끝낸다. 몸통이 마지막 대사 뒤 여백이자 마무리 연출 자리다.
			// 마진을 빼는 건 이 클립이 타임라인의 마지막일 때 엔진의 자연 종료보다
			// 먼저 판정할 프레임을 잡기 위해서다
			if (clip.asset is MainEndClip)
			{
				points.Add(VNTimePoint.CreateSceneEnd(clip.end - StopMargin));
			}
		}
	}
}
