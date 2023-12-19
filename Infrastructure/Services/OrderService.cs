using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Entities;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {     
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IBasketRepository basketRepo, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            //get basket from repo
            var basket = await _basketRepo.GetBasketAsync(basketId);

            //get products from product repo
            var items = new List<OrderItem>();
            foreach(var item in basket.Items){
                var productItem = await _unitOfWork.Repository<Product>().GetIdByAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }
            //get deliverymethod
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetIdByAsync(deliveryMethodId);

            //get subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            //create order
            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, items, subtotal);        
            _unitOfWork.Repository<Order>().Add(order);

            //TODO:save to db
            var result = await _unitOfWork.Complete();

            if (result <= 0) return null;
            
            //return order
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);

            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);

            return await _unitOfWork.Repository<Order>().ListAsync(spec);;
        }
    }
}