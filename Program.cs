using System.Net;
using System;
using System.IO;

namespace Fantacalcio
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] calciatori = File.ReadAllLines("Calciatori.txt");
            File.WriteAllLines("Calciatori.txt", calciatori);
            Console.WriteLine("Finito");
        }
    }
}