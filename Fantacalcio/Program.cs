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
            int[] fantaCrediti = new int[0];
            mainPath = CleanPath(mainPath);
            ExistLogs();
            WriteLogs("Il programma è stato eseguito");
            Setup(ref fileEmpty, ref nomiFantaAllenatori, ref fantaAllenatoriNoSquadra, ref fantaCrediti);
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
            if (!File.Exists(mainPath + "\\logs.txt"))
            {
                File.Create(mainPath + "\\logs.txt").Dispose();
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
        private static void Setup(ref bool fileEmpty, ref string[] nomiFantaAllenatori, ref string[] fantaAllenatoriNoSquadra, ref int[] fantaCrediti)
        {
            if (!Directory.Exists(mainPath + "\\Squadre") || IsDirectoryEmpty(mainPath + "\\Squadre") || CheckPlayersSquad(ref fileEmpty, ref nomiFantaAllenatori, ref fantaAllenatoriNoSquadra) == -1)
            {
                if (fantaAllenatoriNoSquadra.Length == 0)
                {
                    WriteLogs("creazione file per le squadre dei giocatori");
                    Console.WriteLine($"Inizio configurazione squadre");
                    Console.WriteLine("Quanti sono i giocatori?");
                    int nPlayer = 0; bool correctSyntax = false;
                    CheckPlayersNum(ref nPlayer, ref correctSyntax);
                    Array.Resize(ref nomiFantaAllenatori, nPlayer);
                    if (!Directory.Exists(mainPath + "\\Squadre")) { DirectoryInfo setupFolder = Directory.CreateDirectory(mainPath + "\\Squadre"); WriteLogs("cartella per contenere i file creata"); }
                    CheckPlayersName(ref nomiFantaAllenatori);
                    for (int i = 0; i < nPlayer; i++)
                    {
                        nomiFantaAllenatori[i] = RemoveSpecialCharacters(nomiFantaAllenatori[i]);
                        using (StreamWriter sw = File.CreateText($"{mainPath}\\Squadre\\{nomiFantaAllenatori[i]}.txt")) { }
                    }
                    Console.WriteLine($"Creazione file completata");
                    WriteLogs("file necessari per il gioco creati");
                    AssegnazioneFantacrediti(ref nPlayer, ref fantaCrediti);
                    string[] listaCalciatoriDaAcquistare = new string[0];
                    ListaAsta(ref nomiFantaAllenatori, ref fantaCrediti, ref listaCalciatoriDaAcquistare);
                    WriteLogs("completata la lista dei calciatori per l'asta");
                    Asta(ref nomiFantaAllenatori, ref fantaCrediti, ref listaCalciatoriDaAcquistare);
                }
                else { }//vado a riprendere singolarmente ogni giocatore finchè non hanno almeno un calciatore nella loro squadra
            }
            else { }//quando ci sono tutti i file necessari e tutti i fanta-allenatori hanno almeno una rosa decente
        }
        //assegna i fantacreditia a tutti i giocatori
        private static void AssegnazioneFantacrediti(ref int nPlayer, ref int[] fantaCrediti)
        {
            for (int i = 0; i < nPlayer; i++)
            {
                Array.Resize(ref fantaCrediti, fantaCrediti.Length + 1);
                fantaCrediti[i] = 500;
            }
            WriteLogs($"Sono stati assegnati i fantacrediti a tutti i giocatori");
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
                else if (nPlayer <= 1 || nPlayer > 56)
                {
                    Console.WriteLine("Inserisci un numero maggiore di 1 e minore di 57");
                }
            } while (!correctSyntax || nPlayer <= 1 || nPlayer > 56);
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
        private static void ListaAsta(ref string[] nomiFantaAllenatori, ref int[] fantaCrediti, ref string[] listaCalciatoriDaAcquistare)
        {
            Console.WriteLine("Inizio asta");
            WriteLogs("comicia l'asta dei calciatori");
            string[] ruoliCalciaotirDaAcquistare = new string[] { "PORTIERE", "DIFENSORE", "CENTROCAMPISTA", "ATTACCANTE" };
            byte[] nMaxRuoliClaciatoriDaAcquistare = new byte[] { 1, 4, 4, 3 };
            for (int i = 0; i < nomiFantaAllenatori.Length; i++)
            {
                int nCalciatoriInseriti = 0, nMaxRulo = 0, ruolo = 0;
                do
                {
                    Console.WriteLine($"Giocatore {nomiFantaAllenatori[i]} scrivi il nome del {ruoliCalciaotirDaAcquistare[ruolo]} che vuoi acquistare");
                    string calciatoreDaComprare = Console.ReadLine();
                    if (ControlloEsistenza_RuoloCalciatore(ref calciatoreDaComprare, ruoliCalciaotirDaAcquistare[ruolo].ToLower()))
                    {//esiste il calciatore
                        if (ControlloLista(ref listaCalciatoriDaAcquistare, ref calciatoreDaComprare))
                        {
                            Array.Resize(ref listaCalciatoriDaAcquistare, listaCalciatoriDaAcquistare.Length + 1);
                            listaCalciatoriDaAcquistare[listaCalciatoriDaAcquistare.Length - 1] = calciatoreDaComprare;
                        }
                        nCalciatoriInseriti++; nMaxRulo++;
                        if (nMaxRulo == nMaxRuoliClaciatoriDaAcquistare[ruolo])
                        {
                            ruolo++;
                            nMaxRulo = 0;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Non esiste il calciatore o non è del ruolo richiesto\n");
                    }
                } while (nCalciatoriInseriti < 11);
            }
        }
        //controlla se l'utente ha inserito il nome di un calciatore valido e se ha il ruolo richiesto
        private static bool ControlloEsistenza_RuoloCalciatore(ref string calciatoreDaComprare, string ruolo)
        {
            string[] calciatori = File.ReadAllLines(mainPath + "\\Calciatori.txt");
            for (int i = 0; i < calciatori.Length; i++)
            {
                string[] calciatore = calciatori[i].Split(',');
                if (calciatoreDaComprare == calciatore[0] && ruolo == calciatore[1])
                    return true;
            }
            return false;
        }
        //controlla se è già presente un calciatore nella lista dell'asta
        private static bool ControlloLista(ref string[] listaCalciatoriDaAcquistare, ref string calciatoreDaComprare)
        {
            for (int i = 0; i < listaCalciatoriDaAcquistare.Length; i++)
            {
                if (calciatoreDaComprare != listaCalciatoriDaAcquistare[i])
                {
                    return true;//nessun giocatore ha già scelto il calciatore
                }
            }
            if (listaCalciatoriDaAcquistare.Length == 0)
            {
                return true;
            }
            return false;
        }
        private static void Asta(ref string[] nomiFantaAllenatori, ref int[] fantaCrediti, ref string[] listaCalciatoriDaAcquistare)
        {
            for (int i = 0; i < listaCalciatoriDaAcquistare.Length; i++)
            {
                bool astaFinita = false;
                int offertaAsta = 0;
                string risposta = "";
                do
                {
                    Console.WriteLine($"Inserisci un offerta per il calciatore: {listaCalciatoriDaAcquistare[i]}");
                    risposta = Console.ReadLine().ToLower();
                    WriteLogs($"è stata inseria un offerta pari a: '{risposta}'");
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
                            AssegnazioneCalciatore(ref nomiFantaAllenatori, ref astaFinita, offertaAsta, ref fantaCrediti, listaCalciatoriDaAcquistare[i]);
                            WriteLogs($"I giocatori hanno terminato l'asta per un calciatore");
                        }
                        else
                            Console.WriteLine("Prezzo non valido");
                    }
                } while (!astaFinita);
                Console.WriteLine($"L'asta per il calciatore: {risposta} e' conclusa");
            }
            WriteLogs($"L'asta si è completamente conclusa");
        }
        //effettua le operazione di acquisto di un calciatore da parte di un giocatore
        private static void AssegnazioneCalciatore(ref string[] nomiFantaAllenatori, ref bool astaFinita, int offertaAsta, ref int[] fantaCrediti, string calciatore)
        {
            bool nomeCorretto = false;
            do
            {
                Console.WriteLine("\nInserisci il nome dell'utente che ha effettuato l'ultima offerta");
                string nomePlayer = Console.ReadLine();
                for (int i = 0; i < nomiFantaAllenatori.Length; i++)
                {
                    if (nomePlayer == nomiFantaAllenatori[i])
                    {
                        if (fantaCrediti[i] >= offertaAsta)
                        {
                            string[] squadraPlayer = File.ReadAllLines(mainPath + $"\\Squadre\\{nomiFantaAllenatori[i]}.txt");
                            if (squadraPlayer.Length < 11)
                            {
                                Array.Resize(ref squadraPlayer, squadraPlayer.Length + 1);
                                squadraPlayer[squadraPlayer.Length - 1] = calciatore;
                                File.WriteAllLines(mainPath + $"\\Squadre\\{nomiFantaAllenatori[i]}.txt", squadraPlayer);
                                AcquistoCalciatore(calciatore);
                                fantaCrediti[i] -= offertaAsta;
                                nomeCorretto = true;
                                astaFinita = true;
                                WriteLogs($"Il giocatore '{nomePlayer}' ha comprato il seguente calciatore: {calciatore}, alla modica cifra di: {offertaAsta} fantacrediti");
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"{nomePlayer} non puoi più acquistare calciatori in quanto hai già una squadra completa");
                                WriteLogs($"Il giocatore '{nomePlayer} ha provato ad acquistare un calciatore anche se però possiede già una squadra al completo, quindi gli è stato annullato l'acquisto");
                                nomeCorretto = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{nomePlayer} non ha abbastanza crediti, rieffettuare l'asta per questo calciatore");
                            nomeCorretto = true; astaFinita = true;
                            WriteLogs($"Il giocatore '{nomePlayer}' voleva acquistare il calciatore '{calciatore}', ma purtroppo non aveva abbastanza fantacrediti");
                        }
                    }
                }
            } while (!nomeCorretto);
        }
        //rende impossbile l'eventuale tentativo di riacquisto di un calciatore
        private static void AcquistoCalciatore(string calciatore)
        {
            string[] calciatori = File.ReadAllLines(mainPath + "\\Calciatori.txt");
            for (int i = 0; i < calciatori.Length; i++)
            {
                string[] tmp = calciatori[i].Split(',');
                if (calciatore == tmp[0])
                {
                    tmp[3] = "true";
                    calciatori[i] = $"{tmp[0]},{tmp[1]},{tmp[2]},{tmp[3]}";
                    File.WriteAllLines(mainPath + "\\Calciatori.txt", calciatori);
                    break;
                }
            }
        }
    }
}