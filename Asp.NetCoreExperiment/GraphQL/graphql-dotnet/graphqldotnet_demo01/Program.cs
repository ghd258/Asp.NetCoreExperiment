using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "graphqldotnet_demo01", Version = "v1" });
});


var app = builder.Build();


if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "graphqldotnet_demo01 v1"));
}
//���ʵ�ַ��http://localhost:5000/ui/graphiql  
app.UseGraphQLGraphiQL();

//���ʵ�ַ��http://localhost:5000/ui/altair
app.UseGraphQLAltair();

app.UseAuthorization();

app.MapControllers();

app.Run();
