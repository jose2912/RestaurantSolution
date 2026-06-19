## Proyecto: Sistema de Gestión de Pedidos y Documentos de Pago  
**Autor:** Jose Luis Guzman Arias  
**Fecha:** Junio 2026  
**Tecnologías:** ASP.NET Core MVC, C#, SQL Server, ADO.NET, SweetAlert2  

---

### Objetivo
Desarrollar un sistema web para un restaurante que permita gestionar pedidos, calcular precuentas y generar documentos de pago, cumpliendo las reglas de negocio y control de flujos solicitados en la evaluación técnica.

---

### Arquitectura del proyecto
| Capa | Descripción |
|------|--------------|
| **Restaurant.Entities** | Contiene las clases de dominio (`Pedido`, `DetallePedido`, `DocumentoPago`). |
| **Restaurant.Data** | Acceso a datos mediante `SqlConnection` y `SqlCommand`. Incluye `PedidoRepository` y `DocumentoRepository`. |
| **Restaurant.Business** | Lógica de negocio y conexión entre repositorios y controladores (`PedidoService`, `DocumentoService`). |
| **Restaurant.Web** | Capa de presentación con controladores MVC y vistas Razor. |

---

### Principales componentes
- **Procedimientos almacenados (SQL Server):**
  - `sp_CrearPedido`
  - `sp_AgregarDetallePedido`
  - `sp_CalcularTotalesPedido`
  - `sp_GenerarDocumentoPago`
  - `sp_CambiarEstadoDocumento`

- **Controladores:**
  - `PedidosController`: gestiona creación, detalles, precuenta y eliminación.
  - `DocumentosController`: lista documentos y permite cambiar su estado.

- **Vistas:**
  - `Pedidos/Index.cshtml`: lista pedidos y botón *Precuenta*.
  - `Documentos/Index.cshtml`: muestra comprobantes y botones de estado (*Facturar*, *Anular*, *En Proceso*).

---

### Flujo funcional
1. **Creación de pedido** → se guarda en la base de datos.  
2. **Agregar detalles** → productos y cantidades.  
3. **Precuenta** → calcula totales y genera documento de pago.  
4. **Documentos** → muestra comprobantes y permite cambiar estado.  
5. **SweetAlert2** → muestra mensajes de éxito o error en cada acción.  

---

### Reglas de negocio
- Totales calculados con impuestos, descuentos y recargos.  
- Documento de pago generado automáticamente al calcular precuenta.  
- Control de estados: *Generado*, *Facturado*, *Anulado*, *En Proceso*.  
- Validaciones visuales con `TempData` y SweetAlert2.

---

### Resultado final
El sistema cumple con:
- Arquitectura por capas.  
- Separación de responsabilidades.  
- Integración completa entre pedidos y documentos.  
- Control de flujos y estados.  
- Interfaz moderna y funcional.

---

## Instrucciones de instalación y ejecución

### 1. Requisitos previos
- **Visual Studio 2022** o superior  
- **SQL Server 2019** o superior  
- **.NET 6.0 SDK** instalado  
- Conexión local a SQL Server (ejemplo: `localhost\SQLEXPRESS`)  

---

### 2. Configuración de la base de datos
1. Crear una base de datos llamada `RestaurantDB`.  
2. Ejecutar los scripts SQL incluidos en la carpeta `Database` o en tu repositorio:  
   - `sp_CrearPedido.sql`  
   - `sp_AgregarDetallePedido.sql`  
   - `sp_CalcularTotalesPedido.sql`  
   - `sp_GenerarDocumentoPago.sql`  
   - `sp_CambiarEstadoDocumento.sql`  

---

### 3. Configuración de la cadena de conexión
En el archivo **`appsettings.json`** de `Restaurant.Web`, ajustar la cadena de conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=RestaurantDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
