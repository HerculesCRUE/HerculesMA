![](./media/CabeceraDocumentosMD.png)

| Fecha                 | 29/8/2022                                    |
| --------------------- | -----------------------------------------    |
| Título                | Funcionamiento de los ROs Vinculados         |
| Descripción           | Guía de funcionamiento de los ROs vinculados |
| Versión               | 1.0                                          |
| Módulo                | Documentación                                |
| Tipo                  | Especificación                               |
| Cambios de la Versión | Versión inicial                              |

# Funcionamiento de los ROs vinculados

## Descripción
Los ROs vinculados son relaciones que se pueden establecer en las Publicaciones y en los Research Objects (o RO) con otras Publicaciones o Research Objects. Los ROs vinculados se crean en "una dirección", pero la relación es bi-direccional entre ambos ROs.
### Información ontológica
Los ROs vinculados son una relación entre las ontologías de las Publicaciones / Research Objects con las mismas (document.owl  y researchobject.owl).
### Resumen funcional
Para establecer una vinculación entre ROs, el personal investigador debe entrar en un RO en el cual es investigador, y mediante una opción del menú 


## Listado de los ROs vinculados en un RO
Al acceder como usuario loggeado a una publicacion o RO aparecera una pestaña nueva llamada "ROs vinculados" siempre y ucando haya algún RO vinculado, donde se mostrará un listado de ROs que se encuentren vinculados, los ROs mostrados serán únicamente aquellos que se encuentren validados.

### Resumen funcional
Para acceder a esta pestaña se tiene que acceder antes a una publicacion o RO, y debajo de la descripcion y etiquetas, en la sección con pestañas, aparece la pestaña de ROs vinculados.

### Resumen del desarrollo
La lógica javascript se encuentra en el js de las vistas de publicacion y ROs.
Se realizan llamadas al servicio de búsquedas de GNOSS para mostrar los ROs vinculados, los principales datos de la búsqueda personalizada son los siguientes:

- **Búsqueda personalizada:** linkedROs
- **RdfTypes:** "researchobject" y "document"
- **Parámetro Búsqueda personalizada:** id del RO actual
- **Facetas:** Los mismos que los Research Object y las Publicaciones

## Creación de un RO vinculado
Para la creación de una RO vinculado, debes de estar loguedado como personal investigador.

### Resumen funcional
Se debe acceder a una publicacion o RO, despues en la esquina superior derecha hay un menú desplegable de acciones, una de esas acciones se llama "Nuevo RO vinculado", al pincharla se abre un modal donde puedes introducir buscar un RO en el que eres investigador y seleccionarlo.

### Resumen del desarrollo

## Permisos
Los permisos para la edición, creación y borrado de Los ROs vinculados requieren ser un usuario registrado.
- Creación: Los usuarios investigadores del RO.
- Edición: Los usuarios investigadores del RO.
- Borrado: Los usuarios investigadores del RO.
- Listado: Los usuarios investigadores del RO.
