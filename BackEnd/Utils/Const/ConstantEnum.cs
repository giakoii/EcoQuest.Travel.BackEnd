namespace BackEnd.Utils.Const;

public static class ConstantEnum
{
    public enum UserRole
    {
        Customer,
        SaleEmployee,
        PlannedCustomer,	
        Owner,
    }
    
    /// <summary>
    /// Type of product. BlindBox or Accessory
    /// </summary>
    public enum ProductType
    {
        Product = 1,
        BlindBox = 2,
    }
    
    /// <summary>
    /// Sort type
    /// </summary>
    public enum Sort
    {
        MostPopular = 1,
        DecreasingPrice = 2,
        IncreasingPrice = 3,	
        Newest = 4,
        Oldest = 5,
    }

    /// <summary>
    /// Status of Exchange
    /// </summary>
    public enum ExchangeStatus
    {
        PendingExchange = 1,
        isChanging = 2,
        Success = 3,
        Fail = 4
    }


    /// <summary>
    /// Status of Queue
    /// </summary>
    public enum QueueStatus
    {
        Waiting = 1,
        isChanging = 2,
        Fail = 3
    }

    public enum PostingStatus
    {
        PendingExchange = 1,
        Fail = 2,
        PendingRecheck = 3,
    }
    
    /// <summary>
    /// Status of Order Plan
    /// </summary>
    public enum OrderPlans
    {
        Success, 
        Fail, 
        Pending, 
        Cancel, 
        Refunded
    }
    
    /// <summary>
    /// Status of Order
    /// </summary>
    public enum OrderStatus
    {
        Processing = 1,
        Success = 2,
        Failed = 3,
    }
    
    /// <summary>
    /// Payment method
    /// </summary>
    public enum PaymentMethod
    {
        Online = 1,
        CashOnDelivery = 2
    }
    
    /// <summary>
    /// Status of Refund Order
    /// </summary>
    public enum RefundOrderStatus
    {
        PendingApproval = 1,
        Approved = 2,
        Rejected = 3
    }

    /// <summary>
    /// Re-check Satus
    /// </summary>
    public enum RecheckStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
    
    public enum Platform
    {
        Web = 1,
        Mobile = 2
    }
    
    public enum PaymentStatus
    {        
        Success = 0,
    }
}