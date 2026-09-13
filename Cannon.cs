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
    public float X;
    public float Y;
    public int Count; // 発射数(Way数)
    public float Range; // 発射範囲
    public float Interval; // 射撃開始間隔
    public float Timer;

    public Cannon(float x, float y, int count, float range, float interval, float firstInterval)
    {
      X = x;
      Y = y;
      Count = count;
      if (Count < 1)
      {
        Count = 1;
      }
      Range = range * MathF.PI / 180.0f;
      Interval = interval;
      Timer = interval - firstInterval;
    }

    public void Update(float deltaTime, Character shooter, List<NormalBullet> bulletList, Character? target)
    {
      if (Timer < Interval)
      {
        Timer += deltaTime;
        return;
      }
      Timer -= Interval;

      // 発射位置が画面外の場合は発射しない
      if (shooter.X + X < 0 || shooter.X + X >= Form1.nativeWidth || shooter.Y + Y < 0 || shooter.Y + Y >= Form1.nativeHeight)
      {
        return;
      }

      float direction = -MathF.PI * 0.5f;
      if (target != null)
      {
        float dx = target.X - shooter.X - X;
        float dy = target.Y - shooter.Y - Y;
        direction = MathF.Atan2(dy, dx);
      }

      float r = Range / (Count - 1);
      float d = direction - Range * 0.5f;
      for (int a = 0; a < Count; a++)
      {
        NormalBullet bullet = new(shooter.X + X, shooter.Y + Y,
          500 * MathF.Cos(d), 500 * MathF.Sin(d), BulletType.EnemyNormal);
        bulletList.Add(bullet);
        d += r;
      }

    }
  } // Cannonクラスブロックの終わり


  // 弾丸発射パーツ
  internal class RotationCannon
  {
    public float X;
    public float Y;
    public int Direction; // 0=時計回り32Way 1=反時計回り32Way
    public float Interval; // 射撃開始間隔
    public float Timer;
    public float FiredCount;

    public RotationCannon(float x, float y, int direction, float interval, float firstInterval)
    {
      X = x;
      Y = y;
      Direction = direction;
      Interval = interval;
      Timer = interval - firstInterval;
      FiredCount = 0;
    }

    public void Update(float deltaTime, Character shooter, List<NormalBullet> bulletList, Character? target)
    {
      if (Timer < Interval)
      {
        Timer += deltaTime;
        return;
      }

      float d = FiredCount * (MathF.PI * 0.0625f) - MathF.PI * 0.5f;
      if (Direction == 1)
      {
        d += MathF.PI;
        d *= -1;
      }
      NormalBullet bullet = new(shooter.X + X, shooter.Y + Y,
        500 * MathF.Cos(d), -500 * MathF.Sin(d), BulletType.EnemyNormal);
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
  } // RotationCannonクラスブロックの終わり
}
