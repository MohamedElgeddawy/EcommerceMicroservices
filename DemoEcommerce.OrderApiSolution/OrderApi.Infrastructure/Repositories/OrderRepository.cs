using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();
                return order.Id > 0 ? new Response(true, "Order Placed Successfully") : new Response(false, "Error Occured while placing order");
            }
            catch (Exception ex)
            {
                // Log Original Exception
                Log.Error(ex.Message);

                //Display Scare-free message to client


                return new Response(false, "Error Occured while placing order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order == null)
                {
                    return new Response(false, "Order not found");
                }
                context.Orders.Remove(entity);
                await context.SaveChangesAsync();
                return new Response(true, "Order Deleted Successfully");
            }
            catch (Exception ex)
            {
                // Log Original Exception
                Log.Error(ex.Message);
                //Display Scare-free message to client
                return new Response(false, "Error Occured while deleting order");

            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await context.Orders.FindAsync(id);
                return order is not null ? order : null;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                Log.Error(ex.Message);
                //Display Scare-free message to client
                throw new Exception("Error Occured while retrieving order");
            }
        }
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders = await context.Orders.AsNoTracking().ToListAsync();
                return orders is not null ? orders : null;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                Log.Error(ex.Message);
                //Display Scare-free message to client
                throw new Exception("Error Occured while retrieving orders");
            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).FirstOrDefaultAsync();
                return order is not null ? order : null;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                Log.Error(ex.Message);
                //Display Scare-free message to client
                throw new Exception("Error Occured while retrieving order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> prediction)
        {
            try
            {
                var orders = await context.Orders.Where(prediction).ToListAsync();
                return orders is not null ? orders : null;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                Log.Error(ex.Message);
                //Display Scare-free message to client
                throw new Exception("Error Occured while retrieving orders");
            }
        }
        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order == null)
                {
                    return new Response(false, "Order not found");
                }
                context.Entry(order).State = EntityState.Detached;
                context.Orders.Update(entity);
                context.SaveChanges();
                return new Response(true, "Order Updated Successfully");
            }
            catch (Exception ex)
            {
                // Log Original Exception
                Log.Error(ex.Message);
                //Display Scare-free message to client
                return new Response(false, "Error Occured while updating order");
            }
        }
    }
}
