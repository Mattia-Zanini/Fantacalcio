using System;
using System.IO;

namespace Fantacalcio
{
    class Program
    {
        static void Main(string[] args)
        {
            string comand;
            do
            {
                Console.Write("<comand>");
                comand = Console.ReadLine();
                Comandi(comand);
            } while (comand != "exit");
            Console.WriteLine("Il programma si sta chiudendo");
        }
        //in base a ciò che l'utente inserisce come comando lo switch esegue diverse attività
        private static void Comandi(string comand)
        {
            switch (comand)
            {
                case "exit": break;//"esce dal programma", sostanzialmente non fa gninte
                case "help"://mostra i comandi, stampa a schermo il file txt "Help.txt"
                    string[] help = File.ReadAllLines("Help.txt");
                    for(int i = 0; i < help.Length; i++)
                    {
                        string[] str = help[i].Split(',');
                        Console.WriteLine($"{str[0]}\t\t{str[1]}");
                    }
                    break;
            }
        }
    }
}
/*
string[] calciatori = File.ReadAllLines("Calciatori.txt");
File.WriteAllLines("Calciatori.txt", calciatori);
*/