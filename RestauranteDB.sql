-- Creación de Base de Datos
CREATE DATABASE RestauranteDB;
GO
USE RestauranteDB;
GO

-- Tabla de Clientes
CREATE TABLE Cliente (
    ClienteId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Documento NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100),
    Telefono NVARCHAR(20)
);

-- Tabla de Productos
CREATE TABLE Producto (
    ProductoId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Categoria NVARCHAR(50)
);

-- Tabla de Pedido
CREATE TABLE Pedido (
    PedidoId INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    FechaPedido DATETIME DEFAULT GETDATE(),
    Estado NVARCHAR(20) DEFAULT 'Generado',
    SubTotal DECIMAL(10,2) DEFAULT 0,
    Descuento DECIMAL(5,2) DEFAULT 0,
    Impuesto DECIMAL(5,2) DEFAULT 0,
    Recargo DECIMAL(10,2) DEFAULT 0,
    Total DECIMAL(10,2) DEFAULT 0,
    FOREIGN KEY (ClienteId) REFERENCES Cliente(ClienteId)
);

-- Tabla de DetallePedido
CREATE TABLE DetallePedido (
    DetalleId INT IDENTITY(1,1) PRIMARY KEY,
    PedidoId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    TotalLinea AS (Cantidad * PrecioUnitario) PERSISTED,
    FOREIGN KEY (PedidoId) REFERENCES Pedido(PedidoId),
    FOREIGN KEY (ProductoId) REFERENCES Producto(ProductoId)
);

-- Tabla de DocumentoPago
CREATE TABLE DocumentoPago (
    DocumentoId INT IDENTITY(1,1) PRIMARY KEY,
    PedidoId INT NOT NULL,
    FechaDocumento DATETIME DEFAULT GETDATE(),
    Estado NVARCHAR(20) DEFAULT 'Generado',
    Total DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (PedidoId) REFERENCES Pedido(PedidoId)
);
GO
