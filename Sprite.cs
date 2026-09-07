using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  internal class Sprite
  {
    public float X;
    public float Y;
    public float Priority;
    public Rectangle Rect;

    public Sprite(float x, float y, Rectangle rect)
    {
      X = x;
      Y = y;
      Priority = 0;
      Rect = rect;
    }

    public void Draw(Graphics g, Bitmap bmp)
    {
      Rectangle r = new(
        (int)(X - Rect.Width), (int)(Y - Rect.Height), Rect.Width * 2, Rect.Height * 2);
      g.DrawImage(bmp, r, Rect, GraphicsUnit.Pixel);
    }

    public void DrawWithRotation(Graphics g, Bitmap bmp, float rotation)
    {
      g.TranslateTransform(X, Y);
      g.RotateTransform(rotation * 180 / MathF.PI - 90);
      Rectangle r = new(-Rect.Width, -Rect.Height, Rect.Width * 2, Rect.Height * 2);
      g.DrawImage(bmp, r, Rect, GraphicsUnit.Pixel);
      g.ResetTransform();
    }
  }
}
