# Importante

Seguimiento del proyecto se encuentra en https://github.com/MRCuevasCarreno/ExchangeRatesAPI

# ASP.NET Core 8 & EntityFramework Core 8

This template is a simple startup project to start with ABP
using ASP.NET Core and EntityFramework Core.

## Requerimiento

Se debe desarrollar una API RESTful utilizando .NET 8 que actúe como proxy hacia la API de Frankfurter para manejar tasas de cambio de divisas. Utilizando los conocimientos adquiridos en el curso, se deben obtener tasas de cambio desde la API de Frankfurter, almacenarlas en una base de datos SQL Server utilizando Entity Framework, y exponer datos derivados adicionales. Se deben implementar operaciones CRUD completas para gestionar estos datos y proporcionar endpoints que transformen y analicen la información obtenida.
Requerimientos del Proyecto:
Tabla de Monedas (Currency):
•	Columns:
o	Id (int, Primary Key)
o	Symbol (string)
o	Name (string)
Tabla de Tasas de Cambio (ExchangeRates):
•	Columns:
o	Id (int, Primary Key)
o	BaseCurrency (string, Foreign Key to Currency) NOK
o	TargetCurrency (string, Foreign Key to Currency)
o	Rate (decimal)
o	Date (DateTime)

Creación de la Solución:
Crear una solución en .NET 8 que incluya un proyecto de Web API utilizando Minimal APIs o Controllers (a preferencia de ustedes). OK
Consulta a la API de Frankfurter:
Implementar endpoints que permitan obtener tasas de cambio desde la API de Frankfurter y almacenarlas en la base de datos. NOK
Los endpoints deben ser capaces de manejar diferentes tipos de consultas (tasas de cambio más recientes, históricas, series temporales y conversiones). OK
https://api.frankfurter.app/ OK
curl --location 'https://localhost:7132/ExchangeRates/latest' \
--header 'accept: */*'
https://api.frankfurter.app/2000-01-99 OK
curl --location 'https://localhost:7132/ExchangeRates/historical/10-01-2024' \
--header 'accept: */*'
https://api.frankfurter.app/2010-01-01..2010-01-31 OK
curl --location 'https://localhost:7132/ExchangeRates/history?startDate=10-01-2024&endDate=10-05-2024' \
--header 'accept: */*'

Endpoints CRUD:
Implementar los siguientes endpoints para gestionar las tasas de cambio almacenadas:
Ejemplo:
o	GET /rates: Devuelve la lista de tasas de cambio almacenadas. OK
curl --location 'https://localhost:7132/rates \
--header 'accept: */*'
o	GET /rates/{id}: Devuelve una tasa de cambio específica por Id. OK
curl --location 'https://localhost:7132/rates/101' \
--header 'accept: */*'
o	POST /rates: Crea nuevas tasas de cambio. OK
curl --location 'https://localhost:7132/rates' \
--header 'accept: */*' \
--header 'Content-Type: application/json' \
--data '{
  "id": "102",
  "symbol": "P",
  "name": "PruebaBorrar"
}'
o	PUT /rates/{id}: Actualiza una tasa de cambio existente por Id. OK
curl --location --request PUT 'https://localhost:7132/rates/101' \
--header 'accept: */*' \
--header 'Content-Type: application/json' \
--data '{
  "id": "100",
  "symbol": "L",
  "name": "Luna2"
}'
o	DELETE /rates/{id}: Elimina una tasa de cambio por Id. OK
curl --location --request DELETE 'https://localhost:7132/rates/103' \
--header 'accept: */*'
o	GET /rates/currency/{baseCurrency}: Devuelve las tasas de cambio por moneda base. NOK
o	PUT /rates/currency/{baseCurrency}: Actualiza las tasas de cambio por moneda base (requiere pasar un body con los datos a actualizar). NOK
o	DELETE /rates/currency/{baseCurrency}: Elimina las tasas de cambio por moneda base. NOK
Transformación de Datos:
Implementar endpoints que realicen cálculos sobre los datos almacenados:
o	GET /rates/average?base={baseCurrency}&target={targetCurrency}&start={startDate}&end={endDate}: Devuelve el valor promedio de la tasa de cambio entre las fechas especificadas. NOK
o	GET /rates/minmax?base={baseCurrency}&target={targetCurrency}&start={startDate}&end={endDate}: Devuelve los valores mínimo y máximo de la tasa de cambio entre las fechas especificadas. NOK
Autenticación:
Implementar un sistema de autenticación utilizando Json Web Tokens (JWT) para proteger los endpoints CRUD. NOK
Caché:
Configurar caché en memoria para mejorar el rendimiento de las solicitudes GET. OK
Subida a GitHub:
Subir el código fuente a un repositorio de GitHub para su revisión. NOK

FRANKFURTER:
Documentación: https://www.frankfurter.app/docs/
API Host: https://api.frankfurter.app


## Como lanzar

* Abrir la solución Visual Studio 2017
* Run the application.
