using System;
using DelegatesLib;

namespace Otus17
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Укажите путь до папки с документами: ");
            var path = Console.ReadLine();

            Console.Write("Укажите таймаут ожидания документов (сек): ");
            var timeoutRaw = Console.ReadLine();

            var dc = new DocumentsReceiver();
            dc.DocumentsReady += Dc_DocumentsReady;
            dc.TimedOut += Dc_TimedOut;

            dc.Start(path, int.Parse(timeoutRaw ?? string.Empty));

            Console.ReadLine();
        }

        private static void Dc_TimedOut()
        {
            Console.WriteLine("Время ожидания вышло");
        }

        private static void Dc_DocumentsReady()
        {
            Console.WriteLine("Документы получены");
        }
    }
}
