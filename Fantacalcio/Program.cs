using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fantacalcio
{
    class Program
    {
        private static string mainPath = Environment.CurrentDirectory;//imposta la variabile "mainPath", con il percorso assoluto dell'eseguibile, come valore
        static void Main(string[] args)
        {
            bool fileEmpty = false;//variabile necessaria per tener traccia se i file di impostazione sono vuoti
            string[] nomiFantaAllenatori = new string[0];//array dove saranno presenti tutti i nomi dei giocatori
            string[] fantaAllenatoriNoSquadra = new string[0];//array per tener traccia dei fantaallenatori che non hanno una rosa
            int[] fantaCrediti = new int[0];//array dove sono contenuti i fantacrediti di ogni giocatore
            mainPath = CleanPath(mainPath);//chiama della funzione "CleanPath" dove gli viene passata per parametro una stringa, e questa funzione restituirà la stringa, togliendo alcuni percorsi relativi, non necessari
            ExistLogs();//chiama la funzione "ExistLogs" che si occupa di controllare se è presente il file per salvare i logs
            WriteLogs("Il programma è stato eseguito");//chiama la funzione "WriteLogs" per scrivere sul file dei log la stringa passata per parametro
            Setup(ref fileEmpty, ref nomiFantaAllenatori, ref fantaAllenatoriNoSquadra, ref fantaCrediti);//chiama la funzione setup e gli passata tutti gli array e variabili precedentemente creati
        }
        //pulisce il percorso del programma
        private static string CleanPath(string path)
        {
            string[] tmp = path.Split('\\');//divide l'array per ogni \ che divide la stringa
            var tmpList = tmp.ToList();//crea una lista dove inserisce i valori dell'array e per farlo lo converte in una lista
            tmpList.Remove("net5.0");//rimuove la stringa passata per parametro alla funzione, dalla lista
            tmpList.Remove("Debug");//rimuove la stringa passata per parametro alla funzione, dalla lista
            tmpList.Remove("bin");//rimuove la stringa passata per parametro alla funzione, dalla lista
            tmp = tmpList.ToArray();//copia i valori della lista dentro l'array, per farlo converte la lista in un array
            string outputPath = tmp[0];//inizializza la variabile "outPath" con il primo elemento dell'array "tmp"
            for (int i = 1; i < tmp.Length; i++)//ricompone il percorso assoluto
            {
                outputPath += $"\\{tmp[i]}";//aggiunge un pezzo di stringa
            }
            return outputPath;//ritorna il vaore della stringa "outPath"
        }
        //controlla se esiste il file dei logs
        private static void ExistLogs()
        {
            if (!File.Exists(mainPath + "\\logs.txt"))//controlla se nel percorso assoluto passata per parametro esiste il file (il nome e tipo del file sono compresi nel percorso assoluto)
            {
                File.Create(mainPath + "\\logs.txt").Dispose();//crea il file nel percorso assoluto passato per parametro e rilascia le risorse del file
                Console.WriteLine("logs creati");//avvisa l'utente che i file di log sono stati creati
            }
        }
        //scrive un log di quel che succede all'interno del programma
        private static void WriteLogs(string log)
        {
            File.AppendAllText(mainPath + "\\logs.txt", $"{DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]")} {log}" + Environment.NewLine);//aggiunge una stringa alla fine del file, con tanto di "newline"
        }
        //ottiene i nomi dei FantaAllenatori
        private static string[] GetPlayersName(string[] playersName)
        {
            playersName = Directory.GetFileSystemEntries(mainPath + "\\Squadre");//ottiene i percorsi relativi dei file contenuti nel percorso assoluto passato per parametro
            for (int i = 0; i < playersName.Length; i++)
            {
                string[] str = playersName[i].Split("Squadre\\");//splitta l'array in due elementi, 1 è "Squadre\\" il secondo è il nome del file e il suo tipo
                string[] str2 = str[1].Split(".txt");//splitto di nuovo l'array, dove il primo elemento è il nome del file, il secondo è il tipo ".txt" del file
                playersName[i] = str2[0];//assegno all'elemento della posizione in quel momento dell'array il valore dle nome del giocatore
            }
            return playersName;//ritorna il contenuto dell'array
        }
        //controlla se tutti i giocatori hanno formato una squadra
        private static int CheckPlayersSquad(ref bool fileEmpty, ref string[] nomiFantaAllenatori, ref string[] fantaAllenatoriNoSquadra)
        {
            nomiFantaAllenatori = GetPlayersName(nomiFantaAllenatori);//chiama la funzione "GetPlayersName" che ritorna un array contenete come valori i nomi dei giocatori
            int nFantaAllenatoriNoSquadra = 0;//inizializza la variabile che servirà per tener traccia dei giocatori che non hanno una rosa, ovvero neanche un calciatore
            for (int i = 0; i < nomiFantaAllenatori.Length; i++)
            {
                string[] tmp = File.ReadAllLines(mainPath + $"\\Squadre\\{nomiFantaAllenatori[i]}.txt");//legge tutte le righe del file, presente nel percorso assoluto passato per parametro alla funzione
                if (tmp.Length == 0)//nel caso il file non abbia neanche un riga
                {
                    Array.Resize(ref fantaAllenatoriNoSquadra, fantaAllenatoriNoSquadra.Length + 1);//fa un resize dell'array fantaAllenatoriNoSquadra, aumentando il numero di valori che può contenere di 1
                    fantaAllenatoriNoSquadra[fantaAllenatoriNoSquadra.Length - 1] = nomiFantaAllenatori[i];//inserisce nell'ultima posizione possibile dell'array il nome del giocatore che non ha neanche un calciatore nella squadra
                    nFantaAllenatoriNoSquadra++;//aumenta il valore della variabile di 1
                }
            }
            if (nFantaAllenatoriNoSquadra == 0)//controlla se non ci sono fantaallenatori senza un calciatore
            {
                Array.Resize(ref fantaAllenatoriNoSquadra, 0);//ridimensiona l'array impostando il quantitativo di elementi che può contenere a 0
                return 1;//tutti hanno almeno 1 calciatore
            }
            return -1;//almeno un giocatore non ha un calciatore
        }
        private static bool IsDirectoryEmpty(string path)//https://stackoverflow.com/questions/755574/how-to-quickly-check-if-folder-is-empty-net/954837
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();//controlla se nel percorso assoluto passato per parametro ci sono file, e "Any" restituisce un valore booleano true se il controllo restituisce un valore pari a 0 e false in caso contrario
                                                                     //ma ritorna un valore opposto a quel che dovrebbe restituire grazie all'operatore booleano "not"
        }
        //è la funzione principale, che gestisce, il controllo dei file, la loro creazione e chiama le altre funzioni che gestiranno il gioco
        private static void Setup(ref bool fileEmpty, ref string[] nomiFantaAllenatori, ref string[] fantaAllenatoriNoSquadra, ref int[] fantaCrediti)
        {
            if (!Directory.Exists(mainPath + "\\Squadre") || IsDirectoryEmpty(mainPath + "\\Squadre") || CheckPlayersSquad(ref fileEmpty, ref nomiFantaAllenatori, ref fantaAllenatoriNoSquadra) == -1)
            //1 controlla se esiste la directory nel percorso assoluto, passato per parametro, 2 controlla se la directory è vuota, 3 controlla se i file, che dovrebbero contenere la lista della squadra dei giocatori, sono vuoti
            //se una sola di queste condizioni restituisce valore "true" allora significa che bisogna eseguire/ripetere la configurazione iniziale
            {
                if (fantaAllenatoriNoSquadra.Length == 0)//controlla se ci sono giocatori senza una squadra
                {//questa parte di codice viene eseguita quando ci sono fantaallenatori che non hanno una squadra
                    WriteLogs("creazione file per le squadre dei giocatori");//scrive sul file di log che è iniziata la configurazione delle squadre
                    Console.WriteLine($"Inizio configurazione squadre");//avvisa l'utente che comincia la configurazione delle squadre
                    Console.WriteLine("Quanti sono i giocatori?");//chiede agli utenti, il quantitativo di giocatori che vuole partecipare al fantasmagorico Fantacalcio
                    int nPlayer = 0; bool correctSyntax = false;//inizializza le variabili che serviranno per controllare rispettivamente il numero di player e se sono scritti in maniera conforme per la creazione di file su windows
                    CheckPlayersNum(ref nPlayer, ref correctSyntax);//controlla se il numero di giocatori è consono per il gioco
                    Array.Resize(ref nomiFantaAllenatori, nPlayer);//ridimensiona l'array che conterrài nomi dei fantaallenatori con il quantitativo di giocatori presenti
                    if (!Directory.Exists(mainPath + "\\Squadre")) { DirectoryInfo setupFolder = Directory.CreateDirectory(mainPath + "\\Squadre"); WriteLogs("cartella per contenere i file creata"); }
                    //(riferito alla riga di sopra) controlla se non è già presente la cartella per contenere le squadre dei giocatori, in caso quest'ultima non c'è la crea, e scrive nei file dei log quando questa è stata creata
                    CheckPlayersName(ref nomiFantaAllenatori);//controlla che non ci siano nomi uguali tra i giocatori
                    for (int i = 0; i < nPlayer; i++)
                    {
                        nomiFantaAllenatori[i] = RemoveSpecialCharacters(nomiFantaAllenatori[i]);//rende conforme i nomi dei giocatori con il formato richiesto per una creazione dei file di windows
                        using (StreamWriter sw = File.CreateText($"{mainPath}\\Squadre\\{nomiFantaAllenatori[i]}.txt")) { }//crea i file con i nomi, eventualmente puliti, dei giocatori
                    }
                    Console.WriteLine($"Creazione file completata");//avvisa l'utente che le directoy e i file necessari ai giocatori per contenere la loro rosa sono stati creati
                    WriteLogs("file necessari per il gioco creati");//scrive un log, che la creazione dei file e directory principali sono stati creati
                    AssegnazioneFantacrediti(ref nPlayer, ref fantaCrediti);
                    string[] listaCalciatoriDaAcquistare = new string[0];
                    int nCalciatoriUguali = 0;
                    ListaAsta(ref nomiFantaAllenatori, ref fantaCrediti, ref listaCalciatoriDaAcquistare, ref nCalciatoriUguali);
                    WriteLogs("completata la lista dei calciatori per l'asta");
                    Asta(ref nomiFantaAllenatori, ref fantaCrediti, ref listaCalciatoriDaAcquistare, ref nCalciatoriUguali);
                    //permette ai giocatori di acquistare altri calciatori, se eventualmente avevano scelto i medesimi
                    while (nCalciatoriUguali > 0)
                    {
                        string[] playerSquadreIncomplete = SquadreIncomplete(ref nomiFantaAllenatori, ref fantaCrediti);
                        if (playerSquadreIncomplete.Length != 0)
                        {
                            nCalciatoriUguali = 0;
                            Array.Resize(ref listaCalciatoriDaAcquistare, 0);
                            ListaAsta(ref playerSquadreIncomplete, ref fantaCrediti, ref listaCalciatoriDaAcquistare, ref nCalciatoriUguali);
                            Asta(ref playerSquadreIncomplete, ref fantaCrediti, ref listaCalciatoriDaAcquistare, ref nCalciatoriUguali);
                        }
                    }
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
            return Regex.Replace(str, "[^a-zA-Z0-9_]+", "", RegexOptions.Compiled);//ritorna la medesima stringa ricevuta come parametro, ma sostituisce i caratteri non inseriti all'interno del secondo parametro di "Replace" con il niente (sostanzialmente li toglie)
        }
        private static void ListaAsta(ref string[] fantaAllenatori, ref int[] fantaCrediti, ref string[] listaCalciatoriDaAcquistare, ref int nCalciatoriUguali)
        {
            Console.WriteLine("Inizio asta");
            WriteLogs("comicia l'asta dei calciatori");
            string[] ruoliCalciaotirDaAcquistare = new string[] { "PORTIERE", "DIFENSORE", "CENTROCAMPISTA", "ATTACCANTE" };
            byte[] nMaxRuoliClaciatoriDaAcquistare = new byte[] { 1, 4, 4, 3 };
            for (int i = 0; i < fantaAllenatori.Length; i++)
            {
                int nCalciatoriInseriti = 0, nMaxRulo = 0, ruolo = 0;
                do
                {
                    Console.WriteLine($"Giocatore {fantaAllenatori[i]} scrivi il nome del {ruoliCalciaotirDaAcquistare[ruolo]} che vuoi acquistare");
                    string calciatoreDaComprare = Console.ReadLine();
                    if (ControlloEsistenza_RuoloCalciatore(ref calciatoreDaComprare, ruoliCalciaotirDaAcquistare[ruolo].ToLower()))
                    {//esiste il calciatore
                        if (ControlloLista(ref listaCalciatoriDaAcquistare, ref calciatoreDaComprare, ref nCalciatoriUguali))
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
        private static bool ControlloLista(ref string[] listaCalciatoriDaAcquistare, ref string calciatoreDaComprare, ref int nCalciatoriUguali)
        {
            bool calcUguali = false;
            if (listaCalciatoriDaAcquistare.Length == 0)
            {
                return true;
            }
            for (int i = 0; i < listaCalciatoriDaAcquistare.Length; i++)
            {
                if (calciatoreDaComprare == listaCalciatoriDaAcquistare[i])
                {
                    calcUguali = true;
                    nCalciatoriUguali++;//calciatori uguali
                }
            }
            if (!calcUguali)
            {
                return true;
            }
            return false;
        }
        private static void Asta(ref string[] fantaAllenatori, ref int[] fantaCrediti, ref string[] listaCalciatoriDaAcquistare, ref int nCalciatoriUguali)
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
                            AssegnazioneCalciatore(fantaAllenatori, ref astaFinita, offertaAsta, ref fantaCrediti, listaCalciatoriDaAcquistare[i]);
                            WriteLogs($"I giocatori hanno terminato l'asta per un calciatore");
                        }
                        else
                            Console.WriteLine("Prezzo non valido");
                    }
                } while (!astaFinita);
                Console.WriteLine($"L'asta per il calciatore: {risposta} e' conclusa");
            }
            WriteLogs($"L'asta si è completamente conclusa");
            Console.WriteLine($"L'asta si è completamente conclusa");
        }
        //effettua le operazione di acquisto di un calciatore da parte di un giocatore
        private static void AssegnazioneCalciatore(string[] fantaAllenatori, ref bool astaFinita, int offertaAsta, ref int[] fantaCrediti, string calciatore)
        {
            bool nomeCorretto = false;
            do
            {
                Console.WriteLine("\nInserisci il nome dell'utente che ha effettuato l'ultima offerta");
                string nomePlayer = Console.ReadLine();
                for (int i = 0; i < fantaAllenatori.Length; i++)
                {
                    if (nomePlayer == fantaAllenatori[i])
                    {
                        if (fantaCrediti[i] >= offertaAsta)
                        {
                            string[] squadraPlayer = File.ReadAllLines(mainPath + $"\\Squadre\\{fantaAllenatori[i]}.txt");
                            if (squadraPlayer.Length < 11)
                            {
                                Array.Resize(ref squadraPlayer, squadraPlayer.Length + 1);
                                squadraPlayer[squadraPlayer.Length - 1] = calciatore;
                                File.WriteAllLines(mainPath + $"\\Squadre\\{fantaAllenatori[i]}.txt", squadraPlayer);
                                AcquistoCalciatore(calciatore);
                                fantaCrediti[i] -= offertaAsta;
                                nomeCorretto = true;
                                astaFinita = true;
                                WriteLogs($"Il giocatore '{nomePlayer}' ha comprato il seguente calciatore: {calciatore}, alla modica cifra di: {offertaAsta} fantacrediti");
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"{nomePlayer} non puoi più acquistare calciatori in quanto hai già una squadra completa\n");
                                WriteLogs($"Il giocatore '{nomePlayer} ha provato ad acquistare un calciatore anche se però possiede già una squadra al completo, quindi gli è stato annullato l'acquisto");
                                nomeCorretto = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{nomePlayer} non ha abbastanza crediti, rieffettuare l'asta per questo calciatore\n");
                            nomeCorretto = true;
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
        //crea un array con i nomi dei giocatori che hanno una rosa inferiore di 11 claciatori, ma con almeno 1 fantacredito
        private static string[] SquadreIncomplete(ref string[] nomiFantaAllenatori, ref int[] fantaCrediti)
        {
            string[] pSqIm = new string[0];
            for (int i = 0; i < nomiFantaAllenatori.Length; i++)
            {
                string[] tmpSquadre = File.ReadAllLines(mainPath + $"\\Squadre\\{nomiFantaAllenatori[i]}.txt");
                if (tmpSquadre.Length < 11 && fantaCrediti[i] > 0)
                {
                    Array.Resize(ref pSqIm, pSqIm.Length + 1);
                    pSqIm[pSqIm.Length - 1] = nomiFantaAllenatori[i];
                }
            }
            return pSqIm;
        }
    }
}