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
        private static string CleanPath(string path)//pulisce il percorso del programma
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
        private static void ExistLogs()//controlla se esiste il file dei logs
        {
            if (!File.Exists(mainPath + "\\logs.txt"))//controlla se nel percorso assoluto passata per parametro esiste il file (il nome e tipo del file sono compresi nel percorso assoluto)
            {
                File.Create(mainPath + "\\logs.txt").Dispose();//crea il file nel percorso assoluto passato per parametro e rilascia le risorse del file
                Console.WriteLine("logs creati");//avvisa l'utente che i file di log sono stati creati
            }
        }
        private static void WriteLogs(string log)//scrive un log di quel che succede all'interno del programma
        {
            File.AppendAllText(mainPath + "\\logs.txt", $"{DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss]")} {log}" + Environment.NewLine);//aggiunge una stringa alla fine del file, con tanto di "newline"
        }
        private static string[] GetPlayersName(string[] playersName)//ottiene i nomi dei FantaAllenatori
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
        private static int CheckPlayersSquad(ref bool fileEmpty, ref string[] nomiFantaAllenatori, ref string[] fantaAllenatoriNoSquadra)//controlla se tutti i giocatori hanno formato una squadra
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
        private static void Setup(ref bool fileEmpty, ref string[] nomiFantaAllenatori, ref string[] fantaAllenatoriNoSquadra, ref int[] fantaCrediti)//è la funzione principale, che gestisce, il controllo dei file, la loro creazione e chiama le altre funzioni che gestiranno il gioco
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
                    AssegnazioneFantacrediti(ref nPlayer, ref fantaCrediti);//imposta ad ogni giocatore i suoi fantacrediti iniziali
                    string[] listaCalciatoriDaAcquistare = new string[0];//crea l'array che conterrà tutti i nomi dei calciatori che verranno comprati dai giocatori
                    int nCalciatoriUguali = 0;//inizializza la variabile che terrà conto del numero, eventuale, di calcicatori uguali, richiesti durante l'asta
                    ListaAsta(ref nomiFantaAllenatori, ref fantaCrediti, ref listaCalciatoriDaAcquistare, ref nCalciatoriUguali);//si occupa di popolare l'array con i nomi dei calciatori da comprare
                    WriteLogs("completata la lista dei calciatori per l'asta");//scrive nel fiel di log che è stata completata la lista dei calciatori da acquistare
                    Asta(ref nomiFantaAllenatori, ref fantaCrediti, ref listaCalciatoriDaAcquistare, ref nCalciatoriUguali);//gestisce tutt l'asta dei giocatori
                    while (nCalciatoriUguali > 0)//permette ai giocatori di acquistare altri calciatori, se eventualmente avevano scelto i medesimi
                    {
                        string[] playerSquadreIncomplete = SquadreIncomplete(ref nomiFantaAllenatori, ref fantaCrediti);//inizializza l'array con i nomi dei fantaallenatori che non hanno una rosa completa, ma che hanno un minimo di fantacrediti
                        if (playerSquadreIncomplete.Length != 0)//controlla se ci sono giocatori senza rosa completa
                        {
                            nCalciatoriUguali = 0;//imposta il valore dei calciatori uguali a 0
                            Array.Resize(ref listaCalciatoriDaAcquistare, 0);//ridimensiona l'array a 0
                            ListaAsta(ref playerSquadreIncomplete, ref fantaCrediti, ref listaCalciatoriDaAcquistare, ref nCalciatoriUguali);
                            Asta(ref playerSquadreIncomplete, ref fantaCrediti, ref listaCalciatoriDaAcquistare, ref nCalciatoriUguali);
                        }
                        else
                            nCalciatoriUguali = 0;//nel caso, in cui ci sono giocatoir senza rosa completa ma non hanno crediti a sufficienza
                    }
                }
                else { }//vado a riprendere singolarmente ogni giocatore finchè non hanno almeno un calciatore nella loro squadra
            }
            else { }//quando ci sono tutti i file necessari e tutti i fanta-allenatori hanno almeno una rosa decente
        }
        private static void AssegnazioneFantacrediti(ref int nPlayer, ref int[] fantaCrediti)//assegna i fantacreditia a tutti i giocatori
        {
            for (int i = 0; i < nPlayer; i++)//ripete per tutti i giocatori
            {
                Array.Resize(ref fantaCrediti, fantaCrediti.Length + 1);//ridimensiona l'array, aumentando la grandezza di questo di 1
                fantaCrediti[i] = 500;//assegna al giocatore i suoi fantacrediti
            }
            WriteLogs($"Sono stati assegnati i fantacrediti a tutti i giocatori");//viene scritto una riga di log, che indica la conclusione dell'assegnazione dei fantacrediti
        }
        private static void CheckPlayersNum(ref int nPlayer, ref bool correctSyntax)//verifica che venga inserito un numero idoneo di giocatori
        {
            do
            {
                correctSyntax = int.TryParse(Console.ReadLine(), out nPlayer);//prende in input da tastiera, lo converte in int32 e se la conversione ha successo restituisce un valore true
                if (!correctSyntax)//se l'utente non inscerisce un numero
                {
                    Console.WriteLine("Inserisci un valore valido");//avvisa l'utente che la prossima volta, deve inserire un valore accettabile dal programma (int32)
                }
                else if (nPlayer <= 1 || nPlayer > 56)//caso in cui, l'utente ha inserito un numero in input ma questo è minore di 1 o maggiore di 56
                {//questo controllo c'è perchè in questo programma solo un giocatore può avere un determinato calciatore, quindi, se nella seria A ci sono 618 calciatori, al fine di avere tutte squadre con calciatori non doppi, sono possibili un numero massimo di 56 giocatori
                    Console.WriteLine("Inserisci un numero maggiore di 1 e minore di 57");//avvisa l'utente riguardo al valore che deve inserire
                }
            } while (!correctSyntax || nPlayer <= 1 || nPlayer > 56);//il ciclo finisce se l'utente inserisce un numero ed è un valore compreso tra 1 e 57 (esclusi)
        }
        private static void CheckPlayersName(ref string[] nomiFantaAllenatori)//popola l'array con i nomi dei giocatori, e verifica se sono presenti nomi uguali
        {
            for (int i = 0; i < nomiFantaAllenatori.Length; i++)
            {
                Console.WriteLine($"Scrivi il nome del giocatore n°{i + 1}");//avvisa un utente di inserire il suo nome
                nomiFantaAllenatori[i] = Console.ReadLine();//prende in input ciò che l'utente digita da tastiera
                if (CheckSameName(ref nomiFantaAllenatori, i))//controlla se il nome che l'utente ha inserito è già stato precedentemente registrato, da parte di un altra persona
                {
                    i--;//diminuisce i, per far ricominciare
                    Console.WriteLine("Nome già registrato, inseriscine un altro");//avvisa l'utente che il nome che ha digitato è già stato registrato
                }
            }
        }
        private static bool CheckSameName(ref string[] nomiFantaAllenatori, int nNomiInseriti)//ispeziona tutto l'array per vedere se esiste già il giocatore che l'utente ha inserito
        {
            for (int i = 0; i < nNomiInseriti; i++)//ripete il ciclo un numero di volte pari agli utenti che si sono registrati
            {
                if (nomiFantaAllenatori[nNomiInseriti] == nomiFantaAllenatori[i])//controlla se il nome inserito è già stato scelto
                {
                    return true;//il nome è già stato scelto
                }
            }
            return false;//nessuno ha ancora digitato quel nome
        }
        private static string RemoveSpecialCharacters(string str)//https://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_]+", "");//ritorna la medesima stringa ricevuta come parametro, ma sostituisce i caratteri non inseriti all'interno del secondo parametro di "Replace" con il niente (sostanzialmente li toglie)
        }
        //RegexOptions.Compiled
        //il "Compiled" specifica che l'espressione regolare viene compilata in un assembly, ciò consente un'esecuzione più rapida ma aumenta il tempo di avvio.
        private static void ListaAsta(ref string[] fantaAllenatori, ref int[] fantaCrediti, ref string[] listaCalciatoriDaAcquistare, ref int nCalciatoriUguali)//gestisce tutti i nomi che vengono inseriti da parte degli utenti, questi sono i calciatori che i fantaallenatori vorrebberpo acquistare
        {//la funzione ListaAsta si occupa di creare la lista finale di tutti i quanti i calciatori, non doppi, che i giocatori acquisteranno
            Console.WriteLine("Inizio asta");//avvisa gli utenti che comincerà l'asta
            WriteLogs("comicia l'asta dei calciatori");//viene scritto un log che segna l'inizio dell'asta
            string[] ruoliCalciaotirDaAcquistare = new string[] { "PORTIERE", "DIFENSORE", "CENTROCAMPISTA", "ATTACCANTE" };//viene inizializzato un array che contiene i vari ruoli che i calciatori possono avere
            byte[] nMaxRuoliClaciatoriDaAcquistare = new byte[] { 1, 4, 4, 2 };//numero massimo di calciatori per ruolo che un utente può acquistare, in corrispondenza dell'array "ruoliCalciaotirDaAcquistare"
            for (int i = 0; i < fantaAllenatori.Length; i++)//ripete il ciclo per tutti i giocatori
            {
                int nCalciatoriInseriti = 0, nMaxRulo = 0, ruolo = 0;//inizializzo le variabili
                do
                {
                    Console.WriteLine($"Giocatore {fantaAllenatori[i]} scrivi il nome del {ruoliCalciaotirDaAcquistare[ruolo]} che vuoi acquistare");//chiede all'utente un calciatore di un determinato ruolo, da inserire
                    string calciatoreDaComprare = Console.ReadLine();//prende in input ciò che l'utente inserisce da tastiera
                    if (ControlloEsistenza_RuoloCalciatore(ref calciatoreDaComprare, ruoliCalciaotirDaAcquistare[ruolo].ToLower()))//controlla che il claciatore che l'utente ha inserito esiste ed è del ruolo richiesto
                    {//esiste il calciatore ed è del ruolo richiesto
                        if (ControlloLista(ref listaCalciatoriDaAcquistare, ref calciatoreDaComprare, ref nCalciatoriUguali))//controlla se un determinato calciatore che l'utente ha inserito è già stato scelto da qualcuno
                        {//il calciatore non è stato ancora scelto
                            Array.Resize(ref listaCalciatoriDaAcquistare, listaCalciatoriDaAcquistare.Length + 1);//si ridimensiona l'array "listaCalciatoriDaAcquistare" , aumentando la sua grandezza di 1
                            listaCalciatoriDaAcquistare[listaCalciatoriDaAcquistare.Length - 1] = calciatoreDaComprare;//inserisce nell'ultima posizione dell'array, possibile, il nome del calciatore che uno o più giocatori vorrebbero acquistare
                        }
                        nCalciatoriInseriti++; nMaxRulo++;//aumento il numero di calciatori inseriti e il ruolo di quel tipo di calciatore di 1
                        if (nMaxRulo == nMaxRuoliClaciatoriDaAcquistare[ruolo])//viene controllato se sono stati inseriti il numero massimo di calciatori per quel determinato ruolo
                        {
                            ruolo++;//aumenta il segna posto dell'array dei ruoli
                            nMaxRulo = 0;//azzera la variabile che tine conto del numero di calciatori per ruolo inseriti
                        }
                    }
                    else
                    {//il calciatore non esiste o non è del ruolo richiesto
                        Console.WriteLine("Non esiste il calciatore o non è del ruolo richiesto\n");//avvisa l'utente che il calciatore che ha inserito non esiste o non è del ruolo che il programma richiede di inserire
                    }
                } while (nCalciatoriInseriti < 11);//finisce il ciclo per un utente quando inserisce 11 calciatori
            }
        }
        private static bool ControlloEsistenza_RuoloCalciatore(ref string calciatoreDaComprare, string ruolo)//controlla se l'utente ha inserito il nome di un calciatore valido e se ha il ruolo richiesto
        {
            string[] calciatori = File.ReadAllLines(mainPath + "\\Calciatori.txt");//legge tutte le righe del file passato per parametro, ogni riga del file corrisponde ad 
            for (int i = 0; i < calciatori.Length; i++)
            {
                string[] calciatore = calciatori[i].Split(',');
                if (calciatoreDaComprare == calciatore[0] && ruolo == calciatore[1])
                    return true;
            }
            return false;
        }
        private static bool ControlloLista(ref string[] listaCalciatoriDaAcquistare, ref string calciatoreDaComprare, ref int nCalciatoriUguali)//controlla se è già presente un calciatore nella lista dell'asta
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
        private static void AssegnazioneCalciatore(string[] fantaAllenatori, ref bool astaFinita, int offertaAsta, ref int[] fantaCrediti, string calciatore)//effettua le operazione di acquisto di un calciatore da parte di un giocatore
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
        private static void AcquistoCalciatore(string calciatore)//rende impossbile l'eventuale tentativo di riacquisto di un calciatore
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
        private static string[] SquadreIncomplete(ref string[] nomiFantaAllenatori, ref int[] fantaCrediti)//crea un array con i nomi dei giocatori che hanno una rosa inferiore di 11 claciatori, ma con almeno 1 fantacredito
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