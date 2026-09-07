using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  // アニメーション機能付きスプライト
  internal class AnimatedSprite
  {
    public Sprite Sprite;
    public float AnimeTimer;
    public int Count;
    public float Interval;
    public int X;

    public AnimatedSprite(float x, float y, int count, float interval, Rectangle rect)
    {
      Sprite = new(x, y, rect);
      AnimeTimer = 0;
      Count = count;
      Interval = interval;
      X = rect.X;
    }

    public void Update(float deltaTime)
    {
      AnimeTimer += Interval * deltaTime;
      int index = (int)AnimeTimer;
      if (index >= Count)
      {
        index = Count - 1;
      }
      Sprite.Rect.X = X + index * Sprite.Rect.Width;
    }
  }
}
