using System;
using System.IO;
using System.Linq;

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
            Giocatore g = new Giocatore(mainPath);
            Setup(ref fileEmpty, ref nomiFantaAllenatori, ref fantaAllenatoriNoSquadra, ref fantaCrediti, ref g);//chiama la funzione setup e gli passata tutti gli array e variabili precedentemente creati
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
        private static bool IsDirectoryEmpty(string path)//https://stackoverflow.com/questions/755574/how-to-quickly-check-if-folder-is-empty-net/954837
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();//controlla se nel percorso assoluto passato per parametro ci sono file, e "Any" restituisce un valore booleano true se il controllo restituisce un valore pari a 0 e false in caso contrario
                                                                     //ma ritorna un valore opposto a quel che dovrebbe restituire grazie all'operatore booleano "not"
        }
        private static void Setup(ref bool fileEmpty, ref string[] nomiFantaAllenatori, ref string[] fantaAllenatoriNoSquadra, ref int[] fantaCrediti, ref Giocatore g)//è la funzione principale, che gestisce, il controllo dei file, la loro creazione e chiama le altre funzioni che gestiranno il gioco
        {
            if (!Directory.Exists(mainPath + "\\Squadre") || IsDirectoryEmpty(mainPath + "\\Squadre") || g.CheckPlayersSquad(ref fileEmpty, ref nomiFantaAllenatori, ref fantaAllenatoriNoSquadra) == -1)
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
                        nomiFantaAllenatori[i] = g.RemoveSpecialCharacters(nomiFantaAllenatori[i]);//rende conforme i nomi dei giocatori con il formato richiesto per una creazione dei file di windows
                        using (StreamWriter sw = File.CreateText($"{mainPath}\\Squadre\\{nomiFantaAllenatori[i]}.txt")) { }//crea i file con i nomi, eventualmente puliti, dei giocatori
                    }
                    Console.WriteLine($"Creazione file completata");//avvisa l'utente che le directoy e i file necessari ai giocatori per contenere la loro rosa sono stati creati
                    WriteLogs("file necessari per il gioco creati");//scrive un log, che la creazione dei file e directory principali sono stati creati
                    g.AssegnazioneFantacrediti(ref nPlayer, ref fantaCrediti, 500);//imposta ad ogni giocatore i suoi fantacrediti iniziali
                    WriteLogs($"Sono stati assegnati i fantacrediti a tutti i giocatori");//viene scritto una riga di log, che indica la conclusione dell'assegnazione dei fantacrediti
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
            string[] calciatori = File.ReadAllLines(mainPath + "\\Calciatori.txt");//legge tutte le righe del file passato per parametro, ogni riga del file corrisponde ad un elemento dell'array
            for (int i = 0; i < calciatori.Length; i++)//ripete il for per un quantitativo di volte pari alle righe del file
            {
                string[] calciatore = calciatori[i].Split(',');//divide la riga in 4 elementi
                if (calciatoreDaComprare == calciatore[0] && ruolo == calciatore[1])//confronta se il nome/cognome che l'utente ha inserito e il ruolo richiesto corrispondono a quelli di un calciatore esistente
                    return true;//ritorna un valore positivo nel caso in cui queste due condizioni sono verificate contemporaneamente
            }
            return false;//ritorna un valore negativo se, dopo aver confrontato tutti i calciatori, nessuno di questi corrisponde ai valori inseriti
        }
        private static bool ControlloLista(ref string[] listaCalciatoriDaAcquistare, ref string calciatoreDaComprare, ref int nCalciatoriUguali)//controlla se è già presente un calciatore nella lista dell'asta
        {
            bool calcUguali = false;//inizializza con un valore negativo, questa variabile server per dire se ci son stati dei calciatori uguali
            if (listaCalciatoriDaAcquistare.Length == 0)//se non ci sono calciatori da confrontare
            {
                return true;//ritorna un valore positivo, non ha senso confrontare un calciatore, se questoè il primo della lista
            }
            for (int i = 0; i < listaCalciatoriDaAcquistare.Length; i++)//confronta un calciatore con tutta la lista di quelli inseriti fino a quel momento
            {
                if (calciatoreDaComprare == listaCalciatoriDaAcquistare[i])//confronta se i nomi sono identici
                {
                    calcUguali = true;//imposta la variabile a true, quindi è stato trovato ul calciatore già ripetuto
                    nCalciatoriUguali++;//calciatori uguali
                }
            }
            if (!calcUguali)//se viene scansionato tutto l'array e non sono stati trovati calciatori uguali
            {
                return true;//ritorna un valore positivo
            }
            return false;//dopo tutti questi controlli, è stato trovato un calciatore uguale a quello inserito 
        }
        private static void Asta(ref string[] fantaAllenatori, ref int[] fantaCrediti, ref string[] listaCalciatoriDaAcquistare, ref int nCalciatoriUguali)//grestisce la disputa che avviene tra i giocatori, al fine di ottenere un determinato calciatore
        {
            for (int i = 0; i < listaCalciatoriDaAcquistare.Length; i++)//itera per un numero di volte pari ai calciatori messi in vendita, non doppioni
            {
                bool astaFinita = false;//inizializzazione di una variabile, a false; questa servirà per dire al programma quando un asta per un calciatore si è conclusa
                int offertaAsta = 0;//inizializzazione di una variabile int32 con valore pari a 0, serve per tener traccia dei prezzi che i giocatori
                string risposta = "";//inizializzazione di una variabile stringa, con valore ' "" ', questa serve per salvare l'input che l'utente digita da tastiera
                do
                {
                    Console.WriteLine($"Inserisci un offerta per il calciatore: {listaCalciatoriDaAcquistare[i]}");//chiede all'utente un offerta per quel determinato calciatore
                    risposta = Console.ReadLine().ToLower();//riceve in input, ciò che l'utente scrive sa tastiera e lo mette tutto in minuscolo
                    WriteLogs($"è stata inseria un offerta pari a: '{risposta}'");//scrive un log, nel quale si salva l'offerta fatta dall'utente
                    try//qui viene usato un try/catch, per evitare crash del programma se durante la conversione della risposta dell'utente da string a int, questa non è possibile da convertire
                    {
                        int tmp = Convert.ToInt32(risposta);//converte la risposta dell'utente, da string a int32, e ne salva il valore in una variabile temporanea
                        if (tmp > offertaAsta)//confrota l'offerta, e la salva nella variabile "ufficiale", se questa è maggiore dell'offerta fatta in precedenza
                            offertaAsta = tmp;//è maggiore di quella precedente, quindi viene salvata
                        else
                            Console.WriteLine("Devi inserire un prezzo maggiore rispetto all'asta corrente");//è minore o uguale all'offerta precedente, quindi avvisa l'utente che se vuole avanzare un offerta, deve inserire un valore maggiore a quello che aveva proposto
                    }
                    catch//quando la conversione da string a int32 non è possibile
                    {
                        if (risposta == "exitasta")//in questo, caso, l'asta si chiude e si procede al completamento dei dettagli per l'acquisto del calciatore
                        {
                            AssegnazioneCalciatore(fantaAllenatori, ref astaFinita, offertaAsta, ref fantaCrediti, listaCalciatoriDaAcquistare[i]);//chiama la funzione "AssegnazioneCalciatore e gli passa diversi parametri
                            WriteLogs($"I giocatori hanno terminato l'asta per un calciatore");//scrive un log, riguardo al fatto che un asta è terminata per un calciatore
                        }
                        else//caso in cui quello che l'utente ha inserito, sono pagliacciate o cavolate
                            Console.WriteLine("Prezzo non valido");//avvisa l'utente di quel che ha scritto
                    }
                } while (!astaFinita);//il ciclo do/while non smette finchè la variabile "astaFinita" non diventa true
                Console.WriteLine($"L'asta per il calciatore: {risposta} e' conclusa");//avvisa l'utente, che l'asta per un calciatore si è conclusa
            }
            WriteLogs($"L'asta si è completamente conclusa");//salva un log, riguardo al fatto che l'asta si è COMPLETAMENTE conclusa
            Console.WriteLine($"L'asta si è completamente conclusa");//scrive a schermo che l'asta è finita
        }
        private static void AssegnazioneCalciatore(string[] fantaAllenatori, ref bool astaFinita, int offertaAsta, ref int[] fantaCrediti, string calciatore)//effettua le operazione di acquisto di un calciatore da parte di un giocatore
        {
            bool nomeCorretto = false;//per vedere se un il nome utente inserito è corretto
            do
            {
                Console.WriteLine("\nInserisci il nome dell'utente che ha effettuato l'ultima offerta");//avvisa gli utenti di inseririe il nome dell'ultimo giocatore che ha fatto un offerta
                string nomePlayer = Console.ReadLine();//prende in input il risultato
                for (int i = 0; i < fantaAllenatori.Length; i++)//itera un numero di volte pari al quantitativo di plaer esistenti
                {
                    if (nomePlayer == fantaAllenatori[i])//verifica se c'è il giocatore
                    {
                        nomeCorretto = true;//il nome inserito è giusto
                        if (fantaCrediti[i] >= offertaAsta)//verifica se questo ha abbastanza soldi
                        {
                            string[] squadraPlayer = File.ReadAllLines(mainPath + $"\\Squadre\\{fantaAllenatori[i]}.txt");//preleva la squadra del giocatore
                            if (squadraPlayer.Length < 11)//controlla che questo abbia una squadra composta da meno di 11 calciatori
                            {//nel caso in cui la sua rosa è composta da meno di 11 calciatori
                                Array.Resize(ref squadraPlayer, squadraPlayer.Length + 1);//ridimensiona l'array, aggiungendo uno spazio
                                squadraPlayer[squadraPlayer.Length - 1] = calciatore;//inserisce nell'ultimo spazio appena creato il nome del calciatore che ha acquistato
                                File.WriteAllLines(mainPath + $"\\Squadre\\{fantaAllenatori[i]}.txt", squadraPlayer);//salva nel file la squadra aggioranta
                                AcquistoCalciatore(calciatore);//chiama la funzione "AcquistoCalciatore" e gli passa per paraemtro il nome del calciatore
                                fantaCrediti[i] -= offertaAsta;//gli toglie un quantitativo di soldi in conformità alla cifra spesa per comprare il calciatore
                                astaFinita = true;//l'asta per quel calciatore si è conclusa
                                WriteLogs($"Il giocatore '{nomePlayer}' ha comprato il seguente calciatore: {calciatore}, alla modica cifra di: {offertaAsta} fantacrediti");//salva un log, dove scrive che un giocatore ha comprato un calciatore, con nome di entrambi di questi
                                break;//ferma il ciclo for
                            }
                            else
                            {
                                Console.WriteLine($"{nomePlayer} non puoi più acquistare calciatori in quanto hai già una squadra completa\n");//avverte l'utente che non può comprare altri calciatori visto che possiede una squadra al completo
                                WriteLogs($"Il giocatore '{nomePlayer} ha provato ad acquistare un calciatore anche se però possiede già una squadra al completo, quindi gli è stato annullato l'acquisto");//salva un log, riguardando alla cancellazione dell'acquisto del calciatore, per mancanza di posti in squadra
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{nomePlayer} non ha abbastanza crediti, rieffettuare l'asta per questo calciatore\n");//dice che l'utente non ha abbastanza soldi per pagare l'offerta per il calciatore
                            WriteLogs($"Il giocatore '{nomePlayer}' voleva acquistare il calciatore '{calciatore}', ma purtroppo non aveva abbastanza fantacrediti");//scrive un log, che tratta della cancellazione dell'acquisto per mancaza di soldi
                        }
                    }
                }
            } while (!nomeCorretto);//il ciclo termina solo quando è stato inserito un nome di un giocatore corretto
        }
        private static void AcquistoCalciatore(string calciatore)//rende impossbile l'eventuale tentativo di riacquisto di un calciatore
        {
            string[] calciatori = File.ReadAllLines(mainPath + "\\Calciatori.txt");//preleva la lista die calciatori
            for (int i = 0; i < calciatori.Length; i++)//itera tutta la lista
            {
                string[] tmp = calciatori[i].Split(',');//salva in un array i vari elementi che se ne ricavano, dividendo l'elemento dell'array "calciatori" per il carattere ','
                if (calciatore == tmp[0])//controlla se si trova nella riga dei dettagli del calciatore che la funzione riceve per parametro
                {
                    tmp[3] = "true";//cambia lo stato di acquisto del calciatore    false --> non ancora comprato   true --> comprato
                    calciatori[i] = $"{tmp[0]},{tmp[1]},{tmp[2]},{tmp[3]}";//ricompone la riga
                    File.WriteAllLines(mainPath + "\\Calciatori.txt", calciatori);//salva l'array modificato nel file
                    break;//ferma il for, visto che ha trovato già il calciatore necessario e quindi risulta inutile continuare a scorrere l'array
                }
            }
        }
        private static string[] SquadreIncomplete(ref string[] nomiFantaAllenatori, ref int[] fantaCrediti)//crea un array con i nomi dei giocatori che hanno una rosa inferiore di 11 claciatori, ma con almeno 1 fantacredito
        {
            string[] pSqIm = new string[0];//crea l'array, per salvare i nomi di coloro che non hanno nemmeno un calciatore nella loro rosa
            for (int i = 0; i < nomiFantaAllenatori.Length; i++)//itera l'array dei giocatori
            {
                string[] tmpSquadre = File.ReadAllLines(mainPath + $"\\Squadre\\{nomiFantaAllenatori[i]}.txt");//prende la squadra dei rispettivi calciatori
                if (tmpSquadre.Length < 11 && fantaCrediti[i] > 0)//controlla che questi abbiamo almeno un calciatore
                {
                    Array.Resize(ref pSqIm, pSqIm.Length + 1);//aumenta la dimensione dell'array passato per parametro di 1
                    pSqIm[pSqIm.Length - 1] = nomiFantaAllenatori[i];//aggiunge il nome del giocatore
                }
            }
            return pSqIm;
        }
    }
}