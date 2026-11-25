namespace Comercial360.Domain.Entities;

public sealed class StockMovement
{
    private StockMovement() { }

    private StockMovement(Guid productId, Guid saleId, decimal quantity, DateTimeOffset createdAtUtc)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        SaleId = saleId;
        Quantity = quantity;
        Type = "Sale";
        CreatedAtUtc = createdAtUtc.ToUniversalTime();
    }

    public static StockMovement Create(Guid productId, Guid saleId, decimal quantity, DateTimeOffset createdAtUtc)
    {
        if (productId == Guid.Empty || saleId == Guid.Empty || quantity <= 0)
        {
            throw new ArgumentException("Los datos del movimiento de stock no son válidos.");
        }

        return new StockMovement(productId, saleId, quantity, createdAtUtc);
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid SaleId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
}
