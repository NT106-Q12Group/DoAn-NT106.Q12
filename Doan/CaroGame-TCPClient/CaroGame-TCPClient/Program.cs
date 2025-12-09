using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace CaroGame_TCPClient
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run();

        }
    }
}