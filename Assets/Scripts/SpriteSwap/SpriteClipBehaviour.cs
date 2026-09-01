using TimelineVN.Timeline;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelineVN.SpriteSwap
{
	/// <summary>
	/// 클립 하나의 스프라이트를 들고 있다가 트랙에 연결해둔 렌더러에 넣는다
	/// </summary>
	public class SpriteClipBehaviour : PlayableBehaviour, ISingleClip<Sprite, SpriteRenderer>
	{
		/// <summary>
		/// 클립에서 전달받은 스프라이트
		/// </summary>
		public Sprite Data { get; private set; }

		/// <summary>
		/// 클립의 스프라이트를 설정한다
		/// </summary>
		public void SetData(Sprite data)
		{
			Data = data;
		}

		/// <summary>
		/// 들고 있는 스프라이트를 렌더러에 넣는다
		/// </summary>
		public void Apply(SpriteRenderer spriteRenderer, ClipTime time)
		{
			// 아직 안 채운 클립은 손대지 않는다. null 을 넣으면 클립에 경고 뜸
			if (Data == null)
			{
				return;
			}

			spriteRenderer.sprite = Data;
		}
	}
}
