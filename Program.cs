using System;
using System.Runtime.InteropServices;
using System.Threading;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;

class Program
{
    [DllImport("user32.dll")]
    static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [StructLayout(LayoutKind.Sequential)]
    struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    static Timer timer;
    static InputSimulator simulator;
    static int _interval = 4 * 60 * 1000; // 4 minutes

    static void Main(string[] args)
    {
        
        if (args.Length > 0 && int.TryParse(args[0], out var customInterval))
        {
            _interval = customInterval * 1000 * 60;
        }

        timer = new Timer(Callback, null, 0, _interval);
        simulator = new InputSimulator();

        Console.WriteLine("Alive started. Press any key to exit...");
        Console.Read();

        timer.Dispose();
    }

    static void Callback(Object state)
    {
        try
        {
            int inactivityThreshold = _interval -500;

            if (GetLastInputTime() > inactivityThreshold)
            {
                simulator.Keyboard.KeyPress(VirtualKeyCode.SHIFT);
                Console.WriteLine($"Input simulated at {DateTime.Now}.");
            }
            else
            {
                Console.WriteLine("Skipped.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    static uint GetLastInputTime()
    {
        LASTINPUTINFO lastInputInfo = new();
        lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
        GetLastInputInfo(ref lastInputInfo);

        return (uint)Environment.TickCount - lastInputInfo.dwTime;
    }
}