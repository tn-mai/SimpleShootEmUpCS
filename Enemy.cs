using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  // 敵
  internal class Enemy
  {
    public Sprite Sprite;
    public Box Hitbox;
    public List<Cannon> cannonList = new();

    // 0=回転雑魚+直線(低速)
    // 1=回転雑魚+直線(高速)
    // 2=青雑魚+追尾(低速)
    // 3=隕石
    // 4=3WAY雑魚+U字ターン(低速+上部)
    // 5=緑雑魚+U字ターン(高速+下部)
    // 6=赤雑魚+蛇行(高速)
    // 7=中型雑魚+直線(低速)
    // 8=中型雑魚+蛇行(低速)
    public int Type;

    public float Hp;
    public Sprite? TargetSprite;
    public float MoveSpeedX;
    public float MoveSpeedY;
    public float MoveDirection;
    public float MoveStartX;
    public float Timer;

    public static readonly int[] ScoreList = { 100, 100, 200, 500, 400, 200, 100, 1000, 1000, 10000 };

    // コンストラクタ
    public Enemy(float x, float y, int type, Sprite? target)
    {
      Type = type;
      TargetSprite = target;
      MoveStartX = x;
      Timer = 0;

      // 画像と衝突判定を設定
      switch (Type)
      {
      // 回転雑魚
      default:
      case 0:
      case 1:
        Sprite = new(x, y, new(0, 112, 32, 32));
        Hitbox = new(-24, 24, -24, 24);
        Hp = 1;
        break;

      // 青雑魚
      case 2:
        Sprite = new(x, y, new(96, 176, 32, 32));
        Hitbox = new(-24, 24, -24, 24);
        Hp = 1;
        break;

      // 緑雑魚
      case 5:
        Sprite = new(x, y, new(0, 176, 32, 32));
        Hitbox = new(-24, 24, -24, 24);
        Hp = 1;
        break;

      // 隕石
      case 3:
        Sprite = new(x, y, new(0, 208, 64, 64));
        Hitbox = new(-48, 48, -48, 48);
        Hp = 10;
        break;

      // 赤雑魚
      case 6:
        Sprite = new(x, y, new(0, 144, 32, 32));
        Hitbox = new(-24, 24, -24, 24);
        Hp = 1;
        break;

      // 3WAY雑魚
      case 4:
        Sprite = new(x, y, new(0, 64, 32, 48));
        Hitbox = new(-24, 24, -32, 32);
        Hp = 4;
        break;

      // 中型雑魚
      case 7:
        Sprite = new(x, y, new(0, 0, 64, 64));
        Hitbox = new(-48, 48, -48, 48);
        Hp = 10;
        break;

      case 8:
        Sprite = new(x, y, new(0, 0, 64, 64));
        Hitbox = new(-48, 48, -48, 48);
        Hp = 16;
        break;

      case 9:
        Sprite = new(x, y, new(256, 0, 256, 128));
        Hitbox = new(-224, 224, -80, 64);
        Hp = 200;
        break;
      }

      // 移動状態を設定
      switch (Type)
      {
      // 直線移動
      default:
      case 0: // 低速
      case 1: // 高速
      case 7: // 低速
        MoveSpeedX = 0;
        MoveSpeedY = 200;
        MoveDirection = MathF.PI * 0.5f;
        if (Type == 1)
        {
          MoveSpeedY = 600;
        }
        if (Sprite.Y > Form1.nativeHeight / 2)
        {
          MoveSpeedY *= -1;
          MoveDirection *= -1;
        }
        break;

      // 目標を追尾
      case 2:
        if (TargetSprite == null)
        {
          MoveSpeedX = 0;
          MoveSpeedY = 300;
          MoveDirection = MathF.PI * 0.5f;
          if (Sprite.Y > Form1.nativeHeight / 2)
          {
            MoveSpeedY *= -1;
            MoveDirection *= -1;
          }
        }
        else
        {
          float dx = TargetSprite.X - Sprite.X;
          float dy = TargetSprite.Y - Sprite.Y;
          MoveDirection = MathF.Atan2(dy, dx);
          if (Type == 2)
          {
            MoveSpeedX = 300 * MathF.Sin(MoveDirection);
            MoveSpeedY = 300 * MathF.Cos(MoveDirection);
          }
          else
          {
            MoveSpeedX = 600 * MathF.Sin(MoveDirection);
            MoveSpeedY = 600 * MathF.Cos(MoveDirection);
          }
        }
        break;

      // 隕石
      case 3:
        MoveSpeedX = (0.5f - Sprite.X / Form1.nativeWidth) * 200 + Form1.rand.Next(100) - 50;
        MoveSpeedY = 300;
        MoveDirection = -0.5f * MathF.PI;
        break;

      // U字ターン
      case 4: // 低速
      case 5: // 高速
        MoveSpeedX = 0;
        MoveSpeedY = 400;
        if (Type == 5)
        {
          MoveSpeedY = 600;
        }
        MoveDirection = -MathF.PI / 2;
        if (Sprite.Y > Form1.nativeHeight / 2)
        {
          MoveSpeedY *= -1;
          MoveDirection *= -1;
        }
        break;

      // 蛇行
      case 6: // 高速
      case 8: // 低速
        MoveSpeedY = 50;
        if (Type == 6)
        {
          MoveSpeedX = 400;
          MoveSpeedY = 200;
        }
        MoveDirection = 0;
        if (Sprite.X >= Form1.nativeWidth / 2)
        {
          MoveDirection = MathF.PI;
          MoveSpeedX *= -1;
        }
        MoveSpeedX = 0;
        break;

      // ボス１
      case 9:
        MoveSpeedY = 100;
        MoveSpeedX = 0;
        MoveDirection = MathF.PI * -0.5f;
        MoveStartX = x;
        break;
      }

      switch (Type)
      {
      default:
      case 3: // 隕石
        break;
      case 0: // 回転雑魚+直線(低速)
      case 1: // 回転雑魚+直線(高速)
      case 2: // 青雑魚+追尾(低速)
        cannonList.Add(new(0, 0, 0, 2, 0.5f)); // 1Way
        break;
      case 4: // 3WAY雑魚+U字ターン(低速+上部)
        cannonList.Add(new(0, 16, 1, 4, 1.25f)); // 3Way
        break;
      case 5: // 緑雑魚+U字ターン(高速+下部)
        cannonList.Add(new(0, 0, 0, 1, 1)); // 1Way
        break;
      case 6: // 赤雑魚+蛇行(高速)
        cannonList.Add(new(0, 0, 0, 2, 0.5f)); // 1Way
        break;
      case 7: // 中型雑魚+直線(低速)
        cannonList.Add(new(0, 0, 3, 3, 0.5f)); // 16Way
        break;
      case 8: // 中型雑魚+蛇行(低速)
        for (int a = 0; a < 3; a++)
        {
          cannonList.Add(new(-32, 16, 0, 2, 2 - a * 0.1f)); // 1Way
          cannonList.Add(new(32, 16, 0, 2, 2 - a * 0.1f)); // 1Way
        }
        break;
      case 9: // ボス１
        for (int a = 0; a < 3; a++)
        {
          cannonList.Add(new(-192 + a * 32, 64, 0, 2, 2 - a * 0.1f)); // 1Way
          cannonList.Add(new(192 - a * 32, 64, 0, 2, 2 - a * 0.1f)); // 1Way
        }
        cannonList.Add(new(-80, -24, 5, 5, 10));
        cannonList.Add(new(80, -24, 4, 5, 10));
        cannonList.Add(new(0, 0, 2, 2, 5));
        break;
      }
    }

    // 敵の状態を更新する
    public void Update(float deltaTime, List<NormalBullet> enemyBullets)
    {
      Sprite.X += MoveSpeedX * deltaTime;
      Sprite.Y += MoveSpeedY * deltaTime;

      switch (Type)
      {
      // 直線移動
      default:
      case 0:
      case 1:
      case 3:
      case 7:
        break;

      // プレイヤーを追尾
      case 2:
        if (TargetSprite != null)
        {
          float dx = TargetSprite.X - Sprite.X;
          float dy = TargetSprite.Y - Sprite.Y;
          float s = dx * MoveSpeedY - dy * MoveSpeedX;
          if (s >= 0)
          {
            MoveDirection -= 1.5f * deltaTime;
            if (MoveDirection < -MathF.PI)
            {
              MoveDirection = MathF.PI * 2 - MoveDirection;
            }
          }
          else
          {
            MoveDirection += 1.5f * deltaTime;
            if (MoveDirection > MathF.PI)
            {
              MoveDirection = -(MathF.PI * 2 - MoveDirection);
            }
          }
          MoveSpeedX = 300 * MathF.Cos(MoveDirection);
          MoveSpeedY = 300 * MathF.Sin(MoveDirection);
        }
        break;

      // U字ターン
      case 4:
      case 5:
        MoveSpeedY += 300 * MathF.Sin(MoveDirection) * deltaTime;
        break;

      // 蛇行
      case 6:
      case 8:
        if (Type == 6)
        {
          Sprite.X = MoveStartX + MathF.Sin(Sprite.Y * 0.01f) * 400.0f * MathF.Cos(MoveDirection);
        }
        else
        {
          Sprite.X = MoveStartX + MathF.Sin(Sprite.Y * 0.01f) * 200.0f * MathF.Cos(MoveDirection);
        }
        break;

      // ボス１
      case 9:
        if (Sprite.Y > 240)
        {
          Sprite.Y = 240;
          MoveSpeedY = 0;
        }
        if (MoveSpeedY == 0)
        {
          Timer += 0.0025f;
        }
        Sprite.X = MoveStartX + MathF.Sin(Timer) * 400.0f;
        break;
      }

      for (int a = 0; a < cannonList.Count; a++)
      {
        cannonList[a].Update(deltaTime, this, enemyBullets, TargetSprite);
      }
    } // Updateメソッドブロックの終わり
  } // Enemyクラスブロックの終わり
}
