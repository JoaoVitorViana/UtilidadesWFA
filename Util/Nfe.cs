using System;

namespace UtilidadesWFA.Util
{
    public class Nfe
    {
        public static double RetornaValorFreteCalculado(double pPeso, double pValor)
        {
            var toneladas = pPeso / 1000;
            toneladas = Math.Round(toneladas, MidpointRounding.AwayFromZero);
            if (pPeso > 0 && toneladas == 0)
                toneladas = 1;
            return toneladas * pValor;
        }
    }
}