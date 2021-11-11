using System.IO;

namespace Fantacalcio
{
    class Calciatore
    {
        string mainPath = "";//inizializza un attributo string, dove si conterrà il percorso principale del fantacalcio
        public Calciatore(string path)//costruttore, bisogna passarli per parametro un stringa
        {
            this.mainPath = path;//imposta il valore della variabile "mainPath" dell'istanza corrente con quello che è stato passato per parametro alla funzione
        }
        public string[] GetSquadra(ref string[] nomiFantaAllenatori)//ottiene i nomi dei calciatori di ogni giocatore
        {
            string[] squadre = new string[nomiFantaAllenatori.Length];//crea un'array per salvare la squadra dei giocatori
            for (int i = 0; i < nomiFantaAllenatori.Length; i++)//itera per tutti i fantaallenatori
            {
                string[] tmp = File.ReadAllLines(mainPath + $"\\Squadre\\{nomiFantaAllenatori[i]}.txt");//prende in input la squadra del singolo giocatore
                squadre[i] = tmp[0];//salva il primo elemento, il primo calciatore del fantaallenatori
                for (int j = 1; j < tmp.Length; j++)//itera per tutta la squadra
                    squadre[i] += $",{tmp[j]}";//aggiunge il calciatore alla singola stringa, separandolo con una virgola, dal precedente
            }
            return squadre;//ritorna l'array delle squadre
        }
    }
}