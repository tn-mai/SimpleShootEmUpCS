using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  // 弾丸
  internal class NormalBullet
  {
    public Sprite Sprite;
    public Box Hitbox;
    public float MoveSpeedX;
    public float MoveSpeedY;

    // コンストラクタ
    public NormalBullet(float x, float y, Rectangle rect, Box hitbox, float sx, float sy)
    {
      Sprite = new(x, y, rect);
      Hitbox = hitbox;
      MoveSpeedX = sx;
      MoveSpeedY = sy;
    }

    public void Update(float deltaTime)
    {
      Sprite.X += MoveSpeedX * deltaTime;
      Sprite.Y += MoveSpeedY * deltaTime;
    }

    public static bool IsOutsideWindow(NormalBullet bullet)
    {
      Sprite s = bullet.Sprite;
      return s.X + s.Rect.Width / 2 < 0 || s.X - s.Rect.Width / 2 > Form1.nativeWidth ||
        s.Y + s.Rect.Height / 2 < 0 || s.Y - s.Rect.Height / 2 > Form1.nativeHeight;
    }
  }
}
