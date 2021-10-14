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
            string command;
            do
            {
                Console.Write("Fantacalcio>");
                command = Console.ReadLine();
                Comandi(command, mainFolderPath);
            } while (command != "exit");
            Console.WriteLine("Il programma si sta chiudendo");
        }
        //in base a ciò che l'utente inserisce come comando lo switch esegue diverse attività e riconosce se un utente non inserisce un comando
        private static void Comandi(string comand, string mainFolderPath)
        {
            switch (comand)
            {
                case "exit": break;//"esce dal programma", sostanzialmente non fa gninte
                case "help"://mostra i comandi, stampa a schermo il file txt "Help.txt"
                    MostraComandi(mainFolderPath);
                    break;
                default:
                    string[] commands = comand.Split(" ");
                    if (commands[0] == "rgd")
                    {
                        ControlloComandi(commands);
                    }
                    else
                    {
                        Console.WriteLine("Comando non trovato o non accessibile in questa situazione");
                    }
                    break;
            }
        }
        private static void MostraComandi(string mainFolderPath)
        {
            string[] help = File.ReadAllLines(mainFolderPath + "\\Help.txt");
            for (int i = 0; i < help.Length; i++)
            {
                string[] str = help[i].Split(',');
                Console.WriteLine($"{str[0]}  --->  {str[1]}");
            }
        }
        //a seconda del comando scritto dall'utente, che inizia per "rgd", esegue diverse funzioni
        private static void ControlloComandi(string[] comands)
        {
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
                    nomeGiocatori[i] = RemoveSpecialCharacters(nomeGiocatori[i]);
                    using (StreamWriter sw = File.CreateText(@$"Squadre\{nomeGiocatori[i]}.txt")) { }
                }
                Console.WriteLine($"Configurazione completata");
                SetupSquadreGiocatori(nomeGiocatori, mainFolderPath);
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
                ControlloSquadreVuote(mainFolderPath, nomiGiocatori);
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
                else if (nPlayer <= 1 || nPlayer > 24)
                {
                    Console.WriteLine("Inserisci un numero maggiore di 1 e minore di 25");
                }
            } while (!correctSyntax || nPlayer <= 1 || nPlayer > 24);
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
        //crea le squadre di ogni giocatore
        private static void SetupSquadreGiocatori(string[] nomiGiocatori, string path)
        {
            bool finito = false;
            int[] soldiGiocatori = new int[nomiGiocatori.Length];
            SetupFantacrediti(soldiGiocatori);
            do
            {
                Console.WriteLine("Scrivi il giocatore che deve essere messo all'asta");
                string calciatore = Console.ReadLine().ToLower();
                if (ControlloEsistenzaCalciatore(calciatore, path))
                {
                    Asta(calciatore, nomiGiocatori, path, soldiGiocatori);
                }
                else if (calciatore == "exitasta")
                {
                    finito = true;
                }
                else
                {
                    Console.WriteLine("Calciatore non trovato\n");
                }
            } while (!finito);
            ControlloSquadreVuote(path, nomiGiocatori);
        }
        //consegna a tutti i giocatori un tot di crediti con cui possono acquistare i calciatori
        private static void SetupFantacrediti(int[] soldi)
        {
            for (int i = 0; i < soldi.Length; i++)
            {
                soldi[i] = 500;
            }
        }
        //controlla se il calciatore che si vuole acquistare esiste
        private static bool ControlloEsistenzaCalciatore(string calciatoreAsta, string path)
        {
            string[] calciatori = File.ReadAllLines(path + "\\Calciatori.txt");
            for (int i = 0; i < calciatori.Length; i++)
            {
                string[] calciatore = calciatori[i].Split(',');
                if (calciatoreAsta == calciatore[0]) return true;
            }
            return false;
        }
        //gestisce l'acquisto dei calciatori da parte dei partecipanti
        private static void Asta(string calciatore, string[] nomiGiocatori, string path, int[] soldiGiocatori)
        {
            bool astaFinita = false;
            int offertaAsta = 0;
            do
            {
                Console.WriteLine("Inserisci un offerta");
                string risposta = Console.ReadLine().ToLower();
                try
                {
                    int tmp = Convert.ToInt32(risposta);
                    if (tmp > offertaAsta)
                        offertaAsta = tmp;
                    else
                        Console.WriteLine("Devi inserire un prezzo maggiore rispetto all'asta corrente");
                }
                catch
                {
                    if (risposta == "exitasta")
                    {
                        AssegnazioneCalciatore(calciatore, nomiGiocatori, path, ref astaFinita, offertaAsta, soldiGiocatori);
                    }
                    else
                        Console.WriteLine("Prezzo non valido");
                }
            } while (!astaFinita);
            Console.WriteLine($"L'asta per il calciatore: {calciatore} e' conclusa");
        }
        //permette ai giocatori di popolare la propria squadra, controlla anche se i giocatori quando vogliono acquistare un calciatore hanno abbastanza crediti
        private static void AssegnazioneCalciatore(string calciatore, string[] nomiGiocatori, string path, ref bool astaFinita, int offertaAsta, int[] soldiGiocatori)
        {
            bool nomeCorretto = false;
            do
            {
                Console.WriteLine("Inserisci il nome dell'utente che ha effettuato l'ultima offerta");
                string nomePlayer = Console.ReadLine();
                for (int i = 0; i < nomiGiocatori.Length; i++)
                {
                    if (nomePlayer == nomiGiocatori[i])
                    {
                        if(soldiGiocatori[i] >= offertaAsta)
                        {
                            string[] squadraPlayer = File.ReadAllLines(path + $"\\{nomiGiocatori[i]}.txt");
                            Array.Resize(ref squadraPlayer, squadraPlayer.Length + 1);
                            squadraPlayer[squadraPlayer.Length - 1] = calciatore;
                            File.WriteAllLines(path + $"\\{nomiGiocatori[i]}.txt", squadraPlayer);
                            AcquistoCalciatore(path, calciatore);
                            soldiGiocatori[i] -= offertaAsta;
                            nomeCorretto = true;
                            astaFinita = true;
                            break;
                        }
                        else
                            Console.WriteLine($"{nomePlayer} non ha abbastanza crediti, rieffettuare l'asta per questo calciatore"); nomeCorretto = true; astaFinita = true;
                    }
                }
            } while (!nomeCorretto);
        }
        //rende un calciatore non più acquistabile, settando la sua variabile a false, nel file
        private static void AcquistoCalciatore(string path, string calciatore)
        {
            string[] calciatori = File.ReadAllLines(path + "\\Calciatori.txt");
            for (int i = 0; i < calciatori.Length; i++)
            {
                string[] tmp = calciatori[i].Split(',');
                if (calciatore == tmp[0])
                {
                    tmp[3] = "true";
                    calciatori[i] = $"{tmp[0]},{tmp[1]},{tmp[2]},{tmp[3]}";
                    File.WriteAllLines(path + "\\Calciatori.txt", calciatori);
                    break;
                }
            }
        }
        private static void ControlloSquadreVuote(string path, string[] nomiPlayers)//https://stackoverflow.com/questions/61208202/how-to-check-if-a-text-file-is-empty-c-sharp
        {
            for(int i = 0; i < nomiPlayers.Length; i++)
            {
                string[] tmp = File.ReadAllLines(path + $"\\Squadre\\{nomiPlayers[i]}.txt");
                if (tmp.Length == 0)
                {
                    Console.WriteLine($"{nomiPlayers[i]} non penso che tu possa vincere con una squadra di 0 calciatori");
                    File.Delete(path + $"\\Squadre\\{nomiPlayers[i]}.txt");
                }
            }
            Console.WriteLine("\nPer pigrizia dello sviluppatore dovrete ricominciare dall'inizio\n");
            string[] nomiGiocatori = new string[0];
            Setup(path, ref nomiGiocatori);
        }
    }
}