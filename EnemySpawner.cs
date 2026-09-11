using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootingGameCS
{
  // 敵編隊を発生させるクラス
  //
  // 編隊の種類
  //   0=なし
  //   1=直進灰色雑魚x5
  //   2=蛇行赤雑魚x8
  //   3=上から追尾青雑魚x4
  //   4=Uターン緑雑魚x4
  //   5=3Way雑魚x2
  //   6=直進中型雑魚x1
  //   7=蛇行中型雑魚x1
  //   8=横から追尾青雑魚x4
  //   9=下からUターン緑雑魚x4
  //  10=ボス１
  internal class EnemySpawner
  {
    public int Type;    // 出現させる編隊の種類
    public int BaseX;   // 出現位置のX座標
    public int Count;   // 出現済みの敵の数
    public float Timer; // 出現間隔タイマー

    private static readonly int[] enemyTypeList = { 0, 0, 6, 2, 5, 4, 7, 8, 2, 5, 9 };
    private static readonly float[] intervalList = { 0.0f, 0.3f, 0.25f, 0.5f, 1.0f, 2.0f, 0.0f, 0.0f, 0.5f, 1.0f, 0.0f };
    private static readonly float[][] offsetList = {
        new float[] { },
        new float[] { 0, -64, 64, -128, 128 },
        new float[] { 0, 0, 0, 0, 0, 0, 0, 0 },
        new float[] { 0, 256, -256, 128 },
        new float[] { 0, -384, 256, -128 },
        new float[] { 0, 384 },
        new float[] { 0 },
        new float[] { 0 },
        new float[] { 320, 192, 480, 256 },
        new float[] { 0, 192, -128, 256 },
        new float[] { 0 },
      };

    // コンストラクタ
    public EnemySpawner(int type, int x)
    {
      Type = type;
      BaseX = x;
      Count = 0;
      Timer = intervalList[Type];
    }

    // 所定時間ごとに敵を発生させる
    public void Update(float deltaTime, List<Enemy> enemies, List<Enemy> bossList, Character target)
    {
      // 発生数が配列の長さを超えたらそれ以上は発生させない
      if (Count >= offsetList[Type].Length)
      {
        return;
      }

      // タイマーが発生間隔未満なら発生させない
      Timer += deltaTime;
      if (Timer < intervalList[Type])
      {
        return;
      }

      float x = BaseX + offsetList[Type][Count];
      float y = -64;
      if (BaseX >= Form1.nativeWidth * 0.5f)
      {
        x = BaseX - offsetList[Type][Count];
      }

      if (Type == 8)
      {
        x = BaseX;
        y = offsetList[Type][Count];
      }
      else if (Type == 9)
      {
        y = Form1.nativeHeight;
      }
      Enemy e = new(x, y, enemyTypeList[Type], target);
      enemies.Add(e);
      if (Type == 9)
      {
        bossList.Add(e);
      }
      Count++;
      Timer -= intervalList[Type];
    }
  } // EnemySpawnerクラスブロックの終わり
}
