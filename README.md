# SoulFoodAI 🥗🤖

![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)
![Angular](https://img.shields.io/badge/Angular-DD0031?style=flat&logo=angular&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-red.svg)
![OpenAI](https://img.shields.io/badge/OpenAI-GPT--4o-green.svg)
![License](https://img.shields.io/badge/License-MIT-yellow.svg)

Sistema de Gestión Nutricional basado en ASP.NET Core con Integración de Modelos de Lenguaje de Gran Escala (LLM) para la Generación y Evaluación de Dietas Personalizadas.

## 📖 Descripción del Proyecto

El proyecto consiste en el desarrollo de una plataforma web interactiva con **ASP.NET Core** (Backend) y **Angular** (Frontend) para la planificación dietética. La innovación principal radica en el abandono de algoritmos de asignación rígidos en favor de un Agente de Inteligencia Artificial vía **API de OpenAI (ChatGPT)**.

El sistema recolecta datos biométricos y preferencias de ingredientes del usuario mediante una interfaz intuitiva y utiliza un motor de *Prompt Engineering* para interactuar con un LLM. Esta IA se encarga de dos tareas críticas:
1. **Generación de Recetas:** Seleccionar y combinar recetas basadas en los ingredientes favoritos y las restricciones médicas del usuario.
2. **Feedback y Ajuste Nutricional:** Evaluar el progreso semanal del usuario (basado en un diario de sensaciones semanales) y reajustar el plan de forma dinámica.

---

## 🛠️ Stack Tecnológico

- **Backend:** ASP.NET Core 8.0 (C#)
- **Base de Datos:** Microsoft SQL Server 2022 (desplegada vía Docker)
- **Inteligencia Artificial:** OpenAI API (GPT-4o / GPT-3.5 Turbo) para el motor de recomendación
- **Frontend:** Angular
- **ORM:** Entity Framework Core con enfoque Code First
- **Infraestructura:** Docker & Docker Compose

---

## 🔄 Flujo de Trabajo del Sistema (Workflow)

- **Fase A: Captura y Perfilado**  
  El usuario inicia sesión y completa su ficha física. Se almacena su objetivo (Ganar músculo, Perder grasa, etc.) y sus intolerancias.
  
- **Fase B: Selección de Menú**  
  El usuario elige qué tipo de menú/dieta quiere seguir, ya sea Mediterránea, Keto, Equilibrada, etc.

- **Fase C: Selección de Ingredientes**  
  El usuario navega por el catálogo de ingredientes (Proteínas, Hidratos, Frutas, etc.) y marca los que desea incluir en su dieta.

- **Fase D: Generación por IA**  
  El sistema envía un JSON a la API de OpenAI que contiene:
  - Ratios del plan elegido (ej. Keto Laxa o Sana 50% verduras, 25% proteínas, 25% hidratos).
  - Lista de ingredientes permitidos por el usuario.
  - Objetivos del usuario e intolerancias del mismo.
  - Requerimientos calóricos totales.
  
  La IA devuelve una estructura de 7 días con las recetas que mejor encajan.

- **Fase E: Reporte y Re-evaluación**  
  Al final de la semana, el usuario completa el *UserDiary*. Estos datos se envían a la IA junto con el plan seguido. La IA genera un "feedback" humano y profesional, sugiriendo ajustes en el próximo plan alimenticio (*FoodPlan*).

---

## 🎯 Objetivos del Proyecto

### Objetivos Técnicos
- **Integración de API de Terceros:** Implementar un servicio de comunicación asíncrona entre el servidor ASP.NET Core y la API de OpenAI.
- **Arquitectura de Datos Relacional:** Gestionar una base de datos robusta que almacene el historial de ingredientes, planes maestros y diarios de usuario.
- **Seguridad y Perfiles:** Implementar un sistema de autenticación donde cada usuario mantenga la persistencia de sus datos físicos y sus planes activos.
- **Contenerización:** Uso de Docker Compose para orquestar la base de datos, el backend y el frontend.

### Objetivos Funcionales
- **Onboarding Biométrico:** Calcular requerimientos calóricos basados en peso, altura, edad y género.
- **Filtrado de Ingredientes por Usuario:** Permitir al usuario definir su despensa ideal para que la IA solo trabaje con alimentos permitidos.
- **Evaluación Semanal Inteligente:** Analizar el "Reporte" de fin de semana (energía, saciedad, sueño) mediante IA para decidir si se mantiene el plan o se genera uno nuevo.

---

## 🚀 Instalación y Ejecución Local

Para levantar el proyecto en tu entorno local, el método más sencillo es a través de Docker.

### Prerrequisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (o Docker Compose) instalado.
- Opcional para ejecución manual: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) y [Node.js](https://nodejs.org/).
- Una **API Key de OpenAI**.

### Despliegue con Docker Compose (Recomendado)

1. Clona el repositorio:
   ```bash
   git clone <URL_DEL_REPOSITORIO>
   cd SoulFoodAI
   ```

2. Configura las variables de entorno necesarias (asegúrate de configurar tu API Key de OpenAI en el backend).

3. Levanta todos los servicios (Base de Datos, Backend y Frontend):
   ```bash
   cd src
   docker-compose up --build
   ```

4. La aplicación estará disponible en:
   - **Frontend:** http://localhost:4200
   - **Backend API:** http://localhost:8080

---

## 📄 Licencias

El código fuente de este proyecto se distribuye bajo la **Licencia MIT** (ver el archivo `LICENSE` para más detalles).

**Nota sobre dependencias de terceros y servicios:**
- **OpenAI API:** Este proyecto requiere una cuenta activa y hace uso de la API de OpenAI. El uso de esta API está sujeto a los [Términos de Servicio de OpenAI](https://openai.com/policies/terms-of-use).
- **Microsoft SQL Server:** El contenedor de base de datos utiliza la imagen oficial de Microsoft SQL Server 2022, la cual requiere la aceptación de su [EULA (Acuerdo de Licencia de Usuario Final)](https://www.microsoft.com/en-us/sql-server/sql-server-2022-pricing).
