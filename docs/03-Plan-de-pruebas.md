# 3. Plan y registro de pruebas

> **Importante:** las columnas «Resultado obtenido», «Estado» y «Evidencia» deben diligenciarse **después de ejecutar**
> cada caso en el dispositivo. No se rellenaron por el asistente porque los casos no se han ejecutado en un
> dispositivo real. Un fallo encontrado y corregido es un buen resultado: anótelo en «Corrección».

**Dispositivo / versión probada:** _(ej. Pixel 6 – Android 14, o Windows 11)_ · **Fecha:** _(completar)_ · **Responsable:** _(completar)_

## 3.1 Diez casos manuales

| Caso | Condición | Pasos | Resultado esperado | Resultado obtenido | Estado | Evidencia | Corrección |
|---|---|---|---|---|---|---|---|
| P01 | Prompt válido y clave válida | Configurar clave → *Consultar* → «Explica qué es una API REST» | Indicador de carga y luego respuesta visible; mensaje «Respuesta recibida…» | | ☐ OK ☐ Falló | captura | |
| P02 | Prompt vacío o de menos de 5 caracteres | Dejar vacío / escribir «hola» → *Consultar* | No se envía; mensaje de validación | | ☐ OK ☐ Falló | captura | |
| P03 | Sin conexión | Activar modo avión → *Consultar* | La app no se cierra; «Sin conexión a internet…» | | ☐ OK ☐ Falló | captura | |
| P04 | Clave inválida | Guardar una clave falsa → *Consultar* | Error de autorización sin mostrar la clave | | ☐ OK ☐ Falló | captura | |
| P05 | Tiempo agotado | Usar una red muy lenta o cortar la red justo después de pulsar *Consultar* (el límite es de 60 s) | La espera termina con mensaje y se puede reintentar | | ☐ OK ☐ Falló | captura | |
| P06 | Cancelar solicitud | *Consultar* y pulsar *Cancelar* mientras carga | «Consulta cancelada.»; la interfaz no se bloquea | | ☐ OK ☐ Falló | captura | |
| P07 | Guardar conversación | Obtener respuesta → *Guardar* → abrir *Historial* | El registro aparece en el historial | | ☐ OK ☐ Falló | captura | |
| P08 | Eliminar conversación | *Historial* → *Eliminar* → confirmar | Pide confirmación y la lista se actualiza | | ☐ OK ☐ Falló | captura | |
| P09 | Reiniciar la aplicación | Guardar, cerrar por completo y abrir | El historial y los proyectos permanecen | | ☐ OK ☐ Falló | captura | |
| P10 | Pantalla pequeña / ventana estrecha | Probar en teléfono pequeño o reducir la ventana en Windows | Todos los controles son accesibles (se puede desplazar) | | ☐ OK ☐ Falló | captura | |

### Casos adicionales (funciones nuevas)

| Caso | Condición | Resultado esperado | Resultado obtenido | Estado |
|---|---|---|---|---|
| P11 | Escribir «mi correo es a@b.com» y consultar | Aparece el aviso de datos sensibles; *Revisar* cancela el envío | | ☐ OK ☐ Falló |
| P12 | Enviar 9 consultas en menos de un minuto | La novena muestra «Límite de uso alcanzado…» | | ☐ OK ☐ Falló |
| P13 | Crear, editar y eliminar un proyecto | Los cambios se reflejan en la lista; sus consultas siguen en el historial | | ☐ OK ☐ Falló |
| P14 | Editar la nota de una consulta y reabrirla | La nota persiste | | ☐ OK ☐ Falló |
| P15 | *Borrar todos mis datos locales* en Perfil | Historial, proyectos y perfil quedan vacíos | | ☐ OK ☐ Falló |

## 3.2 Pruebas automáticas (xUnit)

Ejecutar desde la raíz del repositorio: `dotnet test AsistenteIA.Tests`

| Archivo | Qué prueba | Casos |
|---|---|---|
| `PromptValidatorTests` | Vacío, corto, mínimo, máximo y sobre el máximo (apoya P02) | 8 |
| `SensitiveDataDetectorTests` | Correo, tarjeta (Luhn), teléfono, cédula, credenciales, clave de API, falsos positivos (apoya P11) | 18 |
| `RequestLimiterTests` | Permite hasta el límite, bloquea y reporta espera, se libera tras la ventana (apoya P12) | 3 |
| `UserProfileTests` | Iniciales del avatar | 5 |

**Resultado de la ejecución:** _(pegar aquí la salida de `dotnet test`: «Correctas: N, Con error: 0»)_

## 3.3 Registro de fallos corregidos

| Fecha | Caso | Falla encontrada | Corrección (commit) |
|---|---|---|---|
| | | | |
