# Examen Parcial - Simulador de Trayectoria de Dron

Este proyecto consiste en una aplicación de consola desarrollada en C# (.NET Core) que simula el recorrido óptimo de un dron sobre un terreno de dimensiones $N \times N$, utilizando algoritmos de grafos (BFS) y optimización mediante Backtracking con la Heurística de Warnsdorff. Los datos de la simulación se persisten en una base de datos PostgreSQL utilizando ADO.NET síncrono puro.

## Requisitos Previos

* .NET SDK (Versión 6.0 o superior)
* PostgreSQL Server instalado localmente
* Un gestor de base de datos (como DBeaver o pgAdmin)

## Instrucciones de Configuración

### 1. Base de Datos (Parte A)
Antes de ejecutar la aplicación, es necesario crear la estructura relacional. Ejecute el siguiente script DDL en su servidor de PostgreSQL (base de datos destino):

-- 1. Creación de la tabla principal (Cabecera)
CREATE TABLE tb_master_control (
    id SERIAL PRIMARY KEY,
    fecha_sistema TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    terreno_n INT NOT NULL,
    despegue_x INT NOT NULL,
    despegue_y INT NOT NULL
);

-- 2. Creación de la tabla de detalle
CREATE TABLE tb_det_log (
    id SERIAL PRIMARY KEY,
    master_id INT NOT NULL,
    paso_etiqueta INT NOT NULL,
    posicion_x INT NOT NULL,
    posicion_y INT NOT NULL,
    CONSTRAINT fk_det_master FOREIGN KEY (master_id) REFERENCES tb_master_control(id) ON DELETE CASCADE
);

------------------------------------------------------------------------------------------------------------

2. Configurar Cadena de Conexión (Parte C)
Abra el archivo appsettings.json ubicado en la raíz del proyecto ExamenParcialDron y configure las credenciales de acceso correspondientes a su entorno local:

{
"ConnectionStrings": {
"PostgreSQLConnection": "Host=localhost;Port=5432;Database=DronSimuladorDb;Username=postgres;Password=TU_CONTRASEÑA_AQUI"
}
}

Importante: Asegúrese de que en las propiedades del archivo appsettings.json dentro de Visual Studio, la opción "Copiar en el directorio de salida" (Copy to Output Directory) esté configurada como "Copiar si es posterior" (Copy if newer).

-------------------------------------------------------------------------------------------------------------
Paquetes NuGet Utilizados
El proyecto ya cuenta con las dependencias necesarias configuradas en el archivo .csproj:

Npgsql (Driver oficial de PostgreSQL para .NET)

Microsoft.Extensions.Configuration (Gestión de configuraciones)

Microsoft.Extensions.Configuration.Json (Soporte para lectura de archivos JSON)

------------------------------------------------------------------------------------------------------------

Características Destacadas (Estrategia de Resolución)
Separación de Responsabilidades: La lógica pura del algoritmo y las estructuras de datos residen exclusivamente en SimuladorVuelo.cs, aislando la infraestructura, lectura de configuraciones y la persistencia en Program.cs.

Heurística de Warnsdorff: Implementada para mitigar la explosión combinatoria del Backtracking clásico (ordenando candidatos por menor grado de salida primero). Permite resolver terrenos de 5 x 5 de manera instantánea.

Control Transaccional Fuerte (Parte D): La rutina de persistencia en base de datos implementa de forma explícita un bloque de transacción (NpgsqlTransaction) garantizando la atomicidad mediante directivas Commit y Rollback.

Algoritmos de Control Exigidos: Se evitó estrictamente el uso de estructuras iterativas tradicionales (for / foreach) en el volcado de datos, resolviendo la persistencia mediante un ciclo while indexado manualmente.

Ingeniería Inversa en Caliente (Parte E): El reporte inverso final decodifica matemáticamente las etiquetas ofuscadas guardadas en la base de datos (pasos pares multiplicados por 2, impares con signo invertido) directamente desde el flujo de lectura del NpgsqlDataReader.