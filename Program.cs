using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShootingGameCS
{
  internal static class Program
  {
    // OSの「スリープ時間の精度を変える機能」を使えるようにする
    [DllImport("winmm.dll")]
    static extern uint timeBeginPeriod(uint uMilliseconds);

    [DllImport("winmm.dll")]
    static extern uint timeEndPeriod(uint uMilliseconds);

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      timeBeginPeriod(1);

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
        double t = sw.Elapsed.TotalSeconds;
        double dt = t - prevTotalSeconds;
        //if (dt < 1.0 / 60.0)
        //{
        //  continue;
        //}
        //if (t - prevTotalSeconds < 1.0 / 120.0)
        //{
        //  Thread.Sleep(8);
        //  t = sw.Elapsed.TotalSeconds;
        //}
        form.Text = dt.ToString();
        form.Update((float)dt);
        prevTotalSeconds = t;

        form.Invalidate();
        Application.DoEvents();
      }
      sw.Stop();
      timeEndPeriod(1);
    }
  }
}