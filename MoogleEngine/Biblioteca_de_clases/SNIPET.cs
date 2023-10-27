namespace MoogleEngine;
public class Snipet
{
  public static string[] texto(string query)
    {
        string[] snipet = new string[Textos.documentos.Length];
        string[] palabraMaxTFIDF = new string[snipet.Length];
        string[] palabrasquery = query.ToLower().Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace("!", "").Replace("?", "").Replace("(", "").Replace(")", "").Replace("*", "").Replace("^", "").Split(' ');
        for (int i = 0; i < palabraMaxTFIDF.Length; i++)
        {
            double mayor = 0;
            palabraMaxTFIDF[i] = "";
            for (int j = 0; j < palabrasquery.Length; j++)
            {
                if (Lista.Ayuda_Para_Snipet.ContainsKey(palabrasquery[j]))
                {
                    int posicion = Lista.Ayuda_Para_Snipet[palabrasquery[j]];
                    double temporal = Matrix.TF_IDF[i, posicion];
                    if (mayor < temporal)
                    {
                        mayor = temporal;
                        palabraMaxTFIDF[i] = palabrasquery[j];
                    }
                }

            }
        }
        for (int i = 0; i < snipet.Length; i++)
        {
            snipet[i] = "";
            Dictionary<string, int> candela = Matrix.Posiciones_De_Palabras[i];
            string[] parrafito = new string[82];
            if (candela.ContainsKey(palabraMaxTFIDF[i]) && palabraMaxTFIDF[i] != "")
            {
                int posicion = candela[palabraMaxTFIDF[i]];
                if (posicion >= 40 && (posicion + 40) <= candela.Count)
                {
                    int z = 0;
                    for (int j = posicion - 40; j < posicion + 40; j++)
                    {
                        parrafito[z] = Matrix.Palabras[i, j];
                        z++;
                    }
                }
                else if (posicion < 80 && (posicion + 80) <= candela.Count)
                {
                    int z = 0;
                    for (int j = posicion; j < posicion + 80; j++)
                    {
                        parrafito[z] =  Matrix.Palabras[i, j];
                        z++;
                    }
                }
                else if (posicion >= 80 && (posicion + 80) > candela.Count)
                {
                    int z = 0;
                    for (int j = posicion - 78; j <= posicion; j++)
                    {
                        parrafito[z] = Matrix.Palabras[i, j];
                        z++;
                    }
                }
                else if (posicion < 80 && (posicion + 80) > candela.Count)
                {
                    int z = 0;
                    int resta = candela.Count - posicion;
                    for (int j = posicion; j < resta; j++)
                    {
                        parrafito[z] = Matrix.Palabras[i, j];
                        z++;
                    }
                }
                string unido = string.Join(" ", parrafito);
                snipet[i] = unido;
            }
        }
        return snipet;
    }
}