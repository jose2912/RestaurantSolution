use RestauranteDB
go
-- SP para calcular totales de un pedido
CREATE PROCEDURE sp_CalcularTotalesPedido
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
CREATE PROCEDURE sp_ObtenerPedidoPorId
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
CREATE PROCEDURE sp_CrearPedido
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
CREATE PROCEDURE sp_CambiarEstadoPedido
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
CREATE PROCEDURE sp_AgregarDetallePedido
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
--CREATE PROCEDURE sp_GenerarDocumentoPago
--    @PedidoId INT
--AS
--BEGIN
--    SET NOCOUNT ON;

--    DECLARE @Total DECIMAL(10,2);

--    SELECT @Total = Total
--    FROM Pedido
--    WHERE PedidoId = @PedidoId;

--    INSERT INTO DocumentoPago (PedidoId, Total)
--    VALUES (@PedidoId, @Total);

--    SELECT SCOPE_IDENTITY() AS NuevoDocumentoId;
--END;
--GO

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
        RAISERROR('El PedidoId no existe en la tabla Pedido.', 16, 1);
    END
END;
GO

-- SP: Obtener detalles de un pedido
CREATE PROCEDURE sp_ObtenerDetallesPorPedido
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
CREATE PROCEDURE sp_EliminarDetallePedido
    @DetalleId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM DetallePedido
    WHERE DetalleId = @DetalleId;
END;
GO

CREATE OR ALTER PROCEDURE sp_EliminarPedido
    @PedidoId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Eliminar detalles asociados
        DELETE FROM DetallePedido
        WHERE PedidoId = @PedidoId;

        -- 2. Eliminar el pedido
        DELETE FROM Pedido
        WHERE PedidoId = @PedidoId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
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


INSERT INTO [RestauranteDB].[dbo].[Producto] (Nombre, PrecioUnitario, Categoria)
VALUES 
('Pizza', 25.00, 'Comida'),
('Ceviche', 40.00, 'Comida'),
('Lomo Saltado', 30.00, 'Comida'),
('Agua Mineral', 3.00, 'Bebida'),
('Inka Cola 500ml', 5.00, 'Bebida');
