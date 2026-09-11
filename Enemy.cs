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
    public Character c;

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
    public List<Cannon> cannonList = new();
    //public List<RotationCannon> rotationCannonList = new();

    public float MoveSpeedX;
    public float MoveSpeedY;
    public float MoveStartX;
    public float MoveStartY;
    public float Timer;
    public Character? Target;

    public static readonly int[] ScoreList = { 100, 100, 200, 500, 400, 200, 100, 1000, 1000, 10000 };

    public static readonly Rectangle[] rectGray= new Rectangle[] { new(0, 112, 32, 32), new(32, 112, 32, 32), new(64, 112, 32, 32), new(96, 112, 32, 32) };
    public static readonly Rectangle[] rectRed = new Rectangle[] { new(0, 144, 32, 32), new(32, 144, 32, 32), new(64, 144, 32, 32) };
    public static readonly Rectangle[] rectGreen = new Rectangle[] { new(0, 176, 32, 32), new(32, 176, 32, 32), new(64, 176, 32, 32) };
    public static readonly Rectangle[] rectBlue = new Rectangle[] { new(96, 176, 32, 32), new(128, 176, 32, 32), new(160, 176, 32, 32) };
    public static readonly Rectangle[] rect3Way = new Rectangle[] { new(0, 64, 32, 48) };
    public static readonly Rectangle[] rectMedium = new Rectangle[] { new(0, 0, 64, 64), new(64, 0, 64, 64) };
    public static readonly Rectangle[] rectAsteroid = new Rectangle[] { new(0, 208, 64, 64) };
    public static readonly Rectangle[] rectBoss1 = new Rectangle[] { new(256, 0, 256, 128) };

    public static readonly Box hitboxSmall = new(-24, 24, -24, 24);
    public static readonly Box hitbox3Way = new(-24, 32, -32, 32);
    public static readonly Box hitboxMedium = new(-48, 48, -48, 48);
    public static readonly Box hitboxBoss1 = new(-224, 224, -80, 64);

    // コンストラクタ
    public Enemy(float x, float y, int type, Character? target)
    {
      Type = type;
      Target = target;
      MoveStartX = x;
      MoveStartY = y;
      Timer = 0;

      float additionalInterval = (float)(Form1.rand.Next(5) - 2) * 0.05f;

      // 画像と衝突判定を設定
      switch (Type)
      {
      default:
      case 0: // 回転雑魚+直線(低速)
        c = new Character(x, y, 1, hitboxSmall, rectGray, 10);
        InitMoveStraight(0.0f, 300.0f);
        cannonList.Add(new(0, 0, 1, 0.0f, 2, 0.5f)); // 1Way
        break;

      case 1: // 回転雑魚+直線(高速)
        c = new Character(x, y, 1, hitboxSmall, rectGray, 10);
        InitMoveStraight(0.0f, 600.0f);
        cannonList.Add(new(0, 0, 1, 0.0f, 2, 0.5f + additionalInterval)); // 1Way
        break;

      case 2: // 青雑魚+追尾(低速)
        c = new Character(x, y, 1, hitboxSmall, rectBlue, 10);
        InitMoveHoming();
        cannonList.Add(new(0, 0, 1, 0.0f, 3, 1.0f + additionalInterval * 2)); // 1Way
        break;

      case 3: // 隕石
        c = new Character(x, y, 10, hitboxMedium, rectAsteroid, 10);
        InitMoveStraight(200.0f, 300.0f);
        break;

      // 3WAY雑魚
      case 4: // 3WAY雑魚+U字ターン(低速+上部)
        c = new Character(x, y, 4, hitbox3Way, rect3Way, 10);
        InitMoveUTurn(200.0f, 1.0f);
        cannonList.Add(new(0, 16, 3, 30.0f, 10, 1.8f)); // 3Way
        break;

      case 5: // 緑雑魚+U字ターン(高速+下部)
        c = new Character(x, y, 1, hitboxSmall, rectGreen, 10);
        InitMoveUTurn(600.0f, 0.0f);
        cannonList.Add(new(0, 0, 1, 0.0f, 1, 1.25f)); // 1Way
        break;

      case 6: // 赤雑魚+蛇行(高速)
        c = new Character(x, y, 1, hitboxSmall, rectRed, 10);
        InitMoveWinding(400, 200);
        cannonList.Add(new(0, 0, 1, 0.0f, 2, 0.5f + additionalInterval)); // 1Way
        break;

      case 7: // 中型雑魚+直線(低速)
        c = new Character(x, y, 10, hitboxMedium, rectMedium, 10);
        InitMoveStraight(0, 200);
        cannonList.Add(new(0, 0, 16, 360.0f, 3, 0.5f)); // 16Way
        Target = null;
        break;

      case 8: // 中型雑魚+蛇行(低速)
        c = new Character(x, y, 16, hitboxMedium, rectMedium, 10);
        InitMoveWinding(200, 50);
        for (int a = 0; a < 3; a++)
        {
          cannonList.Add(new(-32, 16, 1, 0.0f, 2, 2 - a * 0.1f)); // 1Way
          cannonList.Add(new( 32, 16, 1, 0.0f, 2, 2 - a * 0.1f)); // 1Way
        }
        break;

      case 9: // ボス１
        c = new Character(x, y, 200, hitboxBoss1, rectBoss1, 10);
        InitMoveBoss1(x);
        for (int a = 0; a < 3; a++)
        {
          cannonList.Add(new(-192 + a * 32, 64, 1, 0.0f, 2, 2 - a * 0.1f)); // 1Way
          cannonList.Add(new(192 - a * 32, 64, 1, 0.0f, 2, 2 - a * 0.1f)); // 1Way
        }
        //cannonList.Add(new(-80, -24, 5, 5, 10));
        //cannonList.Add(new(80, -24, 4, 5, 10));
        cannonList.Add(new(0, 0, 5, 90.0f, 2, 5));
        break;
      }
      c.AnimeIsLoop = true;
    }

    // 敵の状態を更新する
    public void Update(float deltaTime, List<NormalBullet> enemyBullets)
    {
      c.Update(deltaTime);

      switch (Type)
      {
      // 直線移動
      default:
      case 0:
      case 1:
      case 3:
      case 7:
        c.X += MoveSpeedX * deltaTime;
        c.Y += MoveSpeedY * deltaTime;
        break;

      // プレイヤーを追尾
      case 2:
        c.X += MoveSpeedX * deltaTime;
        c.Y += MoveSpeedY * deltaTime;
        if (Target != null)
        {
          float dx = Target.X - c.X;
          float dy = Target.Y - c.Y;
          float a = MathF.Sqrt(dx * dx + dy * dy);
          MoveSpeedX += 10.0f * (dx / a);
          MoveSpeedY += 10.0f * (dy / a);
          float b = 300.0f / MathF.Sqrt(MoveSpeedX * MoveSpeedX + MoveSpeedY * MoveSpeedY);
          MoveSpeedX *= b;
          MoveSpeedY *= b;
        }
        break;

      // U字ターン
      case 4:
      case 5:
        c.X += MoveSpeedX * deltaTime;
        c.Y += MoveSpeedY * deltaTime;
        Timer -= deltaTime;
        if (Timer <= 0.0f)
        {
          if (MoveStartY < Form1.nativeHeight * 0.5f)
          {
            MoveSpeedY -= 300 * deltaTime;
          }
          else
          {
            MoveSpeedY += 300 * deltaTime;
          }
        }
        break;

      // 蛇行
      case 6:
      case 8:
        if (Type == 6)
        {
          c.X = MoveStartX + MathF.Sin(c.Y * 0.01f) * MoveSpeedX;
        }
        else
        {
          c.X = MoveStartX + MathF.Sin(c.Y * 0.01f) * MoveSpeedX;
        }
        c.Y += MoveSpeedY * deltaTime;
        break;

      // ボス１
      case 9:
        c.X = MoveStartX + MathF.Sin(Timer) * 400.0f;
        c.Y += MoveSpeedY * deltaTime;
        if (c.Y > 240)
        {
          c.Y = 240;
          MoveSpeedY = 0;
        }
        if (MoveSpeedY == 0)
        {
          Timer += 0.0025f;
        }
        break;
      }

      for (int a = 0; a < cannonList.Count; a++)
      {
        cannonList[a].Update(deltaTime, c, enemyBullets, Target);
      }
    } // Updateメソッドブロックの終わり

    // 直線移動のデータを初期化する
    private void InitMoveStraight(float speedX, float speedY)
    {
      MoveSpeedX = (0.5f - c.X / Form1.nativeWidth) * (Form1.rand.Next(100) - 50) * speedX * 0.02f;
      MoveSpeedY = speedY;
      if (c.Y > Form1.nativeHeight / 2)
      {
        MoveSpeedY *= -1;
      }
    }

    // 追尾移動のデータを初期化する
    private void InitMoveHoming()
    {
      MoveSpeedX = 0;
      if (c.X < 0)
      {
        MoveSpeedX = 200;
      }
      else if (c.X >= Form1.nativeWidth)
      {
        MoveSpeedX = -200;
      }

      MoveSpeedY = 0;
      if (c.Y < 0)
      {
        MoveSpeedY = 200;
      }
      else if (c.Y >= Form1.nativeHeight)
      {
        MoveSpeedY = -200;
      }
    }

    // U字ターン移動のデータを初期化する
    private void InitMoveUTurn(float speed, float timer)
    {
      MoveSpeedX = 0;
      MoveSpeedY = speed;
      if (c.Y > Form1.nativeHeight / 2)
      {
        MoveSpeedY *= -1;
      }
      Timer = timer;
    }

    // 蛇行移動のデータを初期化する
    private void InitMoveWinding(float speedX, float speedY)
    {
      MoveSpeedX = speedX;
      MoveSpeedY = speedY;
      if (c.X >= Form1.nativeWidth / 2)
      {
        MoveSpeedX *= -1;
      }
    }

    // ボス１の移動データを初期化する
    private void InitMoveBoss1(float x)
    {
      MoveSpeedY = 100;
      MoveSpeedX = 0;
    }

  } // Enemyクラスブロックの終わり
}
