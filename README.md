# SoulFoodAI
Sistema de Gestión Nutricional basado en ASP.NET Core con Integración de Modelos de Lenguaje de Gran Escala (LLM) para la Generación y Evaluación de Dietas Personalizadas.

## Descripción del Proyecto
El proyecto consiste en el desarrollo de una plataforma web con ASP.NET Core y Angular para la planificación dietética. La innovación principal radica en el abandono de algoritmos de asignación rígidos en favor de un Agente de Inteligencia Artificial vía API de ChatGPT.
El sistema recolecta datos biométricos y preferencias de ingredientes del usuario mediante una interfaz y utiliza un motor de Prompt Engineering para interactuar con un LLM. Esta IA se encarga de dos tareas críticas:
1.	Generación de Recetas: Seleccionar y combinar recetas basadas en los ingredientes favoritos y las restricciones médicas del usuario.
2.	Feedback y Ajuste nutricional: Evaluar el progreso semanal del usuario (basado en un diario de sensaciones semanales) y reajustar el plan de forma dinámica.

## Stack Tecnológico
•	Backend: ASP.NET Core 8.0 (C#).
•	Base de Datos: Microsoft SQL Server 2022 y Azure Data Studio
•	Inteligencia Artificial: OpenAI API (GPT-4o / GPT-3.5 Turbo) para el motor de recomendación.
•	Frontend: Angular
•	ORM: Entity Framework Core con enfoque Code First.

## Flujo de Trabajo del Sistema (Workflow)
Fase A: Captura y Perfilado
El usuario inicia sesión y completa su ficha física. Se almacena su objetivo (Ganar músculo, Perder grasa, etc.) y sus intolerancias.
Fase B: Selección de menú
El usuario elige que tipo de menú/dieta quiere seguir, ya sea Mediterránea, Keto, Equilibrada, etc…
Fase C: Selección de Ingredientes
El usuario navega por el catálogo de ingredientes (Proteínas, Hidratos, Frutas, etc…) y marca los que desea incluir en su dieta.
Fase C: Generación por IA 
El sistema envía un JSON a la API de ChatGPT que contiene:
•	Ratios del plan elegido (ej. Keto Laxa o Sana 50% verduras 25% proteínas 20% hidratos).
•	Lista de ingredientes permitidos por el usuario.
•	Objetivos del usuario e intolerancias de este mismo
•	Requerimientos calóricos totales.
La IA devuelve una estructura de 7 días con las recetas que mejor encajan
Fase D: Reporte y Re-evaluación
Al final de la semana, el usuario completa el UserDiary. Estos datos se envían a la IA junto con el plan seguido. La IA genera un "feedback" humano y profesional, sugiriendo ajustes en el próximo FoodPlan.

## Objetivos del Proyecto
Objetivos Técnicos
•	Integración de API de Terceros: Implementar un servicio de comunicación asíncrona entre el servidor ASP.NET Core y la API de OpenAI.
•	Arquitectura de Datos Relacional: Gestionar una base de datos  robusta que almacene el historial de ingredientes, planes maestros y diarios de usuario.
•	Seguridad y Perfiles: Implementar un sistema de autenticación donde cada usuario mantenga la persistencia de sus datos físicos y sus planes activos.
Objetivos Funcionales
•	Onboarding Biométrico: Calcular requerimientos calóricos basados en peso, altura, edad y género.
•	Filtrado de Ingredientes por Usuario: Permitir al usuario definir su despensa ideal para que la IA solo trabaje con alimentos permitidos.
•	Evaluación Semanal Inteligente: Analizar el "Reporte" de fin de semana (energía, saciedad, sueño) mediante IA para decidir si se mantiene el plan o se genera uno nuevo.

