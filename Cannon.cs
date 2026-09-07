using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  // 弾丸発射パーツ
  internal class Cannon
  {
    // 0=1Way
    // 1=3Way
    // 2=5Way
    // 3=16Way
    // 4=時計回り32Way
    // 5=反時計回り32Way
    public float X;
    public float Y;
    public int Type;
    public float Interval; // 射撃開始間隔
    public float Timer;
    public float FiredCount;

    public Cannon(float x, float y, int type, float interval, float firstInterval)
    {
      X = x;
      Y = y;
      Type = type;
      Interval = interval;
      Timer = interval - firstInterval;
      FiredCount = 0;
    }

    public void Update(float deltaTime, Enemy enemy, List<NormalBullet> bulletList, Sprite? target)
    {
      if (Timer < Interval)
      {
        Timer += deltaTime;
        return;
      }

      Rectangle rect = new(336, 448, 16, 16);
      Box hitbox = new(-8, 8, -8, 8);

      switch (Type)
      {
      default:
      case 0: // 1Way
      case 1: // 3Way
      case 2: // 5Way
      case 3: // 16Way
        float direction = -MathF.PI * 0.5f;
        if (target != null)
        {
          float dx = target.X - enemy.Sprite.X - X;
          float dy = target.Y - enemy.Sprite.Y - Y;
          direction = MathF.Atan2(dy, dx);
        }

        int count = 1 + Type * 2;
        if (Type == 3)
        {
          count = 16;
        }
        for (int a = 0; a < count; a++)
        {
          float d = direction + (a - count / 2) * (MathF.PI * 0.125f);
          NormalBullet bullet = new(enemy.Sprite.X + X, enemy.Sprite.Y + Y,
            rect, hitbox, 500 * MathF.Cos(d), 500 * MathF.Sin(d));
          bulletList.Add(bullet);
        }

        Timer -= Interval;
        break;

      case 4: // 時計回り32Way
      case 5: // 反時計回り32Way
        {
          float d = FiredCount * (MathF.PI * 0.0625f) - MathF.PI * 0.5f;
          if (Type == 5)
          {
            d += MathF.PI;
            d *= -1;
          }
          NormalBullet bullet = new(enemy.Sprite.X + X, enemy.Sprite.Y + Y,
            rect, hitbox, 500 * MathF.Cos(d), -500 * MathF.Sin(d));
          bulletList.Add(bullet);
          if (FiredCount >= 32)
          {
            FiredCount = 0;
            Timer -= Interval;
          }
          else
          {
            FiredCount++;
            Timer -= deltaTime * 5;
          }
        }
        break;
      }
    }
  } // Cannonクラスブロックの終わり
}
