using System;
using System.IO;

namespace Fantacalcio
{
    class Calciatore
    {
        string mainPath = "";//inizializza una string dove si conterrà il percorso principale del fantacalcio
        public Calciatore(string path)
        {
            this.mainPath = path;//imposta il valore della variabile "mainPath" dell'istanza corrente con quello che è stato passato per parametro alla funzione
        }
        public string[] GetSquadra(ref string[] nomiFantaAllenatori)//ottiene i nomi dei calciatori di ogni giocatore
        {
            string[] squadre = new string[nomiFantaAllenatori.Length];
            for (int i = 0; i < nomiFantaAllenatori.Length; i++)
            {
                string[] tmp = File.ReadAllLines(mainPath + $"\\Squadre\\{nomiFantaAllenatori[i]}.txt");
                squadre[i] = tmp[0];
                for (int j = 1; j < tmp.Length; j++)
                    squadre[i] += $",{tmp[j]}";
            }
            return squadre;
        }
    }
}