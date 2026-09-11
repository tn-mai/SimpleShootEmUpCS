using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShootingGameCS
{
  internal static class Program
  {
    //OSの「スリープ時間の精度を変える機能」を使えるようにする
    [DllImport("winmm.dll")]
    static extern uint timeBeginPeriod(uint uMilliseconds);

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      timeBeginPeriod(1); //スリープの精度を１ミリ秒に設定

      // To customize application configuration such as set high DPI settings or default font,
      // see https://aka.ms/applicationconfiguration.
      ApplicationConfiguration.Initialize();

      Form1 form = new();
      form.Show();

      form.Initialize();

      Stopwatch sw = new();
      sw.Start();
      double prevTotalSeconds = 0;
      for (; !form.IsDisposed;)
      {
        int wait = (int)((1.0 / 60.0 - (sw.Elapsed.TotalSeconds - prevTotalSeconds)) * 1000);
        if (wait > 0)
        {
          Thread.Sleep(wait);
        }
        form.Text = (sw.Elapsed.TotalSeconds - prevTotalSeconds).ToString();
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
      sw.Stop();
    }
  }
}