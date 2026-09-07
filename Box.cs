using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  internal class Box
  {
    public float Left;
    public float Right;
    public float Top;
    public float Bottom;

    public Box(float left, float right, float top, float bottom)
    {
      Set(left, right, top, bottom);
    }

    public void Set(float left, float right, float top, float bottom)
    {
      Left = left;
      Right = right;
      Top = top;
      Bottom = bottom;
    }

    public static Box Add(Box box, float x, float y)
    {
      return new Box(box.Left + x, box.Right + x, box.Top + y, box.Bottom + y);
    }

    public bool IsCollide(Box other)
    {
      return Left < other.Right && Right >= other.Left && Top < other.Bottom && Bottom >= other.Top;
    }
  }
}
