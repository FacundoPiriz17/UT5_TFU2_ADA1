using UT5_TFU.Dominio;

namespace UT5_TFU.Repositories;

public sealed class FoodTruckEnMemoriaRepository : IFoodTruckRepository
{
    private readonly object sync = new();
    private readonly List<PuestoComida> puestosComida;
    private readonly List<Producto> productos;
    private readonly List<Pedido> pedidos;
    private int nextOrderId = 4;

    public FoodTruckEnMemoriaRepository()
    {
        puestosComida = new List<PuestoComida>
        {
            new()
            {
                Id = 1,
                Nombre = "Gluten Lovers",
                Descripcion = "Hamburguesas artesanales y snacks para todos los gustos.",
                LogoUrl = "/assets/food-trucks/gluten-lovers.png"
            },
            new()
            {
                Id = 2,
                Nombre = "Sunset Food",
                Descripcion = "Sabores callejeros con ingredientes frescos y mucha onda.",
                LogoUrl = "/assets/food-trucks/sunset-food.png"
            },
            new()
            {
                Id = 3,
                Nombre = "The Rolling Snack",
                Descripcion = "Bocaditos, papas y comidas rapidas para cualquier momento.",
                LogoUrl = "/assets/food-trucks/the-rolling-snack.png"
            }
        };

        productos = new List<Producto>
        {
            new()
            {
                Id = 1,
                PuestoComidaId = 1,
                Nombre = "Tacos al Pastor",
                Descripcion = "3 tacos con carne al pastor, pina, cilantro y cebolla.",
                ImagenUrl = "/assets/productos/tacos-al-pastor.png",
                Precio = 350,
                Stock = 18
            },
            new()
            {
                Id = 2,
                PuestoComidaId = 1,
                Nombre = "Bacon Deluxe",
                Descripcion = "Doble carne, bacon, queso cheddar y cebolla caramelizada.",
                ImagenUrl = "/assets/productos/bacon-deluxe.png",
                Precio = 450,
                Stock = 15
            },
            new()
            {
                Id = 3,
                PuestoComidaId = 1,
                Nombre = "Papas Fritas Supreme",
                Descripcion = "Papas fritas con bacon y queso cheddar.",
                ImagenUrl = "/assets/productos/papas-fritas-supreme.png",
                Precio = 250,
                Stock = 24
            },
            new()
            {
                Id = 4,
                PuestoComidaId = 1,
                Nombre = "BBQ Burger",
                Descripcion = "Hamburguesa con salsa BBQ, aros de cebolla y queso.",
                ImagenUrl = "/assets/productos/bbq-burger.png",
                Precio = 450,
                Stock = 12
            },
            new()
            {
                Id = 5,
                PuestoComidaId = 1,
                Nombre = "Milanesa Completa",
                Descripcion = "Milanesa de carne con papas fritas.",
                ImagenUrl = "/assets/productos/milanesa-completa.png",
                Precio = 450,
                Stock = 10
            },
            new()
            {
                Id = 6,
                PuestoComidaId = 1,
                Nombre = "Choripan",
                Descripcion = "Chorizo artesanal con chimichurri y pan casero.",
                ImagenUrl = "/assets/productos/choripan.png",
                Precio = 300,
                Stock = 20
            },
            new()
            {
                Id = 7,
                PuestoComidaId = 2,
                Nombre = "Sunset Burger",
                Descripcion = "Hamburguesa callejera con salsa especial.",
                ImagenUrl = "/assets/productos/sunset-burger.png",
                Precio = 450,
                Stock = 14
            },
            new()
            {
                Id = 8,
                PuestoComidaId = 2,
                Nombre = "Loaded Nachos",
                Descripcion = "Nachos con cheddar, pico de gallo y crema.",
                ImagenUrl = "/assets/productos/loaded-nachos.png",
                Precio = 380,
                Stock = 16
            },
            new()
            {
                Id = 9,
                PuestoComidaId = 2,
                Nombre = "Street Wrap",
                Descripcion = "Wrap de pollo, vegetales frescos y salsa de la casa.",
                ImagenUrl = "/assets/productos/street-wrap.png",
                Precio = 420,
                Stock = 18
            },
            new()
            {
                Id = 10,
                PuestoComidaId = 3,
                Nombre = "Rolling Combo",
                Descripcion = "Mini burgers con papas rusticas.",
                ImagenUrl = "/assets/productos/rolling-combo.png",
                Precio = 500,
                Stock = 11
            },
            new()
            {
                Id = 11,
                PuestoComidaId = 3,
                Nombre = "Papas Rusticas",
                Descripcion = "Papas crocantes con salsa cheddar.",
                ImagenUrl = "/assets/productos/papas-rusticas.png",
                Precio = 250,
                Stock = 22
            },
            new()
            {
                Id = 12,
                PuestoComidaId = 3,
                Nombre = "Bocaditos Mixtos",
                Descripcion = "Bocaditos calientes para compartir.",
                ImagenUrl = "/assets/productos/bocaditos-mixtos.png",
                Precio = 390,
                Stock = 13
            }
        };

        pedidos = new List<Pedido>
        {
            ConstruirPedidoSemilla(
                id: 1,
                clienteId: 1,
                nombreCliente: "Miguel",
                puestoComidaId: 2,
                estado: EstadoPedido.EnPreparacion,
                pago: new Pago
                {
                    Metodo = MetodoPago.Efectivo,
                    Estado = EstadoPago.Pendiente,
                    Monto = 450,
                    FechaProcesado = new DateTimeOffset(2026, 6, 2, 16, 15, 0, TimeSpan.Zero)
                },
                fechaCreacion: new DateTimeOffset(2026, 6, 2, 16, 15, 0, TimeSpan.Zero),
                fechaEntrega: null,
                ultimaNotificacion: null,
                detalles: new[]
                {
                    new DetallePedido
                    {
                        ProductoId = 7,
                        NombreProducto = "Sunset Burger",
                        Cantidad = 1,
                        PrecioUnitario = 450
                    }
                }),
            ConstruirPedidoSemilla(
                id: 2,
                clienteId: 1,
                nombreCliente: "Miguel",
                puestoComidaId: 1,
                estado: EstadoPedido.EnPreparacion,
                pago: new Pago
                {
                    Metodo = MetodoPago.MercadoPago,
                    Estado = EstadoPago.Pagado,
                    Monto = 1050,
                    FechaProcesado = new DateTimeOffset(2026, 6, 2, 16, 20, 0, TimeSpan.Zero)
                },
                fechaCreacion: new DateTimeOffset(2026, 6, 2, 16, 20, 0, TimeSpan.Zero),
                fechaEntrega: null,
                ultimaNotificacion: null,
                detalles: new[]
                {
                    new DetallePedido
                    {
                        ProductoId = 2,
                        NombreProducto = "Bacon Deluxe",
                        Cantidad = 1,
                        PrecioUnitario = 450
                    },
                    new DetallePedido
                    {
                        ProductoId = 6,
                        NombreProducto = "Choripan",
                        Cantidad = 2,
                        PrecioUnitario = 300
                    }
                }),
            ConstruirPedidoSemilla(
                id: 3,
                clienteId: 2,
                nombreCliente: "Valentina",
                puestoComidaId: 3,
                estado: EstadoPedido.Entregado,
                pago: new Pago
                {
                    Metodo = MetodoPago.Tarjeta,
                    Estado = EstadoPago.Pagado,
                    Monto = 500,
                    FechaProcesado = new DateTimeOffset(2026, 6, 1, 20, 5, 0, TimeSpan.Zero)
                },
                fechaCreacion: new DateTimeOffset(2026, 6, 1, 20, 5, 0, TimeSpan.Zero),
                fechaEntrega: new DateTimeOffset(2026, 6, 1, 20, 25, 0, TimeSpan.Zero),
                ultimaNotificacion: "Pedido de The Rolling Snack listo para retirar.",
                detalles: new[]
                {
                    new DetallePedido
                    {
                        ProductoId = 10,
                        NombreProducto = "Rolling Combo",
                        Cantidad = 1,
                        PrecioUnitario = 500
                    }
                })
        };
    }

    public IReadOnlyList<PuestoComida> ObtenerPuestosComida()
    {
        lock (sync)
        {
            return puestosComida.ToList();
        }
    }

    public PuestoComida? ObtenerPuestoComida(int id)
    {
        lock (sync)
        {
            return puestosComida.FirstOrDefault(puestoComida => puestoComida.Id == id);
        }
    }

    public IReadOnlyList<Producto> ObtenerProductosPorPuestoComida(int puestoComidaId)
    {
        lock (sync)
        {
            return productos
                .Where(producto => producto.PuestoComidaId == puestoComidaId)
                .ToList();
        }
    }

    public Producto? ObtenerProducto(int puestoComidaId, int productoId)
    {
        lock (sync)
        {
            return productos.FirstOrDefault(producto =>
                producto.PuestoComidaId == puestoComidaId && producto.Id == productoId);
        }
    }

    public IReadOnlyList<Pedido> ObtenerPedidos()
    {
        lock (sync)
        {
            return OrdenarPedidos(pedidos).ToList();
        }
    }

    public IReadOnlyList<Pedido> ObtenerPedidosPorCliente(int clienteId)
    {
        lock (sync)
        {
            return OrdenarPedidos(pedidos.Where(pedido => pedido.ClienteId == clienteId)).ToList();
        }
    }

    public IReadOnlyList<Pedido> ObtenerPedidosPorPuestoComida(int puestoComidaId, EstadoPedido? estado = null)
    {
        lock (sync)
        {
            var query = pedidos.Where(pedido => pedido.PuestoComidaId == puestoComidaId);

            if (estado is not null)
            {
                query = query.Where(pedido => pedido.Estado == estado);
            }

            return OrdenarPedidos(query).ToList();
        }
    }

    public Pedido? ObtenerPedido(int id)
    {
        lock (sync)
        {
            return pedidos.FirstOrDefault(pedido => pedido.Id == id);
        }
    }

    public Pedido AgregarPedido(Pedido pedido)
    {
        lock (sync)
        {
            pedido.Id = nextOrderId;
            nextOrderId++;
            pedidos.Add(pedido);
            return pedido;
        }
    }

    public void ActualizarPedido(Pedido pedido)
    {
        lock (sync)
        {
            var indice = pedidos.FindIndex(existente => existente.Id == pedido.Id);
            if (indice >= 0)
            {
                pedidos[indice] = pedido;
            }
        }
    }

    private static IOrderedEnumerable<Pedido> OrdenarPedidos(IEnumerable<Pedido> pedidos)
    {
        return pedidos.OrderByDescending(pedido => pedido.FechaCreacion);
    }

    private static Pedido ConstruirPedidoSemilla(
        int id,
        int clienteId,
        string nombreCliente,
        int puestoComidaId,
        EstadoPedido estado,
        Pago pago,
        DateTimeOffset fechaCreacion,
        DateTimeOffset? fechaEntrega,
        string? ultimaNotificacion,
        IEnumerable<DetallePedido> detalles)
    {
        var pedido = new Pedido
        {
            Id = id,
            ClienteId = clienteId,
            NombreCliente = nombreCliente,
            PuestoComidaId = puestoComidaId,
            Estado = estado,
            Pago = pago,
            FechaCreacion = fechaCreacion,
            FechaEntrega = fechaEntrega,
            UltimaNotificacion = ultimaNotificacion
        };

        pedido.Detalles.AddRange(detalles);
        return pedido;
    }
}
