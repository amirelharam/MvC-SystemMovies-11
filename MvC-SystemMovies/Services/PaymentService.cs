using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Services
{
  
    public class PaymentService : IPaymentService
    {
        public Task<PaymentResult> ChargeAsync(PaymentRequest request)
        {
            if (request.Amount <= 0)
            {
                return Task.FromResult(new PaymentResult { Success = false, ErrorMessage = "Invalid amount." });
            }

            
            if (request.PaymentMethod == "Card" &&
                (string.IsNullOrWhiteSpace(request.CardNumber) || request.CardNumber.EndsWith("0000")))
            {
                return Task.FromResult(new PaymentResult { Success = false, ErrorMessage = "Card declined (simulated)." });
            }

            var transactionId = "SIM-" + Guid.NewGuid().ToString("N")[..12].ToUpper();
            return Task.FromResult(new PaymentResult { Success = true, TransactionId = transactionId });
        }
    }
}
