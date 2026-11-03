# 13. Manual de solución de problemas

## 13.1 Instalación

| Problema | Causa probable | Solución |
|---|---|---|
| Visual Studio no muestra proyectos MAUI | Falta la carga de trabajo | Visual Studio Installer → Modificar → «Desarrollo de la interfaz de usuario de aplicaciones multiplataforma de .NET». |
| `NETSDK1045` / «el SDK actual no admite .NET 9» | SDK antiguo | Instalar .NET SDK 9.0.1xx o superior; `dotnet --list-sdks` para verificar. |
| `A compatible .NET SDK was not found` (global.json) | No hay SDK 9.0 | Instalar el SDK 9 o, si migró a .NET 10, actualizar `global.json`. |
| `workload maui not installed` | Falta el workload por consola | `dotnet workload install maui` (como administrador). |
| No aparecen emuladores Android | SDK/emulador no instalado | Herramientas → Android → Administrador de dispositivos → crear un dispositivo API 34. |

## 13.2 Compilación

| Problema | Causa probable | Solución |
|---|---|---|
| `Unable to load DLL 'e_sqlite3'` en Windows | Faltan componentes C++ | Instalar «Herramientas de compilación de C++ para escritorio» y recompilar. |
| Errores de paquetes NuGet tras cambiar de equipo | Caché o restauración incompleta | Clic derecho en la solución → Restaurar paquetes NuGet; o `dotnet restore`. |
| Errores raros después de cambiar de rama | Carpetas `bin`/`obj` viejas | Cerrar VS, borrar `bin` y `obj` de ambos proyectos, compilar de nuevo. |
| `XFC0000` / errores XAML | Binding o recurso mal escrito | Revisar la línea indicada; los estilos están en `Resources/Styles`. |
| Las pruebas no aparecen en el Explorador | Falta compilar el proyecto de pruebas | Compilar `AsistenteIA.Tests` y pulsar «Ejecutar todas»; o `dotnet test AsistenteIA.Tests`. |
| Archivo de la app en uso al compilar | La app sigue abierta | Cerrar la app o detener la depuración. |

## 13.3 Conexión con la IA

| Mensaje | Causa | Solución |
|---|---|---|
| «Sin conexión a internet…» | Sin red | Revisar Wi-Fi/datos y volver a intentar. |
| «No se pudo conectar con el servicio…» | DNS, proxy o firewall bloquean el dominio | Permitir `generativelanguage.googleapis.com` y `gen.pollinations.ai` (puerto 443). |
| «Tiempo de espera agotado…» | Respuesta muy lenta (> 60 s) | Reintentar, acortar la consulta o cambiar de modelo/proveedor. |
| «Error de autorización (400/401/403)…» | Clave inválida, revocada o de otro proveedor | Configuración → elegir el proveedor correcto → pegar una clave nueva → Guardar → Probar conexión. |
| «…saldo o límite gratuito (402)» | Pollinations sin saldo | Revisar la cuenta en enter.pollinations.ai o cambiar a Gemini. |
| «Modelo no encontrado (404)…» | Modelo retirado o mal escrito | Elegir un modelo de la lista sugerida. |
| «Se superó la cuota… (429)» | Límite del proveedor | Esperar unos minutos. |
| «Límite de uso alcanzado: espere N s» | Más de 8 consultas por minuto (límite de la app) | Esperar el tiempo indicado. |
| «El servicio de IA no está disponible (5xx)» | Falla del proveedor | Reintentar más tarde o cambiar de proveedor. |
| «El servicio bloqueó la consulta (…)» | Filtro de seguridad de Gemini | Reformular la pregunta. |

## 13.4 Configuración y datos

| Problema | Causa | Solución |
|---|---|---|
| «Configure la clave de {proveedor}…» | El proveedor activo no tiene clave | Configuración → escribir la clave de ese proveedor → Guardar. |
| Cambié de proveedor y sigue usando el anterior | No se pulsó Guardar | El selector solo prepara el cambio; pulse **Guardar** y revise «Proveedor activo». |
| «No se pudo leer el almacén seguro» | SecureStorage dañado (p. ej. tras restaurar el sistema) | Eliminar la clave y guardarla de nuevo; en Android, borrar datos de la app. |
| El historial desapareció | Se usó «Borrar todos mis datos locales» o se reinstaló la app | Los datos son locales; no hay copia en la nube. |
| El aviso de uso responsable no vuelve a salir | Ya se aceptó | Es normal; se muestra una sola vez. |
