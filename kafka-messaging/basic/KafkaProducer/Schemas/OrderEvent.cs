namespace KafkaProducer.Schemas
{
    public class OrderEvent
    {
        public int Id { get; set; }
        public string Prodcut { get; set; } = default!;
    }
}
