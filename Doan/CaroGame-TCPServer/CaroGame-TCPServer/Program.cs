using System;

namespace CaroGame_TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("|==================================|");
            Console.WriteLine("|        PLAYER MANAGEMENT         |");
            Console.WriteLine("|           -TCP SERVER-           |");
            Console.WriteLine("|==================================|");
            Console.WriteLine();

            try
            {
                Databases.Databases.InitializeDatabase();
                Console.WriteLine($"Database initialized successfully!");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured while initializing database! ({ex.Message})");
                Console.WriteLine("Press any button to exit!");
                Console.ReadKey();
                return;
            }
            TCPServer server = new TCPServer(25565);
            server.StartServer();

            Console.WriteLine("Press 'E' to stop running server....");
            Console.WriteLine();
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.E)
                {
                    server.Stop();
                    break;
                }
            }

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }
}