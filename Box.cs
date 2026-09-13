using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  // 長方形クラス
  internal class Box
  {
    public float Left;
    public float Right;
    public float Top;
    public float Bottom;

    public static readonly Box Empty = new(0, 0, 0, 0);

    // コンストラクタ
    public Box(float left, float right, float top, float bottom)
    {
      Left = left;
      Right = right;
      Top = top;
      Bottom = bottom;
    }

    // 長方形同士の衝突判定
    public bool IsCollide(Box other)
    {
      return Left < other.Right && Right >= other.Left && Top < other.Bottom && Bottom >= other.Top;
    }

    // 座標(x, y)を足した新しい長方形を作る
    public static Box Add(Box box, float x, float y)
    {
      return new(box.Left + x, box.Right + x, box.Top + y, box.Bottom + y);
    }
  }
}
