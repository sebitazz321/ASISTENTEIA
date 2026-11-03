# 15. Manual de usuario

AsistenteIA es un asistente académico: envía tus preguntas a un modelo de inteligencia artificial, muestra la
respuesta y guarda un historial **solo en tu dispositivo**. Funciona en Windows y Android.

## 15.1 Primer uso

1. Abre la app. La primera vez aparece el aviso **«Uso responsable de la IA»**; léelo y pulsa **Entendido**.
2. En **Inicio** pulsa **Configurar clave y modelo** (o ve a **Perfil › Configuración**).
3. Elige el **proveedor**: *Google Gemini* o *Pollinations*.
4. Escribe tu **clave de API** (se oculta) y elige un **modelo** sugerido.
5. Pulsa **Guardar** y luego **Probar conexión**. Debe aparecer «✔ Conexión exitosa con {proveedor}».

> Clave de Gemini: Google AI Studio (aistudio.google.com/apikey). Clave de Pollinations: enter.pollinations.ai.

## 15.2 Pantallas (ver capturas en el capítulo 8)

| Pantalla | Para qué sirve | Acciones |
|---|---|---|
| **Inicio** | Bienvenida con tu nombre, accesos rápidos y recomendaciones. | Nueva consulta, Mis proyectos, Historial, Mi perfil, Configurar clave y modelo. |
| **Consultar** | Hacer preguntas a la IA. | Elegir plantilla y proyecto, escribir (contador hasta 2000), **Consultar**, **Cancelar**, **Guardar**. |
| **Proyectos** | Agrupar consultas por materia o trabajo. | **+ Nuevo proyecto**, abrir un proyecto. |
| **Detalle de proyecto** | Ver y editar un proyecto. | Editar nombre (3–60) y descripción (máx. 300), ver sus consultas, **Consultar en este proyecto**, eliminar. |
| **Historial** | Ver consultas guardadas. | Buscar (en pregunta, respuesta, categoría o proyecto), **Solo verificadas**, abrir detalle, **Eliminar**. |
| **Detalle de consulta** | Revisar una consulta. | **Marcar como verificada / Quitar verificación**, escribir una **nota personal** (máx. 500) y pulsar **Guardar nota**, **Eliminar consulta**. |
| **Perfil** | Tus datos y estadísticas. | Nombre, programa e institución; contadores de consultas, verificadas y proyectos; Configuración; **Borrar historial de consultas**; **Borrar todos mis datos locales**. |
| **Configuración** | Proveedor, clave y modelo. | Selector de proveedor, clave oculta, modelos, Guardar, Probar conexión, Eliminar clave; avisos de privacidad y «Acerca de». |

## 15.3 Hacer una consulta

1. Ve a **Consultar**.
2. (Opcional) Elige una **plantilla**: Pregunta libre, Explicar un concepto, Resumir un texto, Comparar dos temas, Preguntas de práctica o Revisar código. El texto aparece en el editor y puedes cambiarlo.
3. (Opcional) Elige un **proyecto**: su nombre y descripción se envían como contexto.
4. Escribe tu pregunta (5 a 2000 caracteres) y pulsa **Consultar**. Verás «Consultando {proveedor}…».
5. Si tarda, pulsa **Cancelar**. La espera máxima es 60 segundos.
6. Lee la respuesta, **verifícala** con fuentes confiables y pulsa **Guardar** para añadirla al historial.

**Avisos que puedes ver**

| Aviso | Qué significa |
|---|---|
| «Posibles datos sensibles…» | Tu texto parece tener un correo, teléfono, cédula, tarjeta, contraseña o clave. Pulsa **Revisar** para quitarlos o **Enviar** si estás seguro. |
| «Límite de uso alcanzado: espere N s» | Hiciste más de 8 consultas en un minuto. |
| «[Respuesta recortada…]» | La respuesta superó el límite de longitud: pide un resumen o divide la pregunta. |

## 15.4 Cambiar de proveedor

En **Configuración** elige el otro proveedor, escribe su clave (solo la primera vez), elige el modelo y pulsa **Guardar**.
Cada proveedor guarda su propia clave y su propio modelo. La línea «Proveedor activo» muestra cuál se está usando.

## 15.5 Privacidad y datos

- Tu clave se guarda cifrada en el dispositivo y nunca se muestra.
- El historial, los proyectos y el perfil se guardan solo en tu dispositivo. El perfil **no** se envía a la IA.
- Lo único que se envía al proveedor es el texto de la consulta (y el contexto del proyecto, si elegiste uno).
- **Borrar historial de consultas** elimina las consultas; **Borrar todos mis datos locales** elimina consultas, proyectos y perfil. La clave se elimina en Configuración.

## 15.6 Recomendaciones

- No escribas datos personales, contraseñas ni información confidencial.
- La IA puede equivocarse o inventar datos: márcala como verificada solo después de contrastarla.
- Si ves un error, consulta el Manual de solución de problemas (capítulo 17).
