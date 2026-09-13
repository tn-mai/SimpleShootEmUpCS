using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShootingGameCS
{
  internal static class Program
  {
    //OSの「スリープ時間の精度を変える機能」を使えるようにする
    [DllImport("winmm.dll")]
    static extern uint timeBeginPeriod(uint uMilliseconds);

    [STAThread]
    static void Main()
    {
      timeBeginPeriod(1); //スリープの精度を１ミリ秒に設定

      ApplicationConfiguration.Initialize();

      Form1 form = new();
      form.Show();

      form.Initialize();

      // ゲームループ
      Stopwatch sw = new();
#if true
      for (; !form.IsDisposed;)
      {
        sw.Restart();//時間計測を開始
        form.Update(1.0f / 60.0f);

        form.Refresh();
        Application.DoEvents();
        //経過時間が1/60秒未満の場合、1/60秒が経過するまで停止
        sw.Stop(); //時間計測を終了
        if (sw.ElapsedMilliseconds < 1000 / 60)
        {
          Thread.Sleep(1000 / 60 - (int)sw.ElapsedMilliseconds);
        }
      }
#else
      double prevTotalSeconds = 0;
      sw.Start();
      for (; !form.IsDisposed;)
      {
        int wait = (int)((1.0 / 60.0 - (sw.Elapsed.TotalSeconds - prevTotalSeconds)) * 1000);
        form.Text = (sw.Elapsed.TotalSeconds - prevTotalSeconds).ToString();
        if (wait > 0)
        {
          Thread.Sleep(wait);
        }
        if (sw.Elapsed.TotalSeconds - prevTotalSeconds < 5.0 / 60.0)
        {
          prevTotalSeconds += 1.0 / 60.0;
        }
        else
        {
          prevTotalSeconds = sw.Elapsed.TotalSeconds;
        }

        form.Update(1.0f / 60.0f);

        form.Invalidate();
        Application.DoEvents();
      }
#endif
    } // Mainメソッドブロックの終わり
  }
}