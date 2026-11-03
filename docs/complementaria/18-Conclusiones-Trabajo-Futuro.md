# 18. Conclusiones y trabajo futuro

## 18.1 Resultados obtenidos

- Aplicación .NET MAUI funcional en Windows (y compilable para Android) con **8 pantallas**, arquitectura **MVVM** e
  inyección de dependencias.
- Consumo de API REST de IA con **POST + JSON**, cancelación, tiempo máximo de 60 s y manejo de errores por código HTTP.
- **Dos proveedores** (Gemini y Pollinations) con cambio manual, sin tocar vistas ni ViewModels de consulta.
- Persistencia **SQLite** con CRUD de consultas y proyectos; perfil y preferencias locales.
- Seguridad de credenciales: SecureStorage, clave en encabezado, sin claves en el repositorio (auditoría con 14 controles cumplidos).
- Uso responsable: aviso inicial, detector de datos sensibles, límite de uso, verificación manual y borrado de datos.
- **66 pruebas automáticas** sobre la lógica pura y 15 casos manuales definidos.

## 18.2 Cumplimiento de objetivos

| Objetivo | Resultado |
|---|---|
| Asistente académico con IA vía API REST | ✅ |
| Historial persistente en SQLite | ✅ |
| Seguridad de la clave | ✅ |
| Mínimo cinco pantallas con Shell y MVVM | ✅ (8) |
| Manejo de carga, cancelación y errores | ✅ |
| Pruebas | ✅ automáticas · ⚠️ manuales P10–P15 y P16–P20 por ejecutar |
| Evidencia de trabajo en equipo en Git | ⚠️ El historial no refleja aún commits de ambos integrantes |

## 18.3 Limitaciones

1. El código nuevo (Pollinations, errores 402/408 y 32 pruebas) se escribió sin compilador disponible en el entorno de
   edición: **debe compilarse y ejecutarse en Visual Studio** antes de la entrega.
2. .NET 9 deja de tener soporte el 10/11/2026.
3. Los modelos y la API de Pollinations cambian con frecuencia; los modelos sugeridos deben revisarse.
4. Sin sincronización entre dispositivos: el historial es solo local.
5. `SensitiveDataDetector` es heurístico (puede tener falsos positivos y negativos).
6. Las pantallas no tienen pruebas automáticas de interfaz.
7. `ApplicationId` sigue con el valor de plantilla `com.companyname.asistenteia`.

## 18.4 Trabajo futuro

| Prioridad | Mejora |
|---|---|
| Alta | Migrar a **.NET 10 LTS** y actualizar paquetes (`CommunityToolkit.Mvvm`, xUnit). |
| Alta | Ejecutar y documentar con capturas los casos P10–P20 y las 66 pruebas. |
| Media | Respuestas en *streaming* para ver el texto mientras se genera. |
| Media | Conversaciones de varios turnos (enviar el historial de la conversación como contexto). |
| Media | Cambio automático de proveedor opcional ante 5xx/429, siempre informando al usuario. |
| Media | Exportar el historial a PDF o Markdown. |
| Media | Pruebas de interfaz y pruebas de `AiService` con un `HttpMessageHandler` simulado. |
| Baja | Publicar en Microsoft Store / Google Play con un `ApplicationId` propio. |
| Baja | Backend propio para no exponer claves si la app se distribuye a muchos usuarios. |
