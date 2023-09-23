using Chr.Avro.Confluent;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using KafkaProducer.Schemas;

CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => {
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    AllowAutoCreateTopics = true,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    ClientId = "test-consumer",
    GroupId = "test-consumer-group3"
};

var schemaConfig = new SchemaRegistryConfig { Url = "http://localhost:8081" };
var schemaClient = new CachedSchemaRegistryClient(schemaConfig);

var consumer = new ConsumerBuilder<Ignore, OrderEvent>(consumerConfig)
    .SetAvroKeyDeserializer(schemaClient)
    .SetAvroValueDeserializer(schemaClient)
    .SetErrorHandler((_, err) =>
    {
        Console.WriteLine(err);
    })
    .Build();

consumer.Subscribe("test_person");

try
{
    while (true)
    {
        var cr = consumer.Consume(cts.Token);
        Console.WriteLine($"Offset {cr.Offset.Value}");
        Console.WriteLine($"Consumed event from topic {cr.Topic} with key {cr.Message.Key,-10} and value {cr.Message.Value.Prodcut}");
    }
}
catch (OperationCanceledException)
{
    // Ctrl-C was pressed.
}
finally
{
    consumer.Close();
}