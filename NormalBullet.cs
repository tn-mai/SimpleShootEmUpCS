using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  // 弾丸の種類
  internal class BulletType
  {
    public static int PlayerNormal = 0;
    public static int EnemyNormal = 1;
  }

  // 弾丸
  internal class NormalBullet
  {
    public Character c;
    public float MoveSpeedX;
    public float MoveSpeedY;

    private static readonly Rectangle[][] rectList = {
      new Rectangle[]{ new(32, 424, 16, 32) },
      new Rectangle[]{ new(336, 448, 16, 16) },
    };

    private static readonly Box[] hitboxList = {
      new(-12, 12, -16, 24),
      new(-8, 8, -8, 8),
    };

    // コンストラクタ
    public NormalBullet(float x, float y, float sx, float sy, int type)
    {
      c = new(x, y, 1, hitboxList[type], rectList[type], 10);
      MoveSpeedX = sx;
      MoveSpeedY = sy;
    }

    public void Update(float deltaTime)
    {
      c.X += MoveSpeedX * deltaTime;
      c.Y += MoveSpeedY * deltaTime;
    }

    public static bool IsOutsideWindow(NormalBullet bullet)
    {
      Rectangle rect = bullet.c.GetRect();
      return bullet.c.X + rect.Width / 2 < 0 || bullet.c.X - rect.Width / 2 > Form1.nativeWidth ||
        bullet.c.Y + rect.Height / 2 < 0 || bullet.c.Y - rect.Height / 2 > Form1.nativeHeight;
    }
  }
}
