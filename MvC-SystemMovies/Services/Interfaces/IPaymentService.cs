namespace MvC_SystemMovies.Services.Interfaces
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string PaymentMethod { get; set; } = "Card"; 
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public string? ExpiryMonthYear { get; set; }
        public string? Cvv { get; set; }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
    }

    
    public interface IPaymentService
    {
        Task<PaymentResult> ChargeAsync(PaymentRequest request);
    }
}
