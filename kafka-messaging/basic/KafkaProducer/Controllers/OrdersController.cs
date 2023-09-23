using Chr.Avro.Abstract;
using Chr.Avro.Representation;
using Confluent.Kafka;
using KafkaProducer.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace KafkaProducer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        readonly IProducer<Ignore, OrderEvent> _producer; 
        public OrdersController(IProducer<Ignore, OrderEvent> producer)
        {
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] OrderEvent orderEvent)
        {
            await _producer.ProduceAsync("test_person", new Message<Ignore, OrderEvent>()
            {
                Value = orderEvent,
                Timestamp = new Timestamp(DateTime.UtcNow)
            });

            return Ok();
        }

        [HttpGet]
        public IActionResult InititalizeSchema()
        {
            var builder = new SchemaBuilder();
            var schema = builder.BuildSchema<OrderEvent>(); // a RecordSchema instance

            // do modifications here

            var writer = new JsonSchemaWriter();
            return Ok(writer.Write(schema));
        }
    }
}
