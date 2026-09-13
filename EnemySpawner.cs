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
  //   1=直進緑雑魚x4(出現位置ランダム、出現タイミングややランダム、上部から対角線方向(ややランダム)に直進)
  //   2=直進緑雑魚x8(同時に8機が水平に出現(ランダムなし)、真下へ直進)
  //   3=蛇行灰雑魚x8(0.25秒間隔で同じ位置に出現、ゆっくり目に舌へ移動しつつ左右に大きく蛇行)
  //   4=自機狙い青雑魚x8(画面上と左右にランダムな位置にランダムなタイミングで出現)
  //   5=Uターン赤雑魚x4(0.25秒間隔で同じ位置に出現)
  //   6=3Way雑魚x2
  //   7=直進中型雑魚x1
  //   8=蛇行中型雑魚x1
  //   9=横から追尾青雑魚x4
  //  10=下からUターン緑雑魚x4
  //  11=ボス１
  internal class EnemySpawner
  {
    public int Type;    // 出現させる編隊の種類
    public float BaseX; // 出現位置のX座標
    public int Count;   // 出現済みの敵の数
    public float Timer; // 出現間隔タイマー

    private class TypeData
    {
      public int EnemyType;      // 敵の種類
      public float Interval;     // 出現間隔(秒)
      public float[] OffsetList; // 出現位置の補正量(ドット)

      public TypeData(int enemyType, float interval, float[] offsetList)
      {
        EnemyType = enemyType;
        Interval = interval;
        OffsetList = offsetList;
      }
    }
    private static readonly TypeData[] typeDataList = {
      new(0, 0.0f, new float[]{ }),
      new(0, 0.3f, new float[]{ 0, -64, 64, -128, 128 }),
      new(6, 0.25f, new float[] { 0, 0, 0, 0, 0, 0, 0, 0 }),
      new(2, 0.5f, new float[] { 0, 256, -256, 128 }),
      new(5, 1.0f, new float[] { 0, -384, 256, -128 }),
      new(4, 2.0f, new float[] { 0, 384 }),
      new(7, 0.0f, new float[] { 0 }),
      new(8, 0.0f, new float[] { 0 }),
      new(2, 0.5f, new float[] { 320, 192, 480, 256 }),
      new(5, 1.0f, new float[] { 0, 192, -128, 256 }),
      new(9, 0.0f, new float[] { 0 }),
    };

    // コンストラクタ
    public EnemySpawner(int type, float x)
    {
      Type = type;
      BaseX = x;
      Count = 0;
      Timer = typeDataList[Type].Interval;
    }

    // 所定時間ごとに敵を発生させる
    public void Update(float deltaTime, List<Enemy> enemies, List<Enemy> bossList, Character target)
    {
      // 発生数が配列の長さを超えたらそれ以上は発生させない
      if (Count >= typeDataList[Type].OffsetList.Length)
      {
        return;
      }

      // タイマーが発生間隔未満なら発生させない
      Timer += deltaTime;
      if (Timer < typeDataList[Type].Interval)
      {
        return;
      }

      float offset = typeDataList[Type].OffsetList[Count];
      float x = BaseX + offset;
      float y = -64;
      if (BaseX >= Form1.nativeWidth * 0.5f)
      {
        x = BaseX - offset;
      }

      if (Type == 8)
      {
        x = BaseX;
        y = offset;
      }
      else if (Type == 9)
      {
        y = Form1.nativeHeight;
      }
      Enemy e = new(x, y, typeDataList[Type].EnemyType, target);
      enemies.Add(e);
      if (typeDataList[Type].EnemyType == 9)
      {
        bossList.Add(e);
      }
      Count++;
      Timer -= typeDataList[Type].Interval;
    }
  } // EnemySpawnerクラスブロックの終わり
}
