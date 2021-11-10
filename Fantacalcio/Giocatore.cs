using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Fantacalcio
{
    public class Giocatore
    {
        string mainPath = "";
        public Giocatore(string path)
        {
            this.mainPath = path;
        }
        public int CheckPlayersSquad(ref bool fileEmpty, ref string[] nomiFantaAllenatori, ref string[] fantaAllenatoriNoSquadra)//controlla se tutti i giocatori hanno formato una squadra
        {
            nomiFantaAllenatori = GetPlayersName(nomiFantaAllenatori);//chiama la funzione "GetPlayersName" che ritorna un array contenete come valori i nomi dei giocatori
            int nFantaAllenatoriNoSquadra = 0;//inizializza la variabile che servirà per tener traccia dei giocatori che non hanno una rosa, ovvero neanche un calciatore
            for (int i = 0; i < nomiFantaAllenatori.Length; i++)
            {
                string[] tmp = File.ReadAllLines(this.mainPath + $"\\Squadre\\{nomiFantaAllenatori[i]}.txt");//legge tutte le righe del file, presente nel percorso assoluto passato per parametro alla funzione
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
        private string[] GetPlayersName(string[] playersName)//ottiene i nomi dei FantaAllenatori
        {
            playersName = Directory.GetFileSystemEntries(this.mainPath + "\\Squadre");//ottiene i percorsi relativi dei file contenuti nel percorso assoluto passato per parametro
            for (int i = 0; i < playersName.Length; i++)
            {
                string[] str = playersName[i].Split("Squadre\\");//splitta l'array in due elementi, 1 è "Squadre\\" il secondo è il nome del file e il suo tipo
                string[] str2 = str[1].Split(".txt");//splitto di nuovo l'array, dove il primo elemento è il nome del file, il secondo è il tipo ".txt" del file
                playersName[i] = str2[0];//assegno all'elemento della posizione in quel momento dell'array il valore dle nome del giocatore
            }
            return playersName;//ritorna il contenuto dell'array
        }
        public string RemoveSpecialCharacters(string str)//https://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_]+", "");//ritorna la medesima stringa ricevuta come parametro, ma sostituisce i caratteri non inseriti all'interno del secondo parametro di "Replace" con il niente (sostanzialmente li toglie)
        }
        //RegexOptions.Compiled
        //il "Compiled" specifica che l'espressione regolare viene compilata in un assembly, ciò consente un'esecuzione più rapida ma aumenta il tempo di avvio.
        public void AssegnazioneFantacrediti(ref int nPlayer, ref int[] fantaCrediti, int soldi)//assegna i fantacreditia a tutti i giocatori
        {
            for (int i = 0; i < nPlayer; i++)//ripete per tutti i giocatori
            {
                Array.Resize(ref fantaCrediti, fantaCrediti.Length + 1);//ridimensiona l'array, aumentando la grandezza di questo di 1
                fantaCrediti[i] = soldi;//assegna al giocatore i suoi fantacrediti
            }
        }
    }
}