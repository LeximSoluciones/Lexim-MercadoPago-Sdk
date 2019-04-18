#if NETSTANDARD2_0

using System.Threading.Tasks;
using MercadoPago.Common;

namespace MercadoPago.Resources
{
    // Async versions of actions for all resources.
    // The reason for using this approach of multiple partial classes instead of putting this code
    // in each corresponding class file is that we want to reduce merge conflicts with the original SDK as much as possible.
    // This code in particular would cause a lot of confusion and conflicts when trying to merge changes from origin/master.
    // Also, since we're using conditional compilation to keep compatibility with .NET 4.0, this would cause a lot of
    // #ifs, which we're reducing to only one.

    public partial class Card
    {
        public static async Task<Card> FindByIdAsync(string customerId, string id, bool useCache = false, string accessToken = null) =>
            await GetAsync($"/v1/customers/{customerId}/cards/{id}", accessToken, useCache);

        public async Task<Card> SaveAsync() => await PostAsync($"/v1/customers/{CustomerId}/cards/");

        public async Task<Card> UpdateAsync() => await PutAsync($"/v1/customers/{CustomerId}/cards/{Id}");

        public async Task<Card> DeleteAsync() => await DeleteAsync($"/v1/customers/{CustomerId}/cards/{Id}");
    }

    public partial class Customer
    {
        /// <summary>
        /// Find a customer by ID.
        /// </summary>
        /// <param name="id">Customer ID.</param>
        /// <param name="useCache">Cache configuration.</param>
        /// <param name="accessToken">OAuth Access Token (specific to this request).</param>
        /// <returns>Searched customer.</returns>
        public static async Task<Customer> FindByIdAsync(string id, bool useCache = false, string accessToken = null) => 
            await GetAsync($"/v1/customers/{id}", accessToken, useCache);

        /// <summary>
        /// Save a new customer
        /// </summary>
        public async Task<Customer> SaveAsync() => await PostAsync("/v1/customers");

        /// <summary>
        /// Update editable properties
        /// </summary>
        public async Task<Customer> UpdateAsync() => await PutAsync($"/v1/customers/{Id}");

        /// <summary>
        /// Remove a customer
        /// </summary>
        public async Task<Customer> DeleteAsync() => await DeleteAsync($"/v1/customers/{Id}");
    }

    public partial class MerchantOrder
    {
        public static async Task<MerchantOrder> FindByIdAsync(string id, bool useCache = false, string accessToken = null) =>
            await GetAsync($"/merchant_orders/{id}", accessToken, useCache);

        public async Task<MerchantOrder> LoadAsync(string id, bool useCache = false, string accessToken = null) =>
            await GetAsync($"/merchant_orders/{id}", accessToken, useCache);

        public async Task<MerchantOrder> SaveAsync() => await PostAsync("/merchant_orders");

        public async Task<MerchantOrder> UpdateAsync() => await PutAsync($"/merchant_orders/{Id}");
    }

    public partial class Payment
    {
        /// <summary>
        /// Find a payment through an unique identifier with Local Cache Flag
        /// </summary>
        public static async Task<Payment> FindByIdAsync(long? id, bool useCache = false, string accessToken = null) =>
            await GetAsync($"/v1/payments/{id}", accessToken, useCache);

        /// <summary>
        /// Save a new payment
        /// </summary>
        public async Task<Payment> SaveAsync() => await PostAsync("/v1/payments");
        /// <summary>
        /// Update editable properties
        /// </summary>
        public async Task<Payment> UpdateAsync() => await PutAsync($"/v1/payments/{Id}");

        /// <summary>
        /// Payment refund
        /// </summary> 
        public async Task<Payment> RefundAsync()
        {
            var refund = new Refund
            {
                PaymentId = this.Id
            };
            await refund.SaveAsync();

            if (refund.Id.HasValue)
            {
                this.Status = PaymentStatus.refunded;
            }
            
            return this;
        }

        /// <summary>
        /// Partial payment refund
        /// </summary> 
        public async Task<Payment> RefundAsync(decimal amount)
        {
            var refund = new Refund
            {
                PaymentId = this.Id,
                Amount = amount
            };

            await refund.SaveAsync();

            if (refund.Id.HasValue)
            {
                this.Status = PaymentStatus.refunded;
            }
            return this;
        }
    }

    public partial class Plan
    {
        public static async Task<Plan> LoadAsync(string id, bool useCache = false, string accessToken = null) =>
            await GetAsync($"/v1/plans/{id}", accessToken, useCache);

        public async Task<Plan> SaveAsync() => await PostAsync("/v1/plans");

        public async Task<Plan> UpdateAsync() => await PutAsync($"/v1/plans/{Id}");
    }

    public partial class Preapproval
    {
        /// <summary>
        /// Find a preapproval trought an unique identifier with Local Cache Flag
        /// </summary>
        public static async Task<Preapproval> FindByIdAsync(string id, bool useCache = false, string accessToken = null) =>
            await GetAsync($"/preapproval/{id}", accessToken, useCache);

        /// <summary>
        /// Save a new preapproval
        /// </summary>
        public async Task<Preapproval> SaveAsync() => await PostAsync("/preapproval");

        /// <summary>
        ///  Update editable properties
        /// </summary>
        public async Task<Preapproval> UpdateAsync() => await PutAsync($"/preapproval/{Id}");
    }

    public partial class Preference
    {
        /// <summary>
        /// Find a preference through an unique identifier with Local Cache Flag
        /// </summary>
        public static async Task<Preference> FindByIdAsync(string id, bool useCache = false, string accessToken = null) =>
            await GetAsync($"/checkout/preferences/{id}", accessToken, useCache);

        /// <summary>
        /// Save a new preference
        /// </summary>
        public async Task<Preference> SaveAsync() => await PostAsync($"/checkout/preferences");

        /// <summary>
        ///  Update editable properties
        /// </summary>
        public async Task<Preference> UpdateAsync() => await PutAsync($"/checkout/preferences/{Id}");
    }

    public partial class Refund
    {
        public async Task<Refund> SaveAsync() => await PostAsync($"/v1/payments/{PaymentId}/refunds");
    }

    public partial class AuthorizedPayment
    {
        /// <summary>
        /// Find an authorized payment trought an unique identifier with Local Cache Flag
        /// </summary>
        public static async Task<AuthorizedPayment> FindByIdAsync(long? id, bool useCache = false, string accessToken = null) =>
            await GetAsync($"/authorized_payments/{id}", accessToken, useCache);

        /// <summary>
        /// Save an authorized payment
        /// </summary>
        public async Task<AuthorizedPayment> SaveAsync() => await PostAsync("/authorized_payments");

        /// <summary>
        ///  Update editable properties
        /// </summary>
        public async Task<AuthorizedPayment> UpdateAsync() => await PutAsync($"/authorized_payments/{Id}");
    }
}

#endif