using System;
using Microsoft.Data.Sqlite;

namespace CatalogoVideojuegosADO
{
    class Program
    {
        static void Main(string[] args)
        {
            // Cadena de conexión hacia el archivo de base de datos local
            string connectionString = "Data Source=CatalogoJuegos.db";

            Console.WriteLine("=== SISTEMA DE GESTIÓN DE VIDEOJUEGOS (ADO.NET + SQLite) ===\n");

            try
            {
                // 1 y 2. Crear la base de datos y abrir conexión usando 'using'
                using (SqliteConnection conexion = new SqliteConnection(connectionString))
                {
                    conexion.Open();
                    Console.WriteLine("[OK] Conexión establecida con la base de datos SQLite.");

                    // 3. Crear tabla con al menos tres columnas (aquí usamos 4)
                    string sqlCrearTabla = @"
                        CREATE TABLE IF NOT EXISTS Videojuegos (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Titulo TEXT NOT NULL,
                            Genero TEXT NOT NULL,
                            AnioLanzamiento INTEGER NOT NULL
                        );";

                    using (SqliteCommand cmdCrear = new SqliteCommand(sqlCrearTabla, conexion))
                    {
                        cmdCrear.ExecuteNonQuery();
                        Console.WriteLine("[OK] Tabla 'Videojuegos' creada/verificada correctamente.");
                    }

                    // Limpiamos datos previos para evitar duplicados si ejecutas el programa varias veces
                    using (SqliteCommand cmdLimpiar = new SqliteCommand("DELETE FROM Videojuegos;", conexion))
                    {
                        cmdLimpiar.ExecuteNonQuery();
                    }

                    // 4. Insertar al menos tres registros usando SqliteCommand y parámetros
                    InsertarVideojuego(conexion, "The Legend of Zelda: Breath of the Wild", "Aventura / Mundo Abierto", 2017);
                    InsertarVideojuego(conexion, "Hollow Knight", "Metroidvania", 2017);
                    InsertarVideojuego(conexion, "Elden Ring", "RPG de Acción", 2022);
                    InsertarVideojuego(conexion, "Cyberpunk 2077", "RPG / Ciencia Ficción", 2020);

                    Console.WriteLine("[OK] Se insertaron 4 registros en la base de datos.\n");

                    // 5. Consultar y mostrar registros con SqliteDataReader
                    Console.WriteLine("--- LISTADO DE VIDEOJUEGOS REGISTRADOS ---");
                    string sqlConsultar = "SELECT Id, Titulo, Genero, AnioLanzamiento FROM Videojuegos;";

                    using (SqliteCommand cmdConsultar = new SqliteCommand(sqlConsultar, conexion))
                    using (SqliteDataReader lector = cmdConsultar.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            int id = lector.GetInt32(0);
                            string titulo = lector.GetString(1);
                            string genero = lector.GetString(2);
                            int anio = lector.GetInt32(3);

                            Console.WriteLine($"ID: {id} | Título: {titulo} | Género: {genero} | Año: {anio}");
                        }
                    }
                } // 6. Aquí 'using' cierra y libera la conexión automáticamente
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Error de SQLite: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
            }

            Console.WriteLine("\nProceso finalizado correctamente.");
        }

        static void InsertarVideojuego(SqliteConnection conexion, string titulo, string genero, int anio)
        {
            string sqlInsertar = @"
                INSERT INTO Videojuegos (Titulo, Genero, AnioLanzamiento) 
                VALUES (@titulo, @genero, @anio);";

            using (SqliteCommand cmdInsertar = new SqliteCommand(sqlInsertar, conexion))
            {
                cmdInsertar.Parameters.AddWithValue("@titulo", titulo);
                cmdInsertar.Parameters.AddWithValue("@genero", genero);
                cmdInsertar.Parameters.AddWithValue("@anio", anio);

                cmdInsertar.ExecuteNonQuery();
            }
        }
    }
}