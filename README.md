# SoulFoodAI

Sistema de Gestión Nutricional basado en ASP.NET Core y Angular, con integración del modelo de inteligencia artificial Google Gemini 2.5 Flash para la generación y evaluación de dietas personalizadas.

## Descripción del Proyecto

SoulFoodAI es una aplicación web integral diseñada para resolver la dificultad de mantener una alimentación saludable y variada mediante la personalización inteligente. Su objetivo principal es ofrecer una solución empática frente a los sistemas tradicionales, adaptándose al estado físico y emocional del usuario mientras aprovecha su despensa real para evitar el desperdicio alimentario.

Una característica diferenciadora de SoulFoodAI es la integración nativa del motor de inteligencia artificial Google Gemini 2.5 Flash y la base de datos colaborativa Open Food Facts. Esta sinergia permite la generación automatizada de recetas dinámicas y la catalogación precisa de alimentos, evaluando variables de estilo de vida para diseñar planes nutricionales altamente adaptables.

## Arquitectura y Tecnologías

El desarrollo se ha abordado con un fuerte énfasis en la calidad del software, la seguridad y la escalabilidad, implementando una arquitectura moderna:

- Backend: ASP.NET Core 8.0 (API REST en C#).
- Frontend: Angular.
- Base de Datos: Microsoft SQL Server.
- Inteligencia Artificial: Google Gemini 2.5 Flash.
- Integración de Datos: Open Food Facts.
- Infraestructura y Despliegue: Contenedores Docker.
- Calidad y Testing: SonarCloud, Vitest y xUnit.

## Flujo de Trabajo del Sistema

- Fase A: Captura y Perfilado
  El usuario inicia sesión y completa su ficha física. Se almacena su objetivo y sus intolerancias.

- Fase B: Selección de Menú
  El usuario elige el tipo de menú o dieta que desea seguir.

- Fase C: Selección de Ingredientes
  A través de la integración con Open Food Facts, el usuario navega por el catálogo de ingredientes y marca los que desea incluir en su dieta, basándose en su despensa real.

- Fase D: Generación por Inteligencia Artificial
  El sistema envía la información recolectada a la API de Google Gemini. La IA devuelve una estructura de 7 días con las recetas generadas de manera dinámica que mejor se adaptan a las necesidades del usuario.

- Fase E: Reporte y Reevaluación
  Al final de la semana, el usuario completa un reporte sobre su estado. Estos datos se envían a Gemini, que genera un feedback para realizar ajustes en el próximo plan alimenticio.

## Instalación y Ejecución Local

Para levantar el proyecto en un entorno local, se recomienda el uso de Docker Compose.

### Requisitos Previos

- Docker Desktop o Docker Compose instalado.
- API Key de Google Gemini.

### Pasos de Despliegue

1. Clonar el repositorio.
2. Configurar las variables de entorno necesarias. Específicamente, añadir la clave de la API de Gemini en el archivo de configuración correspondiente del backend.
3. Navegar a la carpeta "src" en la terminal.
4. Ejecutar el comando para levantar los servicios:
   docker-compose up --build
5. La aplicación estará disponible localmente para el frontend y el backend a través de los puertos definidos.

## Licencias y Avisos Legales

El código fuente de este proyecto se distribuye bajo la Licencia MIT.

Avisos sobre dependencias de terceros:
- Google Gemini API: El uso del motor de inteligencia artificial está sujeto a los Términos de Servicio de Google.
- Open Food Facts: La información de productos alimenticios es obtenida de esta base de datos colaborativa, que se distribuye bajo la licencia Open Database License (ODbL).
- Microsoft SQL Server: El uso de las imágenes de contenedor de SQL Server requiere la aceptación de su Acuerdo de Licencia de Usuario Final (EULA).
