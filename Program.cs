using Microsoft.AspNetCore.Mvc;
using ToDoApi;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy",
        policy => policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
 builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("CorsPolicy");

/*if (app.Environment.IsDevelopment())
{*/

    app.UseSwagger();
    app.UseSwaggerUI();
/*}*/
app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});





app.MapGet("/",()=>"hello to miri's site :) ")
app.MapGet("/toDo",   (ToDoDbContext db) =>
         Results.Ok( db.Items)
);

app.MapPost("/toDo", async ([FromBody]Item item,ToDoDbContext tddc) =>{
    item.IsComplete=false;
    
    var t=await tddc.Items.AddAsync(item);
     await tddc.SaveChangesAsync();
     return Results.Created($"/toDo/{item.Id}", item);
    });
// app.MapPut("/toDo/{id}", async (int id,[FromBody]Item item,ToDoDbContext tddc) => {
//     var newItem = await tddc.Items.FindAsync(id);
//     if (item is null)
//         return Results.NotFound();
//     tddc.Items.Remove(newItem);
//     var t=await tddc.Items.AddAsync(item);
//     await tddc.SaveChangesAsync();
//      return Results.Created($"/toDo/{item.Id}", item);
// });
app.MapPut("/toDo/{id}", async (int id, ToDoDbContext tddb) =>
{
    var newItem = await tddb.Items.FindAsync(id);
    if (newItem is null)
        return Results.NotFound();
    newItem.IsComplete = !newItem.IsComplete;
    await tddb.SaveChangesAsync();
    return Results.Created($"/toDo/{newItem.Id}", newItem);
});

app.MapDelete("/toDo/{id}", async (int id,ToDoDbContext tddc) => {

    var item = await tddc.Items.FindAsync(id);
    if (item is null)
        return Results.NotFound();
    tddc.Items.Remove(item);
    await tddc.SaveChangesAsync();
    return Results.Ok(item);
});
// app.MapMethods("/options-or-head", new[] { "OPTIONS", "HEAD" },() => "This is an options or head request ");
app.Run();