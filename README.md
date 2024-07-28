# Importante

Seguimiento del proyecto se encuentra en https://github.com/MRCuevasCarreno/ExchangeRatesAPI

# ASP.NET Core 8 & EntityFramework Core 8

Proyecto .NET Core 8 de curso realizado en Socius Chile.

## Requerimiento

Se debe desarrollar una API RESTful utilizando .NET 8 que actúe como proxy hacia la API de Frankfurter para manejar tasas de cambio de divisas. Utilizando los conocimientos adquiridos en el curso, se deben obtener tasas de cambio desde la API de Frankfurter, almacenarlas en una base de datos SQL Server utilizando Entity Framework, y exponer datos derivados adicionales. Se deben implementar operaciones CRUD completas para gestionar estos datos y proporcionar endpoints que transformen y analicen la información obtenida.

![Captura de pantalla 2024-07-23 095456](https://github.com/user-attachments/assets/3fa9c8ff-7fa3-45eb-9a27-82fe89408488)


# Creación de la Solución:
Crear una solución en .NET 8 que incluya un proyecto de Web API utilizando Minimal APIs o Controllers (a preferencia de ustedes). 
*******
# Consulta a la API de Frankfurter:
Implementar endpoints que permitan obtener tasas de cambio desde la API de Frankfurter y almacenarlas en la base de datos. 

Los endpoints deben ser capaces de manejar diferentes tipos de consultas (tasas de cambio más recientes, históricas, series temporales y conversiones). 
https://api.frankfurter.app/ 

https://api.frankfurter.app/2000-01-99 

https://api.frankfurter.app/2010-01-01..2010-01-31 
*******


# Endpoints CRUD:
Implementar los siguientes endpoints para gestionar las tasas de cambio almacenadas:
Ejemplo:
GET /rates: Devuelve la lista de tasas de cambio almacenadas. 

GET /rates/{id}: Devuelve una tasa de cambio específica por Id. 

POST /rates: Crea nuevas tasas de cambio. 

PUT /rates/{id}: Actualiza una tasa de cambio existente por Id. 

DELETE /rates/{id}: Elimina una tasa de cambio por Id. 

GET /rates/currency/{baseCurrency}: Devuelve las tasas de cambio por moneda base. 

PUT /rates/currency/{baseCurrency}: Actualiza las tasas de cambio por moneda base (requiere pasar un body con los datos a actualizar). 

DELETE /rates/currency/{baseCurrency}: Elimina las tasas de cambio por moneda base. 

*******

# Transformación de Datos:
Implementar endpoints que realicen cálculos sobre los datos almacenados:

GET /rates/average?base={baseCurrency}&target={targetCurrency}&start={startDate}&end={endDate}: Devuelve el valor promedio de la tasa de cambio entre las fechas especificadas. 

GET /rates/minmax?base={baseCurrency}&target={targetCurrency}&start={startDate}&end={endDate}: Devuelve los valores mínimo y máximo de la tasa de cambio entre las fechas especificadas. 

*******
# Autenticación:
Implementar un sistema de autenticación utilizando Json Web Tens (JWT) para proteger los endpoints CRUD. 
Caché:

Configurar caché en memoria para mejorar el rendimiento de las solicitudes GET. 
Subida a GitHub:

Subir el código fuente a un repositorio de GitHub para su revisión. 

# FRANKFURTER:

Documentación: https://www.frankfurter.app/docs/

API Host: https://api.frankfurter.app


## Como lanzar

* Abrir la solución Visual Studio 2017
* Correr la aplicación.
* Utilizar el Swagger.
