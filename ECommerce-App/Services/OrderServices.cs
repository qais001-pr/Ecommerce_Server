using ECommerce_App.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ECommerce_App.Services
{
    public interface IOrders
    {
        Task CreateOrder(Order o);
    }
    public class OrderServices:IOrders
    {
        public IMongoCollection<Order> orderCollection;
        public OrderServices(IMongoDatabase database)
        {
            orderCollection = database.GetCollection<Order>("Order");
        }
        public async Task CreateOrder(Order createOrder)
        {
            await orderCollection.InsertOneAsync(createOrder);
        }
    }
}
