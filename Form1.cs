//#define RANDOM_SPAWN

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
//using SFML.Audio;

/*
  1. プロジェクトを新規作成し、ゲームループを作る
  2. 背景を表示する
  3. 背景をスクロールさせる
  4. 敵を表示する
  5. Characterクラスを作る
  6. 敵をアニメーションさせる

  7. Enemyクラスを作る
  8. 直線的な動きを作る
  9. EnemySpawnerクラスを作る
 10. 敵を複数出現させる
 11. 敵出現テーブルを作る
 12. 蛇行する動きを作る

 13. Playerクラスを作る
 14. 自機を動かす
 15. Normalbulletクラスを作る
 16. 自機から弾を出す
 17. パワーアップアイテムを出す
 18. 自機をパワーアップさせる

 19. Boxクラスを作る
 20. 敵を破壊する
 21. 得点を表示する
 22. Cannonクラスを作る
 23. 敵からまっすぐ弾を出す
 24. 敵から自機を狙う弾を出す

 25. 自機を破壊する(ゲームオーバー)
 26. ボスを出す
 27. ボスの動きを作る
 28. ボスを破壊する(ステージクリア)
*/
namespace ShootingGameCS
{
  public partial class Form1 : Form
  {
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int key);

    // GetAsyncKeyStateで使う仮想キー番号
    public const int vkReturn = 13;    // Enterキー
    public const int vkEscape = 27;    // ESCキー
    public const int vkSpace = 32;     // スペースキー
    public const int vkLeft = 37;      // 矢印キー(左)
    public const int vkUp = 38;        // 矢印キー(上)
    public const int vkRight = 39;     // 矢印キー(右)
    public const int vkDown = 40;      // 矢印キー(下)
    public const int vkLShift = 160;   // 左shiftキー
    public const int vkLControl = 162; // 左ctrlキー
    public const int vkLMenu = 164;    // 左altキー

    public static Random rand = new(); // 乱数

    // 画面の大きさ
    public const int nativeWidth = 1280;
    public const int nativeHeight = 960;

    // ゲーム状態
    const int gsTitle = 0;
    const int gsPlay = 1;
    const int gsClear = 2;
    const int gsGameOver = 3;
    private int gameState = gsTitle;

    const int maxStageNo = 2;
    private int stageNo = 0;     // ステージ番号
    private int score = 0;       // 得点
    private float deadTimer = 0; // 死亡後の時間を計るタイマー
    private bool oldEnterKeyState = false; // 前回のEnterキーの状態

    // プレイヤーの変数
    private Player player = new();
    private List<Character> itemList = new();

    // 敵の変数
    private List<Enemy> enemies = new();
    private List<Enemy> bossList = new();
    private List<NormalBullet> enemyBullets = new();
#if RANDOM_SPAWN
    private float enemySpawnTimer = 0;
#endif

    private List<Character> effectList = new();

    // 背景の変数
    private int[,] backgroundTiles = new int[15 * 8, 20];
    private float backgroundY = 15 * 7 * 64;

    // フォントの変数
    private Font fontTitle = null!;
    private Font fontScore = null!;

    // 画像の変数
    private Bitmap bmpTitleBack = new("assets/images/bg_planet.png");
    private Bitmap bmpCharacter = new("assets/images/objects.png");
    private Bitmap bmpBackground = new("assets/images/bg0.png");
    private Bitmap bmpBackground2 = new("assets/images/bg_space_long.png");

    // エフェクトアニメーション
    private static Rectangle[] rectBlast = {
      new(848, 384, 16, 16), new(864, 368, 32, 32), new(896, 368, 32, 32),
      new(928, 368, 32, 32), new(960, 368, 32, 32), new(992, 368, 32, 32) };

    private static Rectangle[] rectHitEffect = {
      new(32, 464, 16, 16), new(48, 464, 16, 16), new(64, 464, 16, 16), new(80, 464, 16, 16),
    };

    // 背景データ
    private static readonly int[][,] tileList = new int[maxStageNo][,] {
      new int[,]{ { 0, 224, 30 }, { 32, 224, 20 }, { 64, 224, 20 }, { 96, 224, 20 }, { 128, 224, 10 }, { 160, 224, 2 }, { 192, 224, 3 } , { 224, 224, 1 } },
      new int[,]{ { 128, 0, 0 }, { 448, 64, 0 }, { 128, 64, 0 }, { 288, 32, 50 }, { 0, 320, 50 }, { 32, 320, 50 }, { 288, 0, 5 }, { 288, 64, 2 }, { 320, 0, 5 }, { 320, 32, 1 }, { 320, 64, 1 }, },
    };

    // 敵出現データ
    public class SpawnerData
    {
      public int Type;
      public float X;

      public SpawnerData(int type, float x)
      {
        Type = type;
        X = x;
      }
    }
    public class SpawnerList
    {
      public int Line; // スポーンさせる背景Y座標
      public SpawnerData[] Data; // スポ―ナ―の配列

      public SpawnerList(int line, int t0, float x0, int t1, float x1, int t2, float x2, int t3, float x3)
      {
        Line = line * 64;
        Data = new SpawnerData[] { new(t0, x0), new(t1, x1), new(t2, x2), new(t3, x3) };
      }
    }
    //   Y, [種類, X]x4
    private static readonly SpawnerList[] spawnerDataList = new SpawnerList[] {
      new( 18, 1, 15, 0, 0, 0, 0, 0, 0),
      new( 24, 1,  5, 0, 0, 0, 0, 0, 0),
      new( 30, 2,  5, 0, 0, 0, 0, 0, 0),
      new( 40, 2, 15, 0, 0, 0, 0, 0, 0),
      new( 50, 3, 10, 0, 0, 0, 0, 0, 0),
      new( 60, 4, 10, 0, 0, 0, 0, 0, 0),
      new( 70, 5, 13, 0, 0, 0, 0, 0, 0),
      new( 80, 6,  5, 0, 0, 0, 0, 0, 0),
      new( 90, 6, 15, 0, 0, 0, 0, 0, 0),
      new(100, 7,  5, 0, 0, 0, 0, 0, 0),
      new(110, 7, 15, 0, 0, 0, 0, 0, 0),
      new(119,10, 10, 0, 0, 0, 0, 0, 0),
    };
    private List<EnemySpawner> enemySpawnerList = new();
    private int spawnListIndex = 0;

    // コンストラクタ
    public Form1()
    {
      InitializeComponent();
      ClientSize = new Size(nativeWidth, nativeHeight);
    }

    // ゲーム状態を初期化する
    public void Initialize()
    {
      float dpiScale = 96.0f / this.DeviceDpi;
      fontTitle = new("Impact", 96.0f * dpiScale);
      fontScore = new("Sitka", 24.0f * dpiScale);

      // 画像を標準形式に変換
      // これをやらないと描画がかなり遅くなる
      bmpCharacter = bmpCharacter.Clone(
        new Rectangle(0, 0, bmpCharacter.Width, bmpCharacter.Height), PixelFormat.Format32bppPArgb);
      bmpBackground = bmpBackground.Clone(
        new Rectangle(0, 0, bmpBackground.Width, bmpBackground.Height), PixelFormat.Format32bppPArgb);
      bmpBackground2 = bmpBackground2.Clone(
        new Rectangle(0, 0, bmpBackground2.Width, bmpBackground2.Height), PixelFormat.Format32bppPArgb);
    }

    // ステージデータを初期化する
    private void InitStage()
    {
      player.Init();
      itemList.Clear();
      enemies.Clear();
      bossList.Clear();
      enemyBullets.Clear();

      enemySpawnerList.Clear();
      spawnListIndex = 0;

      effectList.Clear();

      backgroundY = 15 * 7 * 64;

      // 背景データを作成
      int tileCount = tileList[stageNo].GetLength(0);
      int totalTiles = 0;
      for (int a = 0; a < tileCount; a++)
      {
        totalTiles += tileList[stageNo][a, 2];
      }
      Random r = new(12345678);
      for (int y = 0; y < 15 * 8; y++)
      {
        for (int x = 0; x < 20; x++)
        {
          int id = 0;
          int p = r.Next(totalTiles);
          for (int a = 0; a < tileCount; a++)
          {
            if (p < tileList[stageNo][a, 2])
            {
              id = a;
              break;
            }
            p -= tileList[stageNo][a, 2];
          }
          backgroundTiles[y, x] = id;
        }
      }
      if (stageNo == 1)
      {
        int[] roadOffsetIndices = { 0, 1, 2, 3, 4, 5, 6 };
        Random.Shared.Shuffle(roadOffsetIndices);
        for (int a = 0; a < 4; a++)
        {
          int y = roadOffsetIndices[a] * 15 + r.Next(10);
          for (int x = 0; x < 20; x++)
          {
            backgroundTiles[y, x] = 0;
            backgroundTiles[y + 1, x] = 1;
            backgroundTiles[y + 2, x] = 2;
          }
        }
      }
    }

    // ゲーム状態を更新する
    public void Update(float deltaTime)
    {
      if (!Focused)
      {
        return;
      }

      switch (gameState)
      {
      case gsTitle: UpdateTitle(deltaTime); break;
      case gsPlay: UpdatePlay(deltaTime); break;
      case gsClear: UpdateClear(deltaTime); break;
      case gsGameOver: UpdateGameOver(deltaTime); break;
      }
    }

    // 画面を描く
    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      Graphics g = e.Graphics;
      switch (gameState)
      {
      case gsTitle: PaintTitle(g); break;
      case gsPlay: PaintPlay(g); break;
      case gsClear: PaintClear(g); break;
      case gsGameOver: PaintGameOver(g); break;
      }
    }

    // タイトル状態を更新する
    private void UpdateTitle(float deltaTime)
    {
      bool enterKeyState = GetAsyncKeyState(vkReturn) < 0;
      if (!oldEnterKeyState && enterKeyState)
      {
        stageNo = 0;
        InitStage();
        score = 0;
        gameState = gsPlay;
      }
      oldEnterKeyState = enterKeyState;
    }

    // タイトル画面を描く
    private void PaintTitle(Graphics g)
    {
      g.DrawImage(bmpTitleBack, 0, 0, nativeWidth, nativeHeight);
      TextRenderer.DrawText(g, "Sci-Fi Shooter", fontTitle, new Point(300, 300), Color.White);
      TextRenderer.DrawText(g, "Push Enter", fontScore, new Point(600, 600), Color.LightGreen);
    }

    // ステージクリア状態を更新する
    private void UpdateClear(float deltaTime)
    {
      bool enterKeyState = GetAsyncKeyState(vkReturn) < 0;
      if (!oldEnterKeyState && enterKeyState)
      {
        stageNo++;
        if (stageNo < maxStageNo)
        {
          InitStage();
          gameState = gsPlay;
        }
        else
        {
          gameState = gsTitle;
        }
      }
      oldEnterKeyState = enterKeyState;
    }

    // ステージクリア画面を描く
    private void PaintClear(Graphics g)
    {
      PaintPlay(g);
      TextRenderer.DrawText(g, "Stage Clear!", fontTitle, new Point(300, 400), Color.White);
    }

    // ゲームオーバー状態を更新する
    private void UpdateGameOver(float deltaTime)
    {
      bool enterKeyState = GetAsyncKeyState(vkReturn) < 0;
      if (!oldEnterKeyState && enterKeyState)
      {
        gameState = gsTitle;
      }
      oldEnterKeyState = enterKeyState;
    }

    // ゲームオーバー画面を描く
    private void PaintGameOver(Graphics g)
    {
      PaintPlay(g);
      TextRenderer.DrawText(g, "GAME OVER", fontTitle, new Point(350, 400), Color.Red);
    }

    // プレイ状態を更新する
    private void UpdatePlay(float deltaTime)
    {
      // プレイヤーの操作
      player.Update(deltaTime);

#if !RANDOM_SPAWN
      // 敵スポナーの発生
      if (spawnListIndex < spawnerDataList.Length && 960 * 8 - spawnerDataList[spawnListIndex].Line >= backgroundY)
      {
        for (int a = 0; a < 4; a += 1)
        {
          int type = spawnerDataList[spawnListIndex].Data[a].Type;
          if (type > 0)
          {
            float x = spawnerDataList[spawnListIndex].Data[a].X;
            enemySpawnerList.Add(new(type, x * 64));
          }
        }
        spawnListIndex++;
      }

      // 敵の出現
      for (int a = 0; a < enemySpawnerList.Count; a += 1)
      {
        enemySpawnerList[a].Update(deltaTime, enemies, bossList, player.c);
      }
      enemySpawnerList.RemoveAll(spawner => spawner.Count <= 0);
#else
      if (backgroundY > 720)
      {
        enemySpawnTimer += deltaTime;
        if (enemySpawnTimer >= 1)
        {
          enemySpawnTimer -= 1;
          if (enemies.Count < 10)
          {
            int type = rand.Next(9);
            enemies.Add(new Enemy(rand.Next(18) * 64 + 64, -64, type, player.c));
          }
        }
      }
#endif

      // 敵の更新
      for (int a = 0; a < enemies.Count; a++)
      {
        enemies[a].Update(deltaTime, enemyBullets);
      }
      enemies.RemoveAll(enemy =>
        enemy.c.Y >= nativeHeight + (enemy.c.GetHeight() + 64) || enemy.c.Y < -(enemy.c.GetHeight() + 64));

      // 敵弾の更新
      for (int a = 0; a < enemyBullets.Count; a++)
      {
        enemyBullets[a].Update(deltaTime);
      }
      enemyBullets.RemoveAll(NormalBullet.IsOutsideWindow);

      // 背景の移動
      if (backgroundY > 0)
      {
        backgroundY -= 1;
      }

#if RANDOM_SPAWN
      if (backgroundY == 1)
      {
        Enemy boss = new(nativeWidth / 2, -192, 9, player.c);
        enemies.Add(boss);
        bossList.Add(boss);
      }
#endif

      // アイテムの出現
      if ((int)backgroundY % 1200 == 100)
      {
        Rectangle[] rectItem = { new(320, 480, 32, 32) };
        Box hitboxItem = new(-32, 32, -32, 32);
        itemList.Add(new((rand.Next(11) + 2) * 64, -32, 1, hitboxItem, rectItem, 10));
      }

      // アイテムの移動と消去
      for (int a = 0; a < itemList.Count; a += 1)
      {
        itemList[a].Y += 150 * deltaTime;
      }
      itemList.RemoveAll(item => item.Y >= nativeHeight + 32);

      // プレイヤーの弾と敵の衝突判定
      for (int a = 0; a < player.Bullets.Count; a++)
      {
        NormalBullet bullet = player.Bullets[a];
        Box boxA = Box.Add(bullet.c.Hitbox, bullet.c.X, bullet.c.Y);
        for (int b = 0; b < enemies.Count; b++)
        {
          Enemy e = enemies[b];
          Box boxB = Box.Add(e.c.Hitbox, e.c.X, e.c.Y);
          if (boxA.IsCollide(boxB))
          {
            e.c.Hp -= 1;
            if (e.c.Hp <= 0)
            {
              // 爆発を表示
              effectList.Add(new(e.c.X, e.c.Y, 1, Box.Empty, rectBlast, 15));

              // 得点を増やす
              if (e.Type >= 0 && e.Type < Enemy.ScoreList.Length)
              {
                score += Enemy.ScoreList[e.Type];
              }

              // 弾と敵を消去
              enemies.RemoveAt(b);
            }
            else
            {
              // ヒットエフェクトを表示
              Character s = new(bullet.c.X, bullet.c.Y - bullet.c.GetHeight() / 2,
                1, Box.Empty, rectHitEffect, 30);
              s.Priority = 1;
              effectList.Add(s);

              // 得点を増やす
              score += 10;
            }
            player.Bullets.RemoveAt(a);
            a--;
            break;
          }
        }
      }

      // 死んでいなければ、プレイヤーの衝突判定を実行
      // 死んでいたら、一定時間後にゲームオーバー状態にする
      if (player.IsDead)
      {
        deadTimer -= deltaTime;
        if (deadTimer <= 0)
        {
          gameState = gsGameOver;
        }
      }
      else
      {
        // 敵の弾とプレイヤーの衝突判定
        Box boxPlayer = Box.Add(player.c.Hitbox, player.c.X, player.c.Y);
        for (int a = 0; a < enemyBullets.Count; a++)
        {
          NormalBullet b = enemyBullets[a];
          Box boxBullet = Box.Add(b.c.Hitbox, b.c.X, b.c.Y);
          if (boxBullet.IsCollide(boxPlayer))
          {
            effectList.Add(new(player.c.X, player.c.Y, 1, Box.Empty, rectBlast, 6));
            player.IsDead = true;
            deadTimer = 2;
            break;
          }
        }

        // 敵とプレイヤーの衝突判定
        for (int a = 0; a < enemies.Count; a++)
        {
          Enemy e = enemies[a];
          Box boxEnemy = Box.Add(e.c.Hitbox, e.c.X, e.c.Y);
          if (boxEnemy.IsCollide(boxPlayer))
          {
            effectList.Add(new(player.c.X, player.c.Y, 1, Box.Empty, rectBlast, 6));
            player.IsDead = true;
            deadTimer = 2;
            break;
          }
        }

        // アイテムとプレイヤーの衝突判定
        for (int a = 0; a < itemList.Count; a += 1)
        {
          Box boxItem = Box.Add(itemList[a].Hitbox, itemList[a].X, itemList[a].Y);
          if (boxItem.IsCollide(boxPlayer))
          {
            itemList[a].Y = nativeHeight + 100;
            player.ShotLevel += 1;
            if (player.ShotLevel > 5)
            {
              player.ShotLevel = 5;
            }
          }
        }
      } // プレイヤーの衝突判定の終わり

      // 爆発・ヒットエフェクトの更新
      effectList.Sort((a, b) => (int)(a.Priority - b.Priority));
      for (int a = 0; a < effectList.Count; a++)
      {
        effectList[a].Update(deltaTime);
      }
      effectList.RemoveAll(character => character.AnimeIsEnd);

      // ステージクリア判定
      if (!player.IsDead && bossList.Count > 0)
      {
        bossList.RemoveAll(boss => boss.c.Hp <= 0);
        if (bossList.Count == 0)
        {
          gameState = gsClear;
        }
      }
    } // Updateメソッドブロックの終わり

    // プレイ画面を描く
    private void PaintPlay(Graphics g)
    {
      {
        //float sx = (float)ClientSize.Width / nativeWidth;
        //float sy = (float)ClientSize.Height / nativeHeight;
        //g.ScaleTransform(sx, sy);

        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode = PixelOffsetMode.Half;
      }

      g.Clear(Color.DarkKhaki);

#if true
      g.CompositingMode = CompositingMode.SourceCopy;
      int minY = (int)(backgroundY / 64);
      if (minY < 0)
      {
        minY = 0;
      }

      int maxY = minY + 16;
      if (maxY >= backgroundTiles.GetLength(0))
      {
        maxY = backgroundTiles.GetLength(0);
      }

      Rectangle srcTile = new(0, 0, 32, 32);
      Rectangle dstTile = new(0, minY * 64 - (int)backgroundY, 64, 64);
      for (int y = minY; y < maxY; y++)
      {
        for (int x = 0; x < 20; x++)
        {
          int id = backgroundTiles[y, x];
          srcTile.X = tileList[stageNo][id, 0];
          srcTile.Y = tileList[stageNo][id, 1];
          dstTile.X = x * 64;
          g.DrawImage(bmpBackground, dstTile, srcTile, GraphicsUnit.Pixel);
        }
        dstTile.Y += 64;
      }
      g.CompositingMode = CompositingMode.SourceOver;
#else
      {
        g.CompositingMode = CompositingMode.SourceCopy;
        g.DrawImage(bmpBackground2, 0, -backgroundY * 3 / 7, 1280, 3840);
        g.CompositingMode = CompositingMode.SourceOver;
      }
#endif

      for (int a = 0; a < enemies.Count; a++)
      {
        enemies[a].c.Draw(g, bmpCharacter);
      }

      for (int a = 0; a < itemList.Count; a++)
      {
        itemList[a].Draw(g, bmpCharacter);
      }

      for (int a = 0; a < player.Bullets.Count; a++)
      {
        player.Bullets[a].c.Draw(g, bmpCharacter);
      }

      for (int a = 0; a < enemyBullets.Count; a++)
      {
        enemyBullets[a].c.Draw(g, bmpCharacter);
      }

      for (int a = 0; a < effectList.Count; a++)
      {
        effectList[a].Draw(g, bmpCharacter);
      }

      if (!player.IsDead)
      {
        player.c.Draw(g, bmpCharacter);
        player.engine.Draw(g, bmpCharacter);
      }

      // 得点を表示
      TextRenderer.DrawText(g, score.ToString(), fontScore, new Point(400, 40), Color.White);

    } // OnPaintメソッドブロックの終わり
  }
}
