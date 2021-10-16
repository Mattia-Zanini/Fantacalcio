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
            bool fileEmpty = false;
            string[] nomiFantaAllenatori = new string[0];
            string[] fantaAllenatoriNoSquadra = new string[] { "ciao" };
            string mainPath = Environment.CurrentDirectory;
            mainPath = CleanPath(mainPath);
            if (!Directory.Exists(mainPath + "\\Squadre") || IsDirectoryEmpty(mainPath + "\\Squadre") || CheckPlayersSquad(ref fileEmpty, ref mainPath, ref nomiFantaAllenatori, ref fantaAllenatoriNoSquadra) != 1)
            {
                Console.WriteLine("non c'è");
            }
        }
        //pulisce il percorso del programma
        private static string CleanPath(string path)
        {
            string[] tmp = path.Split('\\');
            tmp.ToList().Remove("net5.0");
            tmp.ToList().Remove("Debug");
            tmp.ToList().Remove("bin");
            string outputPath = tmp[0];
            for (int i = 1; i < tmp.Length; i++)
            {
                outputPath += $"\\{tmp[i]}";
            }
            return outputPath;
        }
        //ottiene i nomi dei FantaAllenatori
        private static string[] GetPlayersName(ref string mainPath, string[] playersName)
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
        private static int CheckPlayersSquad(ref bool fileEmpty, ref string mainPath, ref string[] nomiFantaAllenatori, ref string[] fantaAllenatoriNoSquadra)
        {
            nomiFantaAllenatori = GetPlayersName(ref mainPath, nomiFantaAllenatori);
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
            return -1;
        }
        private static bool IsDirectoryEmpty(string path)//https://stackoverflow.com/questions/755574/how-to-quickly-check-if-folder-is-empty-net/954837
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}