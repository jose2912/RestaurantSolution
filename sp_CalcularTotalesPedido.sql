use RestauranteDB
go
-- SP para calcular totales de un pedido
CREATE OR ALTER PROCEDURE sp_CalcularTotalesPedido
    @PedidoId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SubTotal DECIMAL(10,2) = 0;
    DECLARE @Descuento DECIMAL(5,2) = 0;
    DECLARE @Impuesto DECIMAL(5,2) = 0;
    DECLARE @Recargo DECIMAL(10,2) = 0;
    DECLARE @Total DECIMAL(10,2) = 0;

    -- SubTotal
    SELECT @SubTotal = ISNULL(SUM(TotalLinea),0)
    FROM DetallePedido
    WHERE PedidoId = @PedidoId;

    -- Valores del pedido
    SELECT @Descuento = ISNULL(Descuento,0),
           @Impuesto = ISNULL(Impuesto,0),
           @Recargo = ISNULL(Recargo,0)
    FROM Pedido
    WHERE PedidoId = @PedidoId;

    -- Calcular Total
    SET @Total = (@SubTotal - (@SubTotal * @Descuento/100))
                 + ((@SubTotal - (@SubTotal * @Descuento/100)) * @Impuesto/100)
                 + @Recargo;

    -- Actualizar Pedido
    UPDATE Pedido
    SET SubTotal = @SubTotal,
        Total = @Total
    WHERE PedidoId = @PedidoId;
END;
GO
-- =============================================
-- SP: Obtener Pedido por Id
-- =============================================
CREATE OR ALTER PROCEDURE sp_ObtenerPedidoPorId
    @PedidoId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT p.PedidoId, p.ClienteId, p.FechaPedido, p.Estado,
           p.SubTotal, p.Descuento, p.Impuesto, p.Recargo, p.Total
    FROM Pedido p
    WHERE p.PedidoId = @PedidoId;
END;
GO

-- =============================================
-- SP: Crear Pedido
-- =============================================
CREATE OR ALTER PROCEDURE sp_CrearPedido
    @FechaPedido DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Pedido (FechaPedido, Estado)
    VALUES (@FechaPedido, 'Generado');

    SELECT SCOPE_IDENTITY();
END;
GO

-- =============================================
-- SP: Cambiar Estado de Pedido
-- =============================================
CREATE OR ALTER PROCEDURE sp_CambiarEstadoPedido
    @PedidoId INT,
    @Estado NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Pedido
    SET Estado = @Estado
    WHERE PedidoId = @PedidoId;
END;
GO
-- SP: Agregar Detalle de Pedido
CREATE OR ALTER PROCEDURE sp_AgregarDetallePedido
    @PedidoId INT,
    @ProductoId INT,
    @Cantidad INT,
    @PrecioUnitario DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO DetallePedido (PedidoId, ProductoId, Cantidad, PrecioUnitario)
    VALUES (@PedidoId, @ProductoId, @Cantidad, @PrecioUnitario);
END;
GO

-- SP: Generar Documento de Pago
CREATE OR ALTER PROCEDURE sp_GenerarDocumentoPago
    @PedidoId INT,
    @FechaDocumento DATETIME,
    @Estado NVARCHAR(20),
    @Total DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Pedido WHERE PedidoId = @PedidoId)
    BEGIN
        INSERT INTO DocumentoPago (PedidoId, FechaDocumento, Estado, Total)
        VALUES (@PedidoId, @FechaDocumento, @Estado, @Total);

        SELECT SCOPE_IDENTITY(); -- devuelve el nuevo DocumentoId
    END
    ELSE
    BEGIN
        -- No lanzar error, devolver NULL
        SELECT NULL;
    END
END;
GO


-- SP: Obtener detalles de un pedido
CREATE OR ALTER PROCEDURE sp_ObtenerDetallesPorPedido
    @PedidoId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DetalleId, PedidoId, ProductoId, Cantidad, PrecioUnitario
    FROM DetallePedido
    WHERE PedidoId = @PedidoId;
END;
GO

-- SP: Eliminar detalle de un pedido
CREATE OR ALTER PROCEDURE sp_EliminarDetallePedido
    @DetalleId INT
AS
BEGIN
    -- Quita esta línea si quieres contar filas
    -- SET NOCOUNT ON;

    DELETE FROM DetallePedido WHERE DetalleId = @DetalleId;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarPedido
    @PedidoId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el pedido tiene documentos asociados
    IF EXISTS (SELECT 1 FROM DocumentoPago WHERE PedidoId = @PedidoId)
    BEGIN
        -- No eliminar, devolver 0 filas afectadas
        RETURN 0;
    END

    -- Eliminar detalles primero
    DELETE FROM DetallePedido WHERE PedidoId = @PedidoId;

    -- Eliminar pedido
    DELETE FROM Pedido WHERE PedidoId = @PedidoId;

    RETURN @@ROWCOUNT; -- devuelve 1 si se eliminó, 0 si no
END;
GO


CREATE OR ALTER PROCEDURE [dbo].[sp_CambiarEstadoDocumento]
    @DocumentoId INT,
    @Estado NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM DocumentoPago WHERE DocumentoId = @DocumentoId)
    BEGIN
        UPDATE DocumentoPago
        SET Estado = @Estado
        WHERE DocumentoId = @DocumentoId;

        RETURN 1; -- documento encontrado
    END
    ELSE
    BEGIN
        RETURN 0; -- documento no existe
    END
END;

--Registro de datos
INSERT INTO [RestauranteDB].[dbo].[Producto] (Nombre, PrecioUnitario, Categoria)
VALUES 
('Pizza', 25.00, 'Comida'),
('Ceviche', 40.00, 'Comida'),
('Lomo Saltado', 30.00, 'Comida'),
('Agua Mineral', 3.00, 'Bebida'),
('Inka Cola 500ml', 5.00, 'Bebida');
