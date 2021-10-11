using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fantacalcio
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = Environment.CurrentDirectory;
            string mainFolderPath = SetupDir(dir);
            string[] nomiGiocatori = new string[0];
            Setup(mainFolderPath, ref nomiGiocatori);
            string comand;
            do
            {
                Console.Write("<comand>");
                comand = Console.ReadLine();
                Comandi(comand, mainFolderPath);
            } while (comand != "exit");
            Console.WriteLine("Il programma si sta chiudendo");
        }
        //in base a ciò che l'utente inserisce come comando lo switch esegue diverse attività
        private static void Comandi(string comand, string mainFolderPath)
        {
            switch (comand)
            {
                case "exit": break;//"esce dal programma", sostanzialmente non fa gninte
                case "help"://mostra i comandi, stampa a schermo il file txt "Help.txt"
                    MostraComandi(mainFolderPath);
                    break;
            }
        }
        private static void MostraComandi(string mainFolderPath)
        {
            string[] help = File.ReadAllLines(mainFolderPath + "\\Help.txt");
            for (int i = 0; i < help.Length; i++)
            {
                string[] str = help[i].Split(',');
                Console.WriteLine($"{str[0]}\t\t{str[1]}");
            }
        }
        //si occupa di preparare i file che conterranno le squadre dei giocatori
        private static void Setup(string mainFolderPath, ref string[] nomiGiocatori)
        {
            string squadreDir = mainFolderPath + "\\Squadre";
            if (!Directory.Exists(squadreDir) || IsDirectoryEmpty(squadreDir))
            {
                Console.WriteLine($"Inizio configurazione squadre");
                Console.WriteLine("Quanti sono i giocatori?");
                int nPlayer = 0; bool correctSyntax = false;
                ControlloNumeroGiocatori(ref nPlayer, ref correctSyntax);
                string[] nomeGiocatori = new string[nPlayer];
                SquadreGiocatori(ref nomeGiocatori);
                //nel caso non esista la cartella
                if (!Directory.Exists(squadreDir)) { DirectoryInfo setupFolder = Directory.CreateDirectory(squadreDir); }
                for (int i = 0; i < nPlayer; i++)
                {
                    string nome = RemoveSpecialCharacters(nomeGiocatori[i]);
                    using (StreamWriter sw = File.CreateText(@$"Squadre\{nome}.txt")) { }
                }
                Console.WriteLine($"Configurazione completata");
            }
            else
            {
                nomiGiocatori = Directory.GetFileSystemEntries(squadreDir);
                for (int i = 0; i < nomiGiocatori.Length; i++)
                {
                    string[] str = nomiGiocatori[i].Split("Squadre\\");
                    string[] str2 = str[1].Split(".txt");
                    nomiGiocatori[i] = str2[0];
                }
            }
        }
        //Questa funzione si occupa di ritagliare il percorso del file, al fine di ottenerne un altro
        private static string SetupDir(string dir)
        {
            string[] mainPathSliced = dir.Split('\\');
            var mainPathSlicedList = mainPathSliced.ToList();
            mainPathSlicedList.Remove("bin");
            mainPathSlicedList.Remove("Debug");
            mainPathSlicedList.Remove("net5.0");
            mainPathSliced = mainPathSlicedList.ToArray();
            string mainPath = mainPathSliced[0];
            for (int i = 1; i < mainPathSliced.Length; i++)
            {
                mainPath += $"\\{mainPathSliced[i]}";
            }
            return mainPath;
        }
        private static void ControlloNumeroGiocatori(ref int nPlayer, ref bool correctSyntax)
        {
            do
            {
                correctSyntax = int.TryParse(Console.ReadLine(), out nPlayer);
                if (!correctSyntax)
                {
                    Console.WriteLine("Inserisci un valore valido");
                }
                else if (nPlayer <= 0)
                {
                    System.Console.WriteLine("Inserisci un numero maggiore di 0");
                }
            } while (!correctSyntax || nPlayer <= 0);
        }
        private static void SquadreGiocatori(ref string[] nomeGiocatori)
        {
            for (int i = 0; i < nomeGiocatori.Length; i++)
            {
                Console.WriteLine($"Scrivi il nome del giocatore n°{i + 1}");
                nomeGiocatori[i] = Console.ReadLine();
                if (ControlloNomeGiocatori(ref nomeGiocatori, i))
                {
                    i--;
                    Console.WriteLine("Nome già registrato, inseriscine un altro");
                }
            }
        }
        private static bool ControlloNomeGiocatori(ref string[] nomeGiocatori, int nomeDaControllare)
        {
            for (int i = 0; i < nomeDaControllare; i++)
            {
                if (nomeGiocatori[nomeDaControllare] == nomeGiocatori[i])
                {
                    return true;
                }
            }
            return false;
        }
        private static string RemoveSpecialCharacters(string str)//https://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_]+", "", RegexOptions.Compiled);
        }
        private static bool IsDirectoryEmpty(string path)//https://stackoverflow.com/questions/755574/how-to-quickly-check-if-folder-is-empty-net/954837
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}