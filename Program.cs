using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fantacalcio
{
    class Program
    {
        private static string mainPath = Environment.CurrentDirectory;
        static void Main(string[] args)
        {
            bool fileEmpty = false;
            string[] nomiFantaAllenatori = new string[0];
            string[] fantaAllenatoriNoSquadra = new string[0];
            mainPath = CleanPath(mainPath);
            ExistLogs();
            WriteLogs("Il programma è stato eseguito");
            Setup(ref fileEmpty, ref nomiFantaAllenatori, ref fantaAllenatoriNoSquadra);
        }
        //pulisce il percorso del programma
        private static string CleanPath(string path)
        {
            string[] tmp = path.Split('\\');
            var tmpList = tmp.ToList();
            tmpList.Remove("net5.0");
            tmpList.Remove("Debug");
            tmpList.Remove("bin");
            tmp = tmpList.ToArray();
            string outputPath = tmp[0];
            for (int i = 1; i < tmp.Length; i++)
            {
                outputPath += $"\\{tmp[i]}";
            }
            return outputPath;
        }
        //controlla se esiste il file dei logs
        private static void ExistLogs()
        {
            if(!File.Exists(mainPath + "\\logs.txt"))
            {
                File.Create(mainPath + "\\logs.txt");
                Console.WriteLine("logs creati");
            }
        }
        //scrive un log di quel che succede all'interno del programma
        private static void WriteLogs(string log)
        {
            File.AppendAllText(mainPath + "\\logs.txt", $"{DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]")} {log}" + Environment.NewLine);
        }
        //ottiene i nomi dei FantaAllenatori
        private static string[] GetPlayersName(string[] playersName)
        {
            playersName = Directory.GetFileSystemEntries(mainPath + "\\Squadre");
            for (int i = 0; i < playersName.Length; i++)
            {
                string[] str = playersName[i].Split("Squadre\\");
                string[] str2 = str[1].Split(".txt");
                playersName[i] = str2[0];
            }
            return playersName;
        }
        //controlla se tutti i giocatori hanno formato una squadra
        private static int CheckPlayersSquad(ref bool fileEmpty, ref string[] nomiFantaAllenatori, ref string[] fantaAllenatoriNoSquadra)
        {
            nomiFantaAllenatori = GetPlayersName(nomiFantaAllenatori);
            int nFantaAllenatoriNoSquadra = 0;
            for (int i = 0; i < nomiFantaAllenatori.Length; i++)
            {
                string[] tmp = File.ReadAllLines(mainPath + $"\\Squadre\\{nomiFantaAllenatori[i]}.txt");
                if (tmp.Length == 0)
                {
                    Array.Resize(ref fantaAllenatoriNoSquadra, fantaAllenatoriNoSquadra.Length + 1);
                    fantaAllenatoriNoSquadra[fantaAllenatoriNoSquadra.Length - 1] = nomiFantaAllenatori[i];
                    nFantaAllenatoriNoSquadra++;
                }
            }
            if (nFantaAllenatoriNoSquadra == 0)
            {
                Array.Resize(ref fantaAllenatoriNoSquadra, 0);
                return 1;//tutti hanno almeno 1 calciatore
            }
            return -1;//almeno un giocatore non ha un calciatore
        }
        private static bool IsDirectoryEmpty(string path)//https://stackoverflow.com/questions/755574/how-to-quickly-check-if-folder-is-empty-net/954837
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
        private static void Setup(ref bool fileEmpty, ref string[] nomiFantaAllenatori, ref string[] fantaAllenatoriNoSquadra)
        {
            if (!Directory.Exists(mainPath + "\\Squadre") || IsDirectoryEmpty(mainPath + "\\Squadre") || CheckPlayersSquad(ref fileEmpty, ref nomiFantaAllenatori, ref fantaAllenatoriNoSquadra) != 1)
            {
                if (fantaAllenatoriNoSquadra.Length == 0)
                {
                    WriteLogs("creazione file per le squadre dei giocatori");
                    Console.WriteLine($"Inizio configurazione squadre");
                    Console.WriteLine("Quanti sono i giocatori?");
                    int nPlayer = 0; bool correctSyntax = false;
                    CheckPlayersNum(ref nPlayer, ref correctSyntax);
                    CheckPlayersName(ref nomiFantaAllenatori);
                    Array.Resize(ref nomiFantaAllenatori, nPlayer);
                    if (!Directory.Exists(mainPath + "\\Squadre")) { DirectoryInfo setupFolder = Directory.CreateDirectory(mainPath + "\\Squadre"); WriteLogs("cartella per contenere i file creata"); }
                    for (int i = 0; i < nPlayer; i++)
                    {
                        nomiFantaAllenatori[i] = RemoveSpecialCharacters(nomiFantaAllenatori[i]);
                        using (StreamWriter sw = File.CreateText($"Squadre\\{nomiFantaAllenatori[i]}.txt")) { }
                    }
                    Console.WriteLine($"Creazione file completata");
                    WriteLogs("file necessari per il gioco creati");
                }
                else { }//vado a riprendere singolarmente ogni giocatore finchè non hanno almeno un calciatore nella loro squadra
            }
        }
        //verifica che venga inserito un numero idoneo di giocatori
        private static void CheckPlayersNum(ref int nPlayer, ref bool correctSyntax)
        {
            do
            {
                correctSyntax = int.TryParse(Console.ReadLine(), out nPlayer);
                if (!correctSyntax)
                {
                    Console.WriteLine("Inserisci un valore valido");
                }
                else if (nPlayer <= 1 || nPlayer > 24)
                {
                    Console.WriteLine("Inserisci un numero maggiore di 1 e minore di 25");
                }
            } while (!correctSyntax || nPlayer <= 1 || nPlayer > 24);
        }
        //popola l'array con i nomi dei giocatori, e verifica se sono presenti nomi uguali
        private static void CheckPlayersName(ref string[] nomiFantaAllenatori)
        {
            for (int i = 0; i < nomiFantaAllenatori.Length; i++)
            {
                Console.WriteLine($"Scrivi il nome del giocatore n°{i + 1}");
                nomiFantaAllenatori[i] = Console.ReadLine();
                if (CheckSameName(ref nomiFantaAllenatori, i))
                {
                    i--;
                    Console.WriteLine("Nome già registrato, inseriscine un altro");
                }
            }
        }
        //ispeziona tutto l'array per vedere se esiste già il giocatore che l'utente ha inserito
        private static bool CheckSameName(ref string[] nomiFantaAllenatori, int nNomiInseriti)
        {
            for (int i = 0; i < nNomiInseriti; i++)
            {
                if (nomiFantaAllenatori[nNomiInseriti] == nomiFantaAllenatori[i])
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
    }
}