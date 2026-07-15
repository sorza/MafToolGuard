using System.ComponentModel;

namespace MafToolGuard
{
    public static class WarehouseTools
    {
        [Description("Consulta o nível de estoque de um produto pelo seu código SKU")]
        public static string GetStockLevel(string sku)
            => $"Produto {sku}: 142 unidades em estoque no depósito central.";

        [Description("Reserva unidades de um produto para um pedido")]
        public static string ReserveStock(string sku, int quantity)
            => $"Reserva de {quantity} unidades do produto {sku} confirmada.";
    }
}
