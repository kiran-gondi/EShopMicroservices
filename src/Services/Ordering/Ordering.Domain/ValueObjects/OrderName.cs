namespace Ordering.Domain.ValueObjects;

public record OrderName
{
    private const int DefaultLength = 5;
    public string Value { get; }
    private OrderName(String value) => Value = value;

    public static OrderName Of(String value)
    {
      ArgumentNullException.ThrowIfNull(value);
      //ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, DefaultLength);

      return new OrderName(value);
    }
}
