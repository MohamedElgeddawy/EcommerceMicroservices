using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;


namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                //check if product already exists
                var getProduct = await GetByAsync(_ => _.Name!.Equals( entity.Name));
                if (getProduct != null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} already added");
                
                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if(currentEntity is not null && currentEntity.Id >0)
                    return new Response(true, $"{entity.Name} created successfully");
                else
                    return new Response(false, $"{entity.Name}  creation failed");
            }
            catch (Exception ex)
            {
                // Log the original exception message
                LogException.LogExceptions(ex);

                //display scary-free message to the user
                return new Response(false, "Product creation failed");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if(product is null)
                    return new Response(false, $"{entity.Name} not found");

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} deleted successfully");
            }
            catch (Exception ex)
            {
                // Log the original exception message
                LogException.LogExceptions(ex);

                //display scary-free message to the user
                return new Response(false, "Product deleting failed");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null? product : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception message
                LogException.LogExceptions(ex);

                //display scary-free message to the user
                throw new Exception("Product retrieving failed");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception message
                LogException.LogExceptions(ex);

                //display scary-free message to the user
                throw new InvalidOperationException("Products retrieving failed");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.AsNoTracking().Where(predicate).FirstOrDefaultAsync();
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception message
                LogException.LogExceptions(ex);
                //display scary-free message to the user
                throw new InvalidOperationException("Product retrieving failed");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} not found");

                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                context.SaveChanges();
                return new Response(true, $"{entity.Name} updated successfully");
            }
            catch (Exception ex)
            {
                // Log the original exception message
                LogException.LogExceptions(ex);
                //display scary-free message to the user
                return new Response(false, "Product updating failed");
            }
        }
    }
}
