using BuildingBlocks.Pagination;

namespace Ordering.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery(PaginatedRequest PaginatedRequest) : IQuery<GetOrdersResult>;
public record GetOrdersResult(PaginatedResult<OrderDto> Orders);
