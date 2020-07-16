# NT1-20-1C-12B-CarritoCompras
**Descripción general:**

Se trata de un carrito de compras donde un usuario puede agregar y quitar productos a su carrito viendo el subtotal de su compra y confirmar o cancelar la misma.

**Modelos mínimos :**

1. Producto
2. Carrito
3. Usuario
4. Compra
5. Categorías

Un **Producto** tiene

- una categoría asociada
- un nombre
- Un precio
- Un stock disponible
- Una lista de carritos en donde se encuentra
- Una lista de compras de dicho tipo de producto

Un **carrito** :

- Tiene una lista de Productos
- Pertenece a un Usuario
- Tiene un precio subtotal

El **Usuario** tiene:

- Nombre
- Fecha de alta
- Un carrito asociado
- Una lista de compras que haya realizado

La **Compra** tiene:

- Un usuario que la realizo
- Una fecha en que fue realizada
- Una lista de productos que se compraron
- Un precio final de la compra

Una **categoría** tiene:

- Una lista de productos
- Un nombre

**Se requiere que:**

1. Se puedan cargar y administrar
  - Usuarios
  - Productos
  - Si se utilizan cate
2. Los Usuarios puedan agregar y quitar productos de sus carritos.
  - Buscar productos filtrando por categoría y ordenar precio o nombre
  - Agregar un producto al carrito
  - Quitar un producto del carrito
  - Ver su carrito con la lista actual de productos.
  - Ver el subtotal del carrito cuando se agrega o quita un producto.
3. Los Usuarios pueden confirmar la compra del carrito
  - Antes de poder confirmar la compra de los elementos del carrito se debe validar el stock de los productos.
  - Al confirmar la compra del carrito el carrito actual del usuario se debe vaciar.
  - Se genera una compra con los datos que tenía el carrito al momento de confirmar.
  - Se altera el stock de los productos cuando se confirma
4. Los Usuarios pueden ver el historial de sus compras realizadas ordenadas por fecha decreciente, fecha creciente y valor total de la compra.
