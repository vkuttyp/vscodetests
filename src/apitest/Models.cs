namespace apitest
{

internal class Order {
    public int id { get; set; }
    public string? Customer { get; set; }
    public string? Subscription { get; set; }="gold";
    public DateOnly Date {get;set;}
    public List<OrderDetail> OrderDetails {get;set;}=new();
}
internal class OrderDetail {
    public int id { get; set; }
    public string? ItemName { get; set; }
    public decimal Quantity {get;set;}
}
}