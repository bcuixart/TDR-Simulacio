using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipusNotificacio { IniciSimulacio, FinalSimulacioArbitrari, FinalSimulacioExtincioMassiva, FinalSimulacioTempsMaxim, FinalSimulacioSenseGuardar, PrimerInfectat, Extincio}
[System.Serializable]
public class Notificacio
{
    public TipusNotificacio tipus;

    public string text;

    public Notificacio(TipusNotificacio _tipus, float temps, string parametreSecundariText, int parametreSecundariNombre)
    {
        tipus = _tipus;

        int hores = Mathf.FloorToInt(temps / 3600f);
        int minuts = Mathf.FloorToInt((temps - 3600 * hores) / 60f);
        int segons = (int)temps - 3600 * hores - 60 * minuts;

        string tempsS = (hores > 0) ? hores.ToString("00") + ":" + minuts.ToString("00") + ":" + segons.ToString("00") : minuts.ToString("00") + ":" + segons.ToString("00");

        switch (tipus)
        {
            case TipusNotificacio.IniciSimulacio:
                text = "00:00 - Inicia la simulació.";
                break;
            case TipusNotificacio.FinalSimulacioArbitrari:
                text = tempsS + " - L'usuari decideix finalitzar la simulació.";
                break;
            case TipusNotificacio.FinalSimulacioExtincioMassiva:
                string completadorTextFSEM = (parametreSecundariNombre == 0) ? "un " : "una " ;
                text = tempsS + " - Mor l'últim individu, " + completadorTextFSEM + parametreSecundariText + "; la simulació s'acaba.";
                break;
            case TipusNotificacio.FinalSimulacioTempsMaxim:
                text = tempsS + " - La simulació arriba al temps màxim establert per l'usuari.";
                break;
            case TipusNotificacio.FinalSimulacioSenseGuardar:
                text = "L'usuari decideix que aquesta simulació no mereix ser desada i l'envia a l'oblit etern.";
                break;
            case TipusNotificacio.PrimerInfectat:
                string completadorTextPI = (parametreSecundariNombre == 0) ? "el primer " : "la primera ";
                text = tempsS + " - S'infecta " + completadorTextPI + parametreSecundariText + ".";
                break;
            case TipusNotificacio.Extincio:
                string completadorTextE = (parametreSecundariNombre == 0) ? "els " : "les ";
                text = tempsS + " - S'extingeixen " + completadorTextE + parametreSecundariText + ".";
                break;
        }

        return;
    }
}
