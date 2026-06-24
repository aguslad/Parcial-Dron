using System;
using System.Collections.Generic;

namespace ExamenParcialDron
{
    // Estructura para manejar coordenadas de forma limpia
    public struct Coordenada
    {
        public int X { get; set; } // Fila
        public int Y { get; set; } // Columna

        public Coordenada(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Clase independiente para encapsular la lógica del negocio y algoritmos (Parte B)
    public class SimuladorVuelo
    {
        private readonly int N;
        private readonly int[,] matriz;
        private readonly List<Coordenada> secuenciaResultados;

        // Vectores fijos de movimiento en L (2 en un sentido, 1 en perpendicular)
        private readonly int[] movX = { -2, -2, 2, 2, -1, 1, -1, 1 };
        private readonly int[] movY = { -1, 1, -1, 1, -2, -2, 2, 2 };

        public SimuladorVuelo(int n)
        {
            N = n;
            matriz = new int[N, N];
            secuenciaResultados = new List<Coordenada>();

            // Inicializar matriz con -1 (indicando que están vacías/libres)
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    matriz[i, j] = -1;
        }

        // Determina cuántas celdas son accesibles desde el punto de despegue (Uso de BFS/Grafo)
        public int CalcularParcelasAlcanzables(Coordenada inicio)
        {
            bool[,] visitado = new bool[N, N];
            Queue<Coordenada> cola = new Queue<Coordenada>();
            int conteo = 0;

            cola.Enqueue(inicio);
            visitado[inicio.X, inicio.Y] = true;

            while (cola.Count > 0)
            {
                Coordenada actual = cola.Dequeue();
                conteo++;

                for (int i = 0; i < 8; i++)
                {
                    int nx = actual.X + movX[i];
                    int ny = actual.Y + movY[i];

                    if (EsValido(nx, ny) && !visitado[nx, ny])
                    {
                        visitado[nx, ny] = true;
                        cola.Enqueue(new Coordenada(nx, ny));
                    }
                }
            }
            return conteo;
        }

        public bool ResolverSimulacion(Coordenada inicio, int totalAlcanzables)
        {
            // El paso 0 es el despegue
            matriz[inicio.X, inicio.Y] = 0;
            secuenciaResultados.Add(inicio);

            if (BuscarRutaRecursiva(inicio.X, inicio.Y, 1, totalAlcanzables))
            {
                return true;
            }

            return false;
        }

        // BACKTRACKING CON HEURÍSTICA DE WARNSDORFF (Menor Grado Primero)
        private bool BuscarRutaRecursiva(int x, int y, int pasoActual, int totalObjetivo)
        {
            // Condición de éxito: cubrimos todas las parcelas que forman parte de la isla alcanzable
            if (pasoActual == totalObjetivo)
                return true;

            // Obtener y ordenar candidatos por su grado ascendente
            List<Candidato> candidatos = ObtenerCandidatosOrdenados(x, y);

            int i = 0;
            while (i < candidatos.Count)
            {
                Candidato c = candidatos[i];

                // Realizar movimiento (marcar visitado)
                matriz[c.Coord.X, c.Coord.Y] = pasoActual;
                secuenciaResultados.Add(c.Coord);

                // Evaluar recursivamente
                if (BuscarRutaRecursiva(c.Coord.X, c.Coord.Y, pasoActual + 1, totalObjetivo))
                    return true;

                // BACKTRACKING: Deshacer movimiento si no prosperó
                matriz[c.Coord.X, c.Coord.Y] = -1;
                secuenciaResultados.RemoveAt(secuenciaResultados.Count - 1);

                i++;
            }

            return false;
        }

        private struct Candidato
        {
            public Coordenada Coord { get; }
            public int Grado { get; }

            public Candidato(Coordenada coord, int grado)
            {
                Coord = coord;
                Grado = grado;
            }
        }

        private List<Candidato> ObtenerCandidatosOrdenados(int x, int y)
        {
            List<Candidato> lista = new List<Candidato>();

            for (int i = 0; i < 8; i++)
            {
                int nextX = x + movX[i];
                int nextY = y + movY[i];

                if (EsValido(nextX, nextY) && matriz[nextX, nextY] == -1)
                {
                    int grado = GetGrado(nextX, nextY);
                    lista.Add(new Candidato(new Coordenada(nextX, nextY), grado));
                }
            }

            // Ordenar por grado ascendente (Menor grado primero)
            lista.Sort((a, b) => a.Grado.CompareTo(b.Grado));
            return lista;
        }

        private int GetGrado(int x, int y)
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                int nx = x + movX[i];
                int ny = y + movY[i];
                if (EsValido(nx, ny) && matriz[nx, ny] == -1)
                    count++;
            }
            return count;
        }

        private bool EsValido(int x, int y)
        {
            return (x >= 0 && x < N && y >= 0 && y < N);
        }

        public List<Coordenada> ObtenerSecuenciaMovimientos()
        {
            return secuenciaResultados;
        }

        public void ImprimirMatriz()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (matriz[i, j] == -1)
                        Console.Write("  . ");
                    else
                        Console.Write($"{matriz[i, j],3} ");
                }
                Console.WriteLine();
            }
        }
    }
}