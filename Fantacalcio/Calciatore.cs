using System;
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
        public bool ControlloCalciatore(ref string calciatoreDaComprare, string ruolo, ref string[] calciatori, bool checkRuolo)//controlla se l'utente ha inserito il nome di un calciatore valido e se ha il ruolo richiesto, il valore "true" serve ad indicare che il programma deve controllare anche il ruolo, se questo è false, controlla solo il nome
        {
            for (int i = 0; i < calciatori.Length; i++)//ripete il for per un quantitativo di volte pari alle righe del file
            {
                string[] calciatore = calciatori[i].Split(',');//divide la riga in 4 elementi
                if (calciatoreDaComprare == calciatore[0] && ruolo == calciatore[1] && checkRuolo == true)//confronta se il nome/cognome che l'utente ha inserito e il ruolo richiesto corrispondono a quelli di un calciatore esistente
                    return true;//ritorna un valore positivo nel caso in cui queste due condizioni sono verificate contemporaneamente
                else if (calciatoreDaComprare == calciatore[0] && checkRuolo == false)
                    return true;
            }
            return false;//ritorna un valore negativo se, dopo aver confrontato tutti i calciatori, nessuno di questi corrisponde ai valori inseriti
        }
        public bool ComandiCalciatore(ref string[] fantaAllenatori, ref string[] comands, ref string[] listaCalciatori, ref string[] squadrePlayer, ref string[] punteggi)//a seconda del comando scritto dall'utente, che inizia per "rgd", esegue diverse funzioni, comandi inerenti al punteggio dei giocatori
        {
            string azione, quantitativo, calciatore;//inizializza le variabili
            azione = comands[1];//passa alla variabile il valore dell'azione inserita dall'utente
            if (azione != "-ibt" || azione != "-e")//controlla le azioni del giocatore
            {//non indica nè imbattibilità nè espulsione
                quantitativo = comands[2];//n di goal/parate/ecc... fatte dal calciatore
                calciatore = $"{comands[3]} {comands[4]}";//nome e cognome del calciatore
            }
            else
            {//indica imbattibilità o espulsione
                quantitativo = null;
                calciatore = $"{comands[2]} {comands[3]}";//nome e cognome del calciatore
            }
            if (ControlloCalciatore(ref calciatore, "non serve", ref listaCalciatori, false))//controlla se il calciatore esiste
            {
                int allenatore = GetFantaAllenatoreProprietario(ref fantaAllenatori, ref squadrePlayer, ref calciatore);//ottiene il nome del fantaallenatore
                if (allenatore != -1)//quando è stato trovato il fantaallenatore
                {
                    double punti = 0;//inizializza la variabile per tener traccia delle prestazioni dei calciatori
                    AzioniCalciatore(ref azione, ref quantitativo, ref fantaAllenatori, ref punti);
                    punteggi[allenatore] = (Convert.ToDouble(punteggi[allenatore]) + punti).ToString();
                    return true;//azioni registrate
                }
            }
            return false;//azioni non registrate
        }
        private int GetFantaAllenatoreProprietario(ref string[] fantaAllenatori, ref string[] squadrePlayer, ref string calciatore)//trova il fantaallenatore che ha un determinato calciatore, quello passato per parametro
        {
            for (int i = 0; i < squadrePlayer.Length; i++)//itera per tutte le squadre
            {
                string[] tmp = squadrePlayer[i].Split(',');//divide la squadra in un altro array
                for (int j = 0; j < tmp.Length; j++)//itera per tutta la squadra
                    if (tmp[j] == calciatore) { return i; }//ritorna il nome del fantaallenatore che ha il calciatore nella propria rosa
            }
            return -1;//non trova il proprietario del calciatore
        }
        private void AzioniCalciatore(ref string azione, ref string quantitativo, ref string[] fantaAllenatori, ref double punti)//aggiorna i punteggi in base alle prestazioni dei giocatori
        {
            switch (azione)
            {//controlla l'azione fatta dal calciatore
                case "-g"://goal fatto
                    punti = 3 * Convert.ToInt32(quantitativo);//aggiunge 3 punti per goal fatto
                    break;//interrompe lo switch
                case "-a"://assist
                    punti = Convert.ToInt32(quantitativo);//aggiunge punti proporzionato al numero di assist
                    break;//interrompe lo switch
                case "-ibt"://imbattibilità portiere
                    punti++;//aggiunge 1 punto
                    break;//interrompe lo switch
                case "-rp"://rigore parato
                    punti = 3 * Convert.ToInt32(quantitativo);//aggiunge 3 punti per rigore segnato
                    break;//interrompe lo switch
                case "-gs"://goal subito
                    punti = -1 * Convert.ToInt32(quantitativo);//sottrae 1 punto per goal subito
                    break;//interrompe lo switch
                case "-e"://espulsione
                    punti--;//sottrae 1 punto
                    break;//interrompe lo switch
                case "-am"://ammonizione
                    punti = -0.5;//sottrae mezzo punto
                    break;//interrompe lo switch
                case "-rs"://rigore subito
                    punti = -3 * Convert.ToInt32(quantitativo);//sottrae 3 punti per rigore subito
                    break;//interrompe lo switch
                case "-au"://autogoal
                    punti = -2 * Convert.ToInt32(quantitativo);//sottrae 2 punti per autogoal
                    break;//interrompe lo switch
            }
        }
    }
}