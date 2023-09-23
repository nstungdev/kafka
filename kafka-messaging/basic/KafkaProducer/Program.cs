using Chr.Avro.Confluent;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using KafkaProducer.Schemas;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped(s =>
{
    var schemaConfig = new SchemaRegistryConfig { Url = "http://localhost:8081" };
    var schemaClient = new CachedSchemaRegistryClient(schemaConfig);
    var config = new ProducerConfig
    {
        BootstrapServers = "localhost:9092",
        ClientId = "TestClient",
        AllowAutoCreateTopics = true,
    };
    return new ProducerBuilder<Ignore, OrderEvent>(config)
        .SetAvroKeySerializer(schemaClient, AutomaticRegistrationBehavior.Always)
        .SetAvroValueSerializer(schemaClient, AutomaticRegistrationBehavior.Always)
        .SetErrorHandler((_, err) =>
        {
            Console.WriteLine(err);
        })
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
