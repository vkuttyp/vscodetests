
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddCors();
var app = builder.Build();
app.UseCors(opt=>
{
    opt.WithOrigins("http://localhost:38383","http://localhsot:833");
});
Console.WriteLine("hi");
var orders=new List<apitest.Order>();
var o=new apitest.Order{id=1,Customer="kutty", Date=new DateOnly(2000,1,1) };
o.OrderDetails.Add(new apitest.OrderDetail{ItemName="Item 1", Quantity=23});
orders.Add(o);

o=new apitest.Order{id=1,Customer="kutty", Date=new DateOnly(2000,1,1), Subscription="silver" };
o.OrderDetails.Add(new apitest.OrderDetail{ItemName="Item 2", Quantity=300});
orders.Add(o);
app.MapGet("/", () => orders)
.RequireAuthorization(policy=> {
    policy.RequireRole("admin");
});
app.MapGet("/kutty", (ClaimsPrincipal user)=>
{
    ArgumentNullException.ThrowIfNull(user.Identity?.Name);
    var name=user.Identity.Name;
    var data=orders.Where(a=>a.Customer==name);
    if(data==null) return Results.Empty;
    return Results.Ok(data);
}).RequireAuthorization(policy=> {
    policy.RequireRole("customer");
});

app.MapGet("/subs", (ClaimsPrincipal user) =>
{
    ArgumentNullException.ThrowIfNull(user.Identity?.Name);
    var name=user.Identity.Name;
    var data=orders.Where(a=>a.Customer==name);
    if(data==null) return Results.Empty;
    var hasClaim=user.HasClaim(c=> c.Type=="subscription");
    if(hasClaim){
    var subs=user.FindFirstValue("subscription") ?? throw new Exception("No claim found");
    var cdata=data.Where(a=>a.Subscription=="gold");
    return Results.Ok(cdata);
    }
    return Results.BadRequest();
}).RequireAuthorization(policy=> {
    policy.RequireRole("customer");
});

app.Run();
