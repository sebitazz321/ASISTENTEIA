# 1. Introducción

## 1.1 Propósito

AsistenteIA es un asistente académico multiplataforma (Windows y Android) construido con .NET MAUI. Envía consultas
académicas a un modelo de inteligencia artificial mediante una API REST, muestra la respuesta, la guarda en un
historial local (SQLite) y permite organizarla en proyectos, con controles de seguridad, privacidad y uso responsable.

## 1.2 Problema

Los estudiantes usan asistentes de IA de forma dispersa: sin organización por materia, sin registro de lo consultado,
sin verificar las respuestas y, a veces, compartiendo datos personales con el proveedor.

## 1.3 Alcance

1. Enviar consultas académicas a un modelo de IA por REST, eligiendo entre **Google Gemini** y **Pollinations**.
2. Guardar las respuestas localmente y agruparlas en proyectos.
3. Promover la verificación de las respuestas (marca «verificada» y notas).
4. Proteger la clave de API y la privacidad del estudiante.

**Fuera de alcance:** cuentas de usuario en un servidor, sincronización en la nube, conversaciones de varios turnos,
carga de archivos y generación de imágenes.

## 1.4 Usuario objetivo

Estudiante de educación superior que necesita aclarar, resumir o practicar contenidos con ayuda de IA de forma rápida
y segura.

## 1.5 Equipo

| Integrante | Rol |
|---|---|
| Juan Carlos Díaz Cifuentes | Servicios y datos; calidad y documentación |
| Sebastián Grande Moreno | Líder e integración; interfaz y accesibilidad |

**Docente:** Sandra Esperanza Sánchez Barragán · **Asignatura:** Programación Avanzada y Aplicada · VI semestre ·
Tecnología en Desarrollo de Software · Universidad de San Buenaventura.
