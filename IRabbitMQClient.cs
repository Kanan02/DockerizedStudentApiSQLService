namespace MyMicroservice
{
    public interface IRabbitMQClient
    {
        public void Publish(string exchange, string routingKey, string payload);
        public void CloseConnection();
    }
}