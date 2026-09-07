using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
//using SFML.Audio;

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

    // 敵の変数
    private List<Enemy> enemies = new();
    private List<NormalBullet> enemyBullets= new();
    private float enemySpawnTimer = 0;
    private List<Enemy> bossList = new();

    private List<AnimatedSprite> animatedSpriteList = new();

    // 背景の変数
    private Sprite[,] backgroundTiles = new Sprite[15 * 8, 20];
    private float backgroundY = 15 * 7 * 64;

    // フォントの変数
    private Font fontTitle = null!;
    private Font fontScore = null!;

    // 画像の変数
    private Bitmap bmpTitleBack = new("assets/images/bg_planet.png");
    private Bitmap bmpCharacter = new("assets/images/objects.png");
    private Bitmap bmpBackground = new("assets/images/bg0.png");
    private Bitmap bmpBackground2 = new("assets/images/bg_space_long.png");

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
      enemies.Clear();
      enemyBullets.Clear();
      bossList.Clear();
      animatedSpriteList.Clear();
      backgroundY = 15 * 7 * 64;

      // 背景データを作成
      int[][,] tileList = new int[maxStageNo][,];
      tileList[0] = new int[,]{ { 0, 224, 30 }, { 32, 224, 20 }, { 64, 224, 20 }, { 96, 224, 20 }, { 128, 224, 10 }, { 160, 224, 2 }, { 192, 224, 3 } , { 224, 224, 1 } };
      tileList[1] = new int[,]{ { 288, 32, 50 }, { 0, 320, 50 }, { 32, 320, 50 }, { 288, 0, 5 }, { 288, 64, 2 }, { 320, 0, 5 }, { 320, 32, 1 }, { 320, 64, 1 } };

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
          int px = tileList[stageNo][id, 0];
          int py = tileList[stageNo][id, 1];
          backgroundTiles[y, x] = new Sprite(x * 32, y * 32, new(px, py, 32, 32));
        }
      }
      if (stageNo == 1)
      {
        Rectangle rectU = new(128, 0, 32, 32);
        Rectangle rectC = new(448, 64, 32, 32);
        Rectangle rectD = new(128, 64, 32, 32);
        int[] indices = { 0, 1, 2, 3, 4, 5, 6 };
        Random.Shared.Shuffle(indices);
        for (int a = 0; a < 4; a++)
        {
          int y = indices[a] * 15 + r.Next(10);
          for (int x = 0; x < 20; x++)
          {
            backgroundTiles[y, x] = new Sprite(x * 32, y * 3 * 32, rectU);
            backgroundTiles[y + 1, x] = new Sprite(x * 32, y * 3 * 32 + 32, rectC);
            backgroundTiles[y + 2, x] = new Sprite(x * 32, y * 3 * 32 + 64, rectD);
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

      // 敵の出現
      if (backgroundY > 720)
      {
        enemySpawnTimer += deltaTime;
        if (enemySpawnTimer >= 1)
        {
          enemySpawnTimer -= 1;
          if (enemies.Count < 10)
          {
            int type = rand.Next(9);
            enemies.Add(new Enemy(rand.Next(18) * 64 + 64, -64, type, player.Sprite));
          }
        }
      }

      for (int a = 0; a < enemies.Count; a++)
      {
        enemies[a].Update(deltaTime, enemyBullets);
      }
      enemies.RemoveAll((Enemy e) => {
        return e.Sprite.Y >= nativeHeight + e.Sprite.Rect.Height || e.Sprite.Y < -(e.Sprite.Rect.Height + 100); });

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

      if (backgroundY == 1)
      {
        Enemy boss = new(nativeWidth / 2, -192, 9, player.Sprite);
        enemies.Add(boss);
        bossList.Add(boss);
      }

      // プレイヤーの弾と敵の衝突判定
      for (int a = 0; a < player.Bullets.Count; a++)
      {
        NormalBullet bullet = player.Bullets[a];
        Box boxA = Box.Add(bullet.Hitbox, bullet.Sprite.X, bullet.Sprite.Y);
        for (int b = 0; b < enemies.Count; b++)
        {
          Enemy e = enemies[b];
          Box boxB = Box.Add(e.Hitbox, e.Sprite.X, e.Sprite.Y);
          if (boxA.IsCollide(boxB))
          {
            e.Hp--;
            if (e.Hp <= 0)
            {
              // 爆発を表示
              animatedSpriteList.Add(new(e.Sprite.X, e.Sprite.Y, 6, 15, new(320, 368, 32, 32)));

              // 得点を増やす
              if (e.Type < Enemy.ScoreList.Length)
              {
                score += Enemy.ScoreList[e.Type];
              }

              // 弾と敵を消去
              enemies.RemoveAt(b);
            }
            else
            {
              AnimatedSprite s = new(
                bullet.Sprite.X, bullet.Sprite.Y - bullet.Sprite.Rect.Height / 2,
                4, 30, new(304, 352, 16, 16));
              s.Sprite.Priority = 1;
              animatedSpriteList.Add(s);
              score += 10;
            }
            player.Bullets.RemoveAt(a);
            a--;
            break;
          }
        }
      }

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
        Box boxPlayer = Box.Add(player.Hitbox, player.Sprite.X, player.Sprite.Y);
        for (int a = 0; a < enemyBullets.Count; a++)
        {
          NormalBullet b = enemyBullets[a];
          Box boxBullet = Box.Add(b.Hitbox, b.Sprite.X, b.Sprite.Y);
          if (boxBullet.IsCollide(boxPlayer))
          {
            animatedSpriteList.Add(new(player.Sprite.X, player.Sprite.Y, 6, 15, new(320, 368, 32, 32)));
            player.IsDead = true;
            deadTimer = 2;
            break;
          }
        }

        // 敵とプレイヤーの衝突判定
        for (int a = 0; a < enemies.Count; a++)
        {
          Enemy e = enemies[a];
          Box boxEnemy = Box.Add(e.Hitbox, e.Sprite.X, e.Sprite.Y);
          if (boxEnemy.IsCollide(boxPlayer))
          {
            animatedSpriteList.Add(new(player.Sprite.X, player.Sprite.Y, 6, 15, new(320, 368, 32, 32)));
            player.IsDead = true;
            deadTimer = 2;
            break;
          }
        }
      }

      // 爆発・ヒットエフェクトの更新
      animatedSpriteList.Sort((a, b) => (int)(a.Sprite.Priority - b.Sprite.Priority));
      for (int a = 0; a < animatedSpriteList.Count; a++)
      {
        animatedSpriteList[a].Update(deltaTime);
      }
      animatedSpriteList.RemoveAll((AnimatedSprite e) => { return e.AnimeTimer >= e.Count; });

      // ステージクリア判定
      if (!player.IsDead && bossList.Count > 0)
      {
        bossList.RemoveAll(boss => boss.Hp <= 0);
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

#if false
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

      int tileY = minY * 64 - (int)backgroundY;
      for (int y = minY; y < maxY; y++)
      {
        for (int x = 0; x < 20; x++)
        {
          Rectangle d = new(x * 64, tileY, 64, 64);
          g.DrawImage(bmpBackground, d, backgroundTiles[y, x].Rect, GraphicsUnit.Pixel);
        }
        tileY += 64;
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
        enemies[a].Sprite.Draw(g, bmpCharacter);
      }

      for (int a = 0; a < player.Bullets.Count; a++)
      {
        player.Bullets[a].Sprite.Draw(g, bmpCharacter);
      }

      for (int a = 0; a < enemyBullets.Count; a++)
      {
        enemyBullets[a].Sprite.Draw(g, bmpCharacter);
      }

      for (int a = 0; a < animatedSpriteList.Count; a++)
      {
        animatedSpriteList[a].Sprite.Draw(g, bmpCharacter);
      }

      if (!player.IsDead)
      {
        player.Sprite.Draw(g, bmpCharacter);
      }

      // 得点を表示
      TextRenderer.DrawText(g, score.ToString(), fontScore, new Point(400, 40), Color.White);

    } // OnPaintメソッドブロックの終わり
  }
}
