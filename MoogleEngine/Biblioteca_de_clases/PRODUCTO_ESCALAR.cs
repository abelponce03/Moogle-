namespace MoogleEngine;
public class Producto_Escalar
{
    public static double[] calcular(string query)
    {
        double[] idf = new double[Lista.Palabras.Count];
        //calcular idf que no es mas que la cantidad de documentos entre la aparicion de la palabra en
        //el conjunto de documentos
        for (int i = 0; i < Matrix.TF_IDF.GetLength(1); i++)
        {
            double aparicion = 0;
            for (int j = 0; j < Matrix.TF_IDF.GetLength(0); j++)
            {
                if (Matrix.TF_IDF[j, i] != 0)
                {
                    aparicion++;
                }
            }
            //la condicional mas importante de todo el trabajo para mi que cantidad de errores me dio xd
            //ya que este vector este hecho en funcion de la matriz analizando el tf-idf de columna en 
            //columna para ver la aparicion de la palabra en los documentos
            //pero el tf - idf de las conjunciones, prepociones y demas demas es cero
            //asi que se indefine el idf 
            //por eso ignoro completamente esas palabras para que tome el valor x defecto de un 
            //array numerico
            if (aparicion != 0)
            {
                idf[i] = Math.Log(Textos.documentos.Length / aparicion);
            }
        }
        string[] vector  = query.ToLower().Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace("?", "").Replace("(", "").Replace(")", "").Split(' ');

        double[] vectorNum = new double[Lista.Palabras.Count];
        //aqui realice un proceso similar al de la matriz para el tf idf del vector query
        Dictionary<string, int> diccionario = new Dictionary<string, int>();
        foreach (string palabra in vector)
        {
            if (diccionario.ContainsKey(palabra))
            {
                diccionario[palabra]++;
            }
            else
            {
                diccionario[palabra] = 1;
            }
        }
        int cantDePalabras = diccionario.Values.Sum();
        for (int i = 0; i < Lista.Palabras.Count; i++)
        {
            string palabra = Lista.Palabras[i];
            double contador = 0;
            if (diccionario.ContainsKey(palabra))
            {
                contador = diccionario[palabra];
            }

            vectorNum[i] = contador / cantDePalabras * idf[i];


        }
        //vamos a proceder a calcular el producto punto que no es mas que la suma de los
        //productos de los elementos correspondientes del vectorquery con los vectores documentos 
        //y los ire guardando en el array producto punto
        double[] productoPunto = new double[Matrix.TF_IDF.GetLength(0)];
        double sumatoriaVector = 0;
        int contador1 = 0;
        double vectorMagnitud = 0;
        double[] similitud = new double[Matrix.TF_IDF.GetLength(0)];
        for (int i = 0; i < Matrix.TF_IDF.GetLength(0); i++)
        {
            for (int j = 0; j < Matrix.TF_IDF.GetLength(1); j++)
            {
                productoPunto[i] += Matrix.TF_IDF[i, j] * vectorNum[j];
                if (contador1 == 0)
                {
                    sumatoriaVector += Math.Pow(vectorNum[j], 2);
                    //magnitud del vector que sera multiplicada x las magnitudes de cada documento
                    vectorMagnitud = Math.Sqrt(sumatoriaVector);
                }
            }
            //y aqui multiplico la magnitud del vector por la de cada documento
            Magnitud.documentos[i] = Magnitud.documentos[i] * vectorMagnitud;
            //ahora voy a aplicar la formula de la similitud del coseno que no es mas que la
            //division del producto punto del vector con el documento entre la magnitud del 
            //vector por la magnitud del documento
            //aqui igual hay que utilizar una constante en la division x si acaso la query o algun documento esta vacio
            //no de error ya que la division por cero no esta definida
            similitud[i] = productoPunto[i] / (Magnitud.documentos[i] + 1);
            contador1 = 1;
        }
        return similitud;
    }
}