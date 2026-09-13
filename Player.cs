using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  // 自機
  internal class Player
  {
    public Character c;
    public Character engine;
    public List<NormalBullet> Bullets;
    public float ShotInterval;
    public int ShotLevel;
    public bool IsDead;

    private static Rectangle[] rect = { new(0, 512 - 48, 32, 48) };
    private static Rectangle[] rectEngine = { new(32, 512 - 8, 16, 8), new(32, 512 - 16, 16, 8), new(32, 512 - 24, 16, 8) };

    public Player()
    {
      c = new(Form1.nativeWidth / 2, Form1.nativeHeight / 2, 1, new(-16, 16, -16, 16), rect, 15);
      engine = new(c.X, c.Y + 40, 1, Box.Empty, rectEngine, 15);
      engine.AnimeIsLoop = true;
      Bullets = new();
      ShotInterval = 0;
      ShotLevel = 1;
      IsDead = false;
    }

    public void Init()
    {
      c.X = Form1.nativeWidth / 2;
      c.Y = Form1.nativeHeight * 2 / 3;
      c.ResetAnimeTimer();

      engine.X = c.X;
      engine.Y = c.Y + 40;
      engine.ResetAnimeTimer();

      Bullets.Clear();
      ShotInterval = 0;
      ShotLevel = 1;
      IsDead = false;
    }

    public void Update(float deltaTime)
    {
      c.Update(deltaTime);
      engine.Update(deltaTime);

      if (!IsDead)
      {
        if (Form1.GetAsyncKeyState('A') < 0)
        {
          c.X -= 600 * deltaTime;
        }
        if (Form1.GetAsyncKeyState('D') < 0)
        {
          c.X += 600 * deltaTime;
        }
        if (Form1.GetAsyncKeyState('W') < 0)
        {
          c.Y -= 600 * deltaTime;
        }
        if (Form1.GetAsyncKeyState('S') < 0)
        {
          c.Y += 600 * deltaTime;
        }
        if (Form1.GetAsyncKeyState(Form1.vkSpace) < 0)
        {
          ShotInterval -= deltaTime;
          if (ShotInterval <= 0)
          {
            if (ShotLevel % 2 == 1)
            {
              Bullets.Add(new NormalBullet(c.X, c.Y - 16, 0, -1200, BulletType.PlayerNormal));
            }
            else
            {
              Bullets.Add(new NormalBullet(c.X - 16, c.Y - 12, 0, -1200, BulletType.PlayerNormal));
              Bullets.Add(new NormalBullet(c.X + 16, c.Y - 12, 0, -1200, BulletType.PlayerNormal));
            }
            for (int a = 1; a < ((ShotLevel + 1) / 2); a += 1)
            {
              float mx = MathF.Cos((90 - a * 15) * MathF.PI / 180.0f) * 1200;
              float my = MathF.Sin((90 - a * 15) * MathF.PI / 180.0f) * -1200;
              Bullets.Add(new NormalBullet(c.X - a * 8, c.Y - 16, mx, my, BulletType.PlayerNormal));
              Bullets.Add(new NormalBullet(c.X + a * 8, c.Y - 16, -mx, my, BulletType.PlayerNormal));
            }
            ShotInterval += 0.15f;
          }
        }
        else
        {
          ShotInterval = 0;
        }
      }

      // 画面端の移動制限
      if (c.X < 24)
      {
        c.X = 24;
      }
      if (c.X > Form1.nativeWidth - 24)
      {
        c.X = Form1.nativeWidth - 24;
      }
      if (c.Y < 48)
      {
        c.Y = 48;
      }
      if (c.Y > Form1.nativeHeight - 32)
      {
        c.Y = Form1.nativeHeight - 32;
      }

      engine.X = c.X;
      engine.Y = c.Y + 40;

      // プレイヤーの弾の移動
      for (int a = 0; a < Bullets.Count; a++)
      {
        Bullets[a].Update(deltaTime);
      }
      Bullets.RemoveAll(NormalBullet.IsOutsideWindow);
    }
  }
}
