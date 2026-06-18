# FoodTrack — Backend API

API REST desarrollada en **C# .NET Core** para la gestión de pedidos en festivales gastronómicos con food trucks. Trabajo Final de las Unidades Temáticas 5 de Análisis y Diseño de Aplicaciones (ADA/ANDIS).

---

## Índice

1. [Cómo ejecutar](#1-cómo-ejecutar)
2. [Cómo probar los endpoints](#2-cómo-probar-los-endpoints)
3. [Casos de uso implementados](#3-casos-de-uso-implementados)
4. [Endpoints de la API](#4-endpoints-de-la-api)
5. [Principios SOLID aplicados](#5-principios-solid-aplicados)
6. [Patrones de diseño utilizados](#6-patrones-de-diseño-utilizados)
7. [Decisiones técnicas](#7-decisiones-técnicas)
8. [Cambios respecto al modelo de UT3](#8-cambios-respecto-al-modelo-de-ut3)
9. [Qué no se implementó y por qué](#9-qué-no-se-implementó-y-por-qué)

---

## 1. Cómo ejecutar

### Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Pasos

```bash
# Clonar el repositorio
git clone <url-del-repo>
cd UT5_TFU2_ADA1

# Ejecutar la API
dotnet run --project UT5_TFU
```

La API queda disponible en `http://localhost:5183`.

La documentación interactiva (Swagger UI) está en: `http://localhost:5183/swagger`

No se necesita ninguna base de datos ni configuración adicional. Al arrancar, el sistema ya tiene datos cargados: 3 food trucks, sus menús y 3 pedidos de ejemplo.

---

## 2. Cómo probar los endpoints

### Swagger UI

Abrir `http://localhost:5183/swagger` en el navegador. Desde ahí se puede explorar y ejecutar cada endpoint directamente.


## 3. Casos de uso implementados

La selección de casos de uso parte del prototipo de la UT4 y del diagrama de clases de la UT3. Los elegimos porque cubren el flujo completo de uso del sistema: desde que el cliente elige dónde comer hasta que retira su pedido.

| # | Caso de uso | Actor | Endpoint |
|---|---|---|---|
| UC1 | Visualizar food trucks disponibles | Cliente | `GET /api/foodtrucks` |
| UC2 | Ver el menú de un food truck | Cliente | `GET /api/foodtrucks/{id}/menu` |
| UC3 | Registrar un pedido (autoservicio) | Cliente | `POST /api/pedidos` |
| UC4 | Realizar el pago | Cliente / Cajero | `POST /api/pedidos/{id}/pago/confirmar` |
| UC5 | Consultar estado del pedido | Cliente | `GET /api/pedidos/{id}` |
| UC6 | Notificar pedido listo | Sistema (automático) | — disparado internamente al pasar a `ListoParaRetirar` |
| UC7 | Ver pedidos confirmados (vista cocina) | Cocinero | `GET /api/foodtrucks/{id}/pedidos` |
| UC8 | Actualizar estado del pedido | Cocinero / Cajero | `POST /api/pedidos/{id}/preparar`, `/listo`, `/entregar` |

**Por qué elegimos estos casos de uso:** representan el flujo de negocio completo sin solapamientos. UC6 fue una decisión de diseño más que todo, en lugar de ser un endpoint propio, se dispara automáticamente como efecto secundario de UC8 (cuando el cocinero marca el pedido como listo).

---

## 4. Endpoints de la API

### Food Trucks

#### Listar food trucks — UC1

```
GET /api/foodtrucks
```

```bash
curl http://localhost:5183/api/foodtrucks
```

#### Obtener detalle de un food truck

```
GET /api/foodtrucks/{id}
```

```bash
curl http://localhost:5183/api/foodtrucks/1
```

#### Ver menú de un food truck — UC2

```
GET /api/foodtrucks/{id}/menu
```

```bash
curl http://localhost:5183/api/foodtrucks/1/menu
```

#### Ver pedidos de un food truck (vista cocina) — UC7

```
GET /api/foodtrucks/{id}/pedidos
GET /api/foodtrucks/{id}/pedidos?estado=EnPreparacion
```

Hicimos que el parámetro `estado` fuera opcional. 
Los valores posibles son: `Pendiente`, `EnPreparacion`, `ListoParaRetirar`, `Entregado`, `Cancelado`.

```bash
# Todos los pedidos del food truck 2
curl http://localhost:5183/api/foodtrucks/2/pedidos

# Solo los pedidos en preparación
curl "http://localhost:5183/api/foodtrucks/2/pedidos?estado=EnPreparacion"
```

---

### Pedidos

#### Registrar un pedido nuevo — UC3

```
POST /api/pedidos
Content-Type: application/json
```

Body:
```json
{
  "clienteId": 10,
  "nombreCliente": "Juan",
  "puestoComidaId": 1,
  "metodoPago": "Tarjeta",
  "items": [
    { "productoId": 1, "cantidad": 2 },
    { "productoId": 3, "cantidad": 1 }
  ],
  "simularRechazoPago": false
}
```

- `metodoPago`: `"Efectivo"`, `"Tarjeta"` o `"MercadoPago"`
- `simularRechazoPago`: `true` para probar el caso de pago rechazado (el pedido no se registra)
- Los `productoId` deben pertenecer al `puestoComidaId` indicado

ejemplo:
```bash
curl -X POST http://localhost:5183/api/pedidos \
  -H "Content-Type: application/json" \
  -d '{"clienteId":10,"nombreCliente":"Juan","puestoComidaId":1,"metodoPago":"Tarjeta","items":[{"productoId":1,"cantidad":2}],"simularRechazoPago":false}'
```

Respuesta esperada
`201 Created`:
```json
{
  "id": 4,
  "clienteId": 10,
  "nombreCliente": "Juan",
  "estadoPedido": "EnPreparacion",
  "pago": { "metodo": "Tarjeta", "estado": "Pagado", "monto": 700 },
  "progreso": [...]
}
```

#### Consultar detalle y estado de un pedido — UC5

```
GET /api/pedidos/{id}
```

```bash
curl http://localhost:5183/api/pedidos/1
```

La respuesta incluye un array `progreso` que muestra en qué paso del flujo se encuentra el pedido. Esto nos serviría para trackear el estado en el frontend.

#### Consultar pedidos de un cliente — UC5

```
GET /api/clientes/{clienteId}/pedidos
```

```bash
curl http://localhost:5183/api/clientes/1/pedidos
```

---

### Cambios de estado — UC8

Estos tres endpoints representan las acciones que realiza el personal del food truck. No tienen body como tal

#### Cocinero: pasar a En Preparación

```
POST /api/pedidos/{id}/preparar
```

```bash
curl -X POST http://localhost:5183/api/pedidos/1/preparar
```

#### Cocinero: marcar como Listo para Retirar (dispara UC6)

```
POST /api/pedidos/{id}/listo
```

```bash
curl -X POST http://localhost:5183/api/pedidos/1/listo
```

Cuando el pedido pasa a `ListoParaRetirar`, el sistema genera automáticamente la notificación y la incluye en la respuesta bajo el campo `notificacion`.

#### Cajero: registrar entrega

```
POST /api/pedidos/{id}/entregar
```

```bash
curl -X POST http://localhost:5183/api/pedidos/1/entregar
```

---

### Pagos — UC4

#### Confirmar cobro en efectivo

```
POST /api/pedidos/{id}/pago/confirmar
Content-Type: application/json
```

Body:
```json
{
  "simularRechazoPago": false
}
```

Se usa principalmente para los pedidos pagados en efectivo, donde el pago queda `Pendiente` hasta que el cajero confirma el cobro físico. Para tarjeta y MercadoPago el pago ya queda confirmado al crear el pedido.

```bash
curl -X POST http://localhost:5183/api/pedidos/1/pago/confirmar \
  -H "Content-Type: application/json" \
  -d '{"simularRechazoPago": false}'
```

---

### Flujo completo de ejemplo

```
1. GET  /api/foodtrucks             → cliente elige un food truck
2. GET  /api/foodtrucks/2/menu      → cliente ve el menú
3. POST /api/pedidos                → cliente arma y confirma el pedido
4. GET  /api/pedidos/4              → cliente hace tracking del estado
5. POST /api/pedidos/4/preparar     → cocinero empieza a preparar
6. POST /api/pedidos/4/listo        → cocinero termina → se genera notificación automáticamente
7. GET  /api/pedidos/4              → cliente ve "Listo para retirar" y la notificación
8. POST /api/pedidos/4/entregar     → cajero registra la entrega
9. POST /api/pedidos/4/pago/confirmar → cajero confirma cobro si era efectivo
```

---

## 5. Principios SOLID aplicados

### S — Responsabilidad Única (Single Responsibility)

Cada clase tiene una única razón para cambiar:

- `PagoService` solo resuelve qué estrategia de pago usar y delega.
- `NotificacionService` solo sabe construir el texto de una notificación.
- `PedidoListoNotificacionObserver` solo sabe qué hacer cuando un pedido cambia de estado.
- `FoodTruckEnMemoriaRepository` solo maneja la persistencia en memoria.

Si mañana hay que cambiar el texto de las notificaciones, se toca únicamente `NotificacionService`. Si hay que cambiar cómo se persiste un pedido, se toca únicamente el repositorio.

### O — Abierto/Cerrado (Open/Closed)

**Ejemplo de como agregaríamos otro métodos de pago:**

```csharp
//Acá simulamos que agregamos criptomonedas como un método de pago
public sealed class PagoStrategyCripto : IPagoStrategy { ... }

// Y lo registraríamos en Program.cs así:
builder.Services.AddScoped<IPagoStrategy, PagoStrategyCripto>();
```

No se modifica ni `PagoService` ni ningún controller. El sistema está **abierto a extensión** y **cerrado a modificación**.

Lo mismo aplica para los observers: se puede agregar un `EmailNotificacionObserver` o un `SMSNotificacionObserver` sin tocar `PedidoService`.

### L — Sustitución de Liskov (Liskov Substitution)

Las tres estrategias de pago (`PagoStrategyEfectivo`, `PagoStrategyTarjeta`, `PagoStrategyMercadoPagoStrategy`) son intercambiables a través de `IPagoStrategy`. `PagoService` las usa indistintamente sin saber la clase concreta.


### I — Segregación de Interfaces (Interface Segregation)

En lugar de una sola interfaz monolítica, tenemos interfaces específicas:

| Interfaz | Quién la consume |
|---|---|
| `IFoodTruckRepository` | `PedidoService`, `PuestoComidaService` |
| `IPagoService` | `PedidoService` |
| `INotificacionService` | `PedidoListoNotificacionObserver` |
| `IPedidoEstadoObserver` | `PedidoService` |
| `IPagoStrategy` | `PagoService` |

Ninguna clase depende de métodos que no usa.

### D — Inversión de Dependencias (Dependency Inversion)

Todos los componentes dependen de abstracciones (interfaces), no de implementaciones concretas. Lo podemos ver en `Program.cs`:

```csharp
// Program.cs — todos los componentes están registrados por su interfaz
builder.Services.AddSingleton<IFoodTruckRepository, FoodTruckEnMemoriaRepository>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IPedidoEstadoObserver, PedidoListoNotificacionObserver>();
builder.Services.AddScoped<IPagoStrategy, PagoStrategyEfectivo>();
builder.Services.AddScoped<IPagoStrategy, PagoStrategyTarjeta>();
builder.Services.AddScoped<IPagoStrategy, PagoStrategyMercadoPagoStrategy>();
```


---

## 6. Patrones de diseño utilizados

### Strategy — Métodos de Pago

**Problema:** Cada método de pago tiene un comportamiento diferente. El efectivo queda pendiente hasta que el cajero confirma; tarjeta y MercadoPago se resuelven inmediatamente (mock).

**Solución:** La interfaz `IPagoStrategy` define dos operaciones (`Iniciar` y `Confirmar`) y hay una clase concreta por cada método de pago.

```
IPagoStrategy
    ├── PagoStrategyEfectivo       → Iniciar: queda Pendiente | Confirmar: Pagado
    ├── PagoStrategyTarjeta        → Iniciar: Pagado inmediato
    └── PagoStrategyMercadoPago    → Iniciar: Pagado inmediato
```

### Observer — Notificación automática cuando el pedido está listo

**Problema:** Cuando un cocinero marca un pedido como `ListoParaRetirar`, el sistema debe generar una notificación para el cliente. Pero `PedidoService` no debería acoplar lógica de notificaciones.

**Solución:** `PedidoService` publica un evento `PedidoEstadoCambiado` a todos los observers registrados. Cada observer decide si le interesa el evento y qué hacer con él.

```
PedidoService
    → publica PedidoEstadoCambiado
        → PedidoListoNotificacionObserver
            → llama NotificacionService.CrearNotificacionPedidoListo()
            → escribe pedido.UltimaNotificacion = "Pedido de X listo para retirar."
```

---

### Repository — Acceso a datos desacoplado

**Problema:** Los servicios de negocio no deberían saber si los datos viven en memoria, en SQL Server o en cualquier otro origen.

**Solución:** `IFoodTruckRepository` define el contrato de acceso a datos. `FoodTruckEnMemoriaRepository` es la implementación en memoria. Los servicios solo dependen de la interfaz.

**Archivos:** `Repositories/IFoodTruckRepository.cs`, `Repositories/FoodTruckEnMemoriaRepository.cs`


## 7. Decisiones técnicas

### Persistencia en memoria

Se eligió persistencia en memoria deliberadamente, no por limitación técnica. Sino porque el foco del trabajo como tal son los patrones de diseño y la arquitectura, no la integración con una base de datos. Al estar el repositorio detrás de `IFoodTruckRepository`, reemplazarlo por una implementación con Entity Framework Core en un futuro solo requeriría agregar una clase nueva sin modificar los servicios ni los controllers respetando así OCP. 

### Un solo repositorio en vez de uno por entidad

En un principio pensamos en hacer uso de genericos con un `IRepository<T>` o con implementaciones separadas por entidad teniendo cada una su repositorio.Pero en la implementación final se optó por un único `IFoodTruckRepository` que agrupa toda la lógica de acceso a datos. Todo esto ya que las entidades están fuertemente relacionadas entre sí (al crear un pedido se necesita el producto y el puesto al mismo tiempo), y tener un único repositorio evita coordinar múltiples repositorios para operaciones que lógicamente son atómicas.


### Pago como un objeto dentro de pedido

El pago no es una entidad separada: es un objeto de valor dentro de un `Pedido`. Y como un pago nunca se consulta de forma independiente, no tiene identidad propia, y modelarlo como entidad separada sentimos que no era lo correcto.

### Notificación embebida en el pedido

La notificación queda guardada como `string? UltimaNotificacion` en el pedido, accesible al consultar el detalle del mismo. Se decidió no crear un endpoint separado para notificaciones porque el cliente ya consulta el estado de su pedido y la notificación podría viaja en la misma respuesta anterior.

### `SimularRechazoPago` en el request

Este campo permite al equipo de frontend probar el flujo de pago rechazado sin modificar el código. Es la forma más pragmática de testear casos de error en un sistema con pagos mockeados.

---

## 8. Cambios respecto al modelo de UT3

### Enum EstadoPedido

| UT3 | UT5/6 | Razón del cambio |
|---|---|---|
| `enCola` | `Pendiente` | Nombre más claro |
| `preparandose` | `EnPreparacion` | Consistencia en español |
| `listo` | `ListoParaRetirar` | Más descriptivo para el cliente |
| `cancelado` | `Cancelado` | Sin cambio |
| — | `Entregado` | Estado final necesario para cerrar el flujo |

### Actor de "Registrar Pedido": Cajero → Cliente

En la UT3, el actor de "Registrar Pedido" era el Cajero. Pero en el prototipo de la UT4 hicimos que el cliente arma y confirma su propio pedido como si fuera un autoservicio. Por todo esto el cajero solo interviene en la confirmación del cobro en efectivo y en el registro de la entrega como tal.

### Cajero

Se eliminó `RegistrarPedido()` de la clase Cajero en el modelado y se agregó la acción de confirmar cobro como `POST /api/pedidos/{id}/pago/confirmar`.

### Repositorio único en vez de uno por entidad

El diagrama de UT3 mostraba repositorios separados (`FoodtruckRepository`, `PedidoRepository`, etc.). En la implementación se unificó en `IFoodTruckRepository` por las razones explicadas en la sección anterior.

### Eliminación de Empleado/Cajero/Cocinero como entidades

El prototipo no tiene pantallas de login ni gestión de empleados. Los actores Cocinero y Cajero son roles conceptuales: el frontend llama a los endpoints según la pantalla que está usando. Crear entidades `Empleado`, `Cajero` y `Cocinero` en el backend no aportaría nada funcional dado que no hay autenticación  por ahora.

---

## 9. Qué no se implementó y por qué

| Qué | Por qué no |
|---|---|
| Autenticación / login | Fuera del alcance de UT5/6. Los actores (Cliente, Cocinero, Cajero) son roles conceptuales como tal |
| Base de datos real | El foco son los patrones y SOLID, no la implementacion con una base de datos. |
| Integración real con MercadoPago | Es un mock intencional. La arquitectura Strategy permite reemplazarlo sin cambiar nada más. |
| Push notifications reales | La notificación queda guardada en el pedido y el cliente la lee al consultar el estado. |
| Endpoint `GET /api/clientes/{id}/notificaciones` | La notificación viaja en el detalle del pedido; un endpoint separado sería redundante, pero se podría implementar. |
