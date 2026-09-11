using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  internal class Character
  {
    public float X;
    public float Y;
    public float Priority;
    public int Hp;
    public Box Hitbox;
    public Rectangle[] AnimeRect;
    public float AnimeTimestep;
    public float AnimeTimer;
    public bool AnimeIsLoop;
    public bool AnimeIsEnd;

    public Character(float x, float y, int hp, Box hitbox, Rectangle[] animeRect, float timestep)
    {
      X = x;
      Y = y;
      Priority = 0;
      Hp = hp;
      Hitbox = hitbox;
      AnimeRect = animeRect;
      AnimeTimestep = timestep;
      AnimeTimer = 0;
      AnimeIsLoop = false;
      AnimeIsEnd = false;
    }

    // アニメーションを更新する
    public void Update(float deltaTime)
    {
      if (AnimeIsEnd)
      {
        return;
      }
      AnimeTimer += AnimeTimestep * deltaTime;
      if (AnimeTimer >= AnimeRect.Length)
      {
        if (AnimeIsLoop)
        {
          AnimeTimer -= AnimeRect.Length;
        }
        else
        {
          AnimeTimer = AnimeRect.Length - 1;
          AnimeIsEnd = true;
        }
      }
    }

    public void ResetAnimeTimer()
    {
      AnimeTimer = 0;
      AnimeIsEnd = false;
    }

    public float GetWidth()
    {
      return GetRect().Width;
    }

    public float GetHeight()
    {
      return GetRect().Height;
    }

    public Rectangle GetRect()
    {
      return AnimeRect[(int)AnimeTimer];
    }

    // キャラクターを描く
    public void Draw(Graphics g, Bitmap bmp)
    {
      Rectangle a = GetRect();
      Rectangle r = new(
        (int)(X - a.Width), (int)(Y -a.Height), a.Width * 2, a.Height * 2);
      g.DrawImage(bmp, r, a, GraphicsUnit.Pixel);
    }

    // 回転付きで描く
    public void DrawWithRotation(Graphics g, Bitmap bmp, float rotation)
    {
      g.TranslateTransform(X, Y);
      g.RotateTransform(rotation * 180 / MathF.PI - 90);
      Rectangle a = GetRect();
      Rectangle r = new(-a.Width, -a.Height, a.Width * 2, a.Height * 2);
      g.DrawImage(bmp, r, a, GraphicsUnit.Pixel);
      g.ResetTransform();
    }
  }
}
