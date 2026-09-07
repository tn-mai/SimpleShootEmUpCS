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
    public Sprite Sprite;
    public Box Hitbox;
    public List<NormalBullet> Bullets;
    public float ShotInterval;
    public bool IsDead;

    public Player()
    {
      Sprite = new(Form1.nativeWidth / 2, Form1.nativeHeight / 2, new(0, 512 - 48, 32, 48));
      Hitbox = new(-16, 16, -16, 16);
      Bullets = new();
      ShotInterval = 0;
      IsDead = false;
    }

    public void Init()
    {
      Sprite.X = Form1.nativeWidth / 2;
      Sprite.Y = Form1.nativeHeight * 2 / 3;
      Sprite.Rect.Width = 0;
      Sprite.Rect.Height = 512 - 48;
      Sprite.Rect.Width = 32;
      Sprite.Rect.Height = 48;
      Hitbox.Set(-16, 16, -16, 16);
      ShotInterval = 0;
      IsDead = false;
      Bullets.Clear();
    }

    public void Update(float deltaTime)
    {
      if (!IsDead)
      {
        if (Form1.GetAsyncKeyState('A') < 0)
        {
          Sprite.X -= 600 * deltaTime;
        }
        if (Form1.GetAsyncKeyState('D') < 0)
        {
          Sprite.X += 600 * deltaTime;
        }
        if (Form1.GetAsyncKeyState('W') < 0)
        {
          Sprite.Y -= 600 * deltaTime;
        }
        if (Form1.GetAsyncKeyState('S') < 0)
        {
          Sprite.Y += 600 * deltaTime;
        }
        if (Form1.GetAsyncKeyState(Form1.vkSpace) < 0)
        {
          ShotInterval -= deltaTime;
          if (ShotInterval <= 0)
          {
            Bullets.Add(new NormalBullet(Sprite.X, Sprite.Y,
              new(32, 424, 16, 32), new(-12, 12, -16, 24), 0, -1200));
            ShotInterval += 0.1f;
          }
        }
        else
        {
          ShotInterval = 0;
        }
      }

      // 画面端の移動制限
      if (Sprite.X < 24)
      {
        Sprite.X = 24;
      }
      if (Sprite.X > Form1.nativeWidth - 24)
      {
        Sprite.X = Form1.nativeWidth - 24;
      }
      if (Sprite.Y < 48)
      {
        Sprite.Y = 48;
      }
      if (Sprite.Y > Form1.nativeHeight - 32)
      {
        Sprite.Y = Form1.nativeHeight - 32;
      }

      // プレイヤーの弾の移動
      for (int a = 0; a < Bullets.Count; a++)
      {
        Bullets[a].Update(deltaTime);
      }
      Bullets.RemoveAll(NormalBullet.IsOutsideWindow);
    }
  }
}
