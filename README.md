# Gestor IAAS

Aplicación de escritorio para el registro, seguimiento y análisis de Infecciones Asociadas a la Atención en Salud (IAAS), desarrollada para apoyar el trabajo del área de Infecciones de un hospital, reemplazando el registro manual en Excel por una base de datos local estructurada.

## Motivación

El equipo de Infecciones de un hospital lleva un registro mensual de IAAS en una planilla Excel, con columnas como mes, tipo de IAAS, RUT del paciente, notificación al MINSAL, servicio clínico, microorganismo y observaciones. Este proyecto nace para:

- Centralizar esos registros en una base de datos real, evitando errores de tipeo e inconsistencias.
- Permitir filtrar rápidamente las IAAS por servicio clínico, año, brote o notificación a MINSAL.
- Mantener compatibilidad con el flujo de trabajo existente: se puede seguir importando los Excel ya elaborados, o ingresar los datos manualmente.
- Generar reportes visuales (gráficos) y exportar la información nuevamente a Excel, en el mismo formato institucional.

## Funcionalidades principales

- **Catálogos editables**: gestión de Tipos de IAAS (incluyendo las 28 notificables al MINSAL) y Servicios Clínicos/Especialidades, con posibilidad de agregar y eliminar.
- **Ingreso manual**: formulario para registrar una IAAS nueva sin depender de Excel, con selección de uno o más servicios clínicos, tipo de IAAS mediante lista editable (autocompletado o texto libre), y marcado de brotes.
- **Importación desde Excel**: carga masiva de archivos `.xlsx`, con detección automática de la fila y el orden de las columnas (no depende de una estructura fija), creación automática de catálogos faltantes, formateo automático de RUT, y control de duplicados mediante el identificador original del Excel (DOT) combinado con el año.
- **Listado con filtros combinables**: por año, servicio clínico, brote y notificación a MINSAL, con contador de registros mostrados.
- **Exportación a Excel**: genera un archivo `.xlsx` con el mismo formato institucional, resaltando en rojo las filas notificadas al MINSAL.
- **Gráficos**: visualización de cantidad de IAAS por servicio clínico o por mes, en formato de barras, línea o torta, con exportación a imagen PNG.

## Tecnologías utilizadas

- **C# / .NET** — lenguaje y framework principal
- **WPF (Windows Presentation Foundation)** — interfaz de escritorio
- **Entity Framework Core** — acceso a datos y migraciones
- **SQLite** — base de datos local (archivo único, sin necesidad de servidor)
- **ClosedXML** — lectura y escritura de archivos Excel
- **LiveCharts2** — generación de gráficos interactivos

## Estructura de datos

- **TipoIAAS**: catálogo de tipos de infección (las 28 notificables al MINSAL, más cualquier otra detectada en archivos importados).
- **ServicioClinico**: catálogo de servicios de hospitalización y especialidades médicas.
- **RegistroIAAS**: registro individual de una IAAS, con relación muchos-a-muchos hacia Servicios Clínicos (un registro puede asociarse a más de un servicio), y campos propios para notificación a MINSAL y condición de brote (estas decisiones se guardan por registro, no por catálogo, ya que un mismo tipo de IAAS puede o no notificarse según el caso puntual).

## Cómo ejecutar el proyecto

### Requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) (versión 10 o superior)
- Windows (la interfaz está construida con WPF)

### Pasos

```bash
git clone https://github.com/Marcelgutierr/GestorIAAS.git
cd GestorIAAS/GestorIAAS
dotnet ef database update
dotnet run
```

Esto crea la base de datos local (`iaas.db`, no incluida en el repositorio por contener datos sensibles) y abre la aplicación.

## Privacidad y datos sensibles

Esta aplicación está pensada para funcionar completamente de forma local, sin conexión a internet ni servicios externos: toda la información queda almacenada únicamente en el archivo `iaas.db` del equipo donde se ejecuta. Este archivo está excluido del control de versiones mediante `.gitignore`, y el repositorio público/privado no contiene datos reales de pacientes.

## Estado del proyecto

En desarrollo activo. Próximas mejoras planificadas:

- Rediseño visual de la interfaz
- Generación de instalador para distribución sencilla
- Carga automática del catálogo inicial (28 IAAS MINSAL + servicios clínicos)

## Autor

Proyecto desarrollado como parte de portafolio personal, en conjunto con retroalimentación del equipo de Infecciones Asociadas a la Atención en Salud de un hospital.
