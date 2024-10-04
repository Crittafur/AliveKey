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

    private static Timer _timer;
    private static InputSimulator _simulator;
    private static int _interval = 4 * 60 * 1000; // default to 4 minutes
    private static readonly int _buffer = 500;
    private static bool _isExiting = false;

    static void Main(string[] args)
    {
        
        if (args.Length > 0 && int.TryParse(args[0], out var customInterval))
        {
            _interval = customInterval * 1000 * 60;
        }

        _simulator = new InputSimulator();
        _timer = new Timer(Callback, null, 0, _interval);

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("Exiting...");
            _isExiting = true;
            _timer.Dispose();
            e.Cancel = true; // Prevent immediate termination
        };

        Console.WriteLine("Program running. Press Ctrl+C to exit.");
        
        while (!_isExiting)
        {
            Thread.Sleep(100);
        }
    }

    static void Callback(Object state)
    {
        try
        {
            int inactivityThreshold = _interval - _buffer;

            if (GetLastInputTime() > inactivityThreshold)
            {
                _simulator.Keyboard.KeyPress(VirtualKeyCode.SHIFT);
                Console.WriteLine($"Input simulated at {DateTime.Now}.");
            }
            else
            {
                Console.WriteLine("Skipped. Last input time is less than inactivity threshold.");
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
        
        if (!GetLastInputInfo(ref lastInputInfo))
        {
            throw new Exception("Failed to get last input info.");
        }

        return (uint)Environment.TickCount - lastInputInfo.dwTime;
    }
}