namespace MoogleEngine;
public class Magnitud
{
    public static double[] documentos {get; set;}
    public Magnitud()
    {
       documentos = Magnitud_();
    }
    public static double[] Magnitud_()
    {
        double[] magnitud = new double[Matrix.TF_IDF.GetLength(0)];
        for (int i = 0; i < Matrix.TF_IDF.GetLength(0); i++)
        {
            double sumatoriaDocumento = 0;
            for (int j = 0; j < Matrix.TF_IDF.GetLength(1); j++)
            {
                sumatoriaDocumento += Math.Pow(Matrix.TF_IDF[i, j], 2);
            }
            //ahora vamos a ver la magnitud de cada documento
            double docMagnitud = Math.Sqrt(sumatoriaDocumento);
            magnitud[i] = docMagnitud;
        }
        return magnitud;
    }
}