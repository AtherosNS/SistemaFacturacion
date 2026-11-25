using Comercial360.Application.Sales;
using Microsoft.EntityFrameworkCore;

namespace Comercial360.Adapters.Persistence;

public sealed class EfSaleQuery(Comercial360DbContext dbContext) : ISaleQuery
{
    public async Task<SalePageData> ListAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        // Error 2 fix: separar baseQuery (para count) del query ordenado/paginado.
        // El COUNT de SQL ignora ORDER BY, así que ordenar antes del count es costoso e innecesario.
        var baseQuery = dbContext.Sales.AsNoTracking();
        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var items = await baseQuery
            .OrderByDescending(sale => sale.CreatedAtUtc)
            .ThenByDescending(sale => sale.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(sale => new SaleSummaryData(
                sale.Id,
                sale.CustomerId,
                sale.CreatedAtUtc,
                sale.Subtotal,
                sale.TaxTotal,
                sale.Total,
                sale.Status))
            .ToArrayAsync(cancellationToken);

        return new SalePageData(items, page, pageSize, totalCount);
    }

    public async Task<SaleDetailData?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var sale = await dbContext.Sales
            .AsNoTracking()
            .Include(item => item.Lines)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        return sale is null
            ? null
            : new SaleDetailData(
                sale.Id,
                sale.CustomerId,
                sale.CreatedAtUtc,
                sale.Subtotal,
                sale.TaxTotal,
                sale.Total,
                sale.Status,
                sale.Lines
                    .OrderBy(line => line.Id)
                    .Select(line => new SaleLineData(
                        line.ProductId,
                        line.DescriptionSnapshot,
                        line.Quantity,
                        line.UnitNetPrice,
                        line.NetAmount,
                        line.TaxAmount))
                    .ToArray());
    }
}
