# EduConecta

Sistema web de gestion academica desarrollado en ASP.NET Core MVC con Entity Framework Core
y SQL Server. Permite administrar sedes, estudiantes, becas, docentes, cursos, modulos y
matriculas, e incluye dos reportes de seguimiento.

El proyecto implementa el diagrama entidad-relacion elaborado en la Unidad 1
(MER_EduConecta), respetando sus entidades, relaciones, claves y restricciones.

## Integrantes

| Integrante | Usuario de GitHub | Aporte principal |
|---|---|---|
| Dario Haziel Tambo Parra | SuperGenio56 | Base del proyecto, paginacion, sedes, estudiantes y becas |
| Julio Cesar Cruz Kübber | Junior02000XD | Docentes, cursos, modulos y prerrequisitos |
| Nombre del integrante 3 | usuario3 | Matriculas, validaciones y reportes |

## Requisitos

- .NET SDK 10.0
- SQL Server (Express, Developer o LocalDB)
- Visual Studio 2022 o superior, o Visual Studio Code

## Pasos para ejecutar

1. Clonar el repositorio:

       git clone https://github.com/SuperGenio56/EduConecta.git

2. Abrir `EduConecta.sln`.

3. En `EduConecta/appsettings.json`, ajustar el valor de `Server` de la cadena
   `EduConectaConexion` al nombre de la instancia local de SQL Server. La conexion usa
   autenticacion de Windows, por lo que no se guardan usuarios ni contrasenas.

4. Crear la base de datos aplicando las migraciones. Desde la carpeta `EduConecta`:

       dotnet ef database update

   En Visual Studio, el equivalente es `Update-Database` en la Consola del
   Administrador de paquetes.

5. Ejecutar el proyecto con F5 o con `dotnet run`.

Las migraciones crean la base `EduConectaDb` y cargan datos iniciales de sedes, docentes,
cursos y modulos, para que la aplicacion se pueda probar de inmediato. Los estudiantes,
becas y matriculas se registran desde la aplicacion.

## Reportes

### Ocupacion y recaudacion por curso

Muestra, para cada curso, cuantos estudiantes se matricularon, cuantos cupos quedan libres,
el porcentaje de ocupacion y el dinero recaudado, separando el monto bruto, el descuento
por becas y el monto neto. Se puede filtrar por periodo academico, modalidad y sede.

Sirve a coordinacion academica para decidir que cursos abrir o cerrar en el siguiente
periodo y para proyectar los ingresos. Las matriculas anuladas quedan fuera del calculo.

### Becas otorgadas y descuentos aplicados

Muestra, por estudiante y tipo de beca, cuantas matriculas se beneficiaron, el monto
original, el monto becado y el monto que el estudiante debe pagar. Se puede filtrar por
rango de fechas de matricula, tipo de beca y sede.

Sirve a bienestar estudiantil para controlar cuanto deja de percibir la institucion por
concepto de becas en un periodo y verificar que cada beca se este usando.

## Estructura del proyecto

    EduConecta/
      Controllers/   controladores MVC
      Data/          DbContext, configuraciones Fluent API y migraciones
      Helpers/       paginacion reutilizable
      Models/        entidades del modelo
      ViewModels/    modelos de formularios y reportes
      Views/         vistas Razor
