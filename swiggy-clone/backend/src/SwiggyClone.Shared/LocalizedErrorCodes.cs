namespace SwiggyClone.Shared;

/// <summary>
/// Centralised error code constants used by handlers and the localization filter.
/// Each constant maps to a key in the ErrorMessages.resx resource files.
/// </summary>
public static class LocalizedErrorCodes
{
    // ─────────────────────── Auth ─────────────────────────────────
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string SessionExpired = "SESSION_EXPIRED";
    public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
    public const string AccountLocked = "ACCOUNT_LOCKED";
    public const string InvalidOtp = "INVALID_OTP";
    public const string OtpExpired = "OTP_EXPIRED";

    // ─────────────────────── Validation ───────────────────────────
    public const string ValidationError = "VALIDATION_ERROR";

    // ─────────────────────── Orders ───────────────────────────────
    public const string OrderNotFound = "ORDER_NOT_FOUND";
    public const string OrderAlreadyCancelled = "ORDER_ALREADY_CANCELLED";
    public const string CannotCancelOrder = "CANNOT_CANCEL_ORDER";
    public const string OrderAlreadyDelivered = "ORDER_ALREADY_DELIVERED";

    // ─────────────────────── Restaurant ───────────────────────────
    public const string RestaurantNotFound = "RESTAURANT_NOT_FOUND";
    public const string RestaurantClosed = "RESTAURANT_CLOSED";
    public const string RestaurantAlreadyExists = "RESTAURANT_ALREADY_EXISTS";

    // ─────────────────────── Menu ─────────────────────────────────
    public const string MenuItemNotFound = "MENU_ITEM_NOT_FOUND";
    public const string CategoryNotFound = "CATEGORY_NOT_FOUND";

    // ─────────────────────── Cart ─────────────────────────────────
    public const string CartEmpty = "CART_EMPTY";
    public const string ItemNotAvailable = "ITEM_NOT_AVAILABLE";
    public const string DifferentRestaurant = "DIFFERENT_RESTAURANT";

    // ─────────────────────── Payment ──────────────────────────────
    public const string PaymentFailed = "PAYMENT_FAILED";
    public const string InsufficientBalance = "INSUFFICIENT_BALANCE";
    public const string PaymentNotFound = "PAYMENT_NOT_FOUND";

    // ─────────────────────── Delivery ─────────────────────────────
    public const string DeliveryPartnerNotFound = "DELIVERY_PARTNER_NOT_FOUND";
    public const string DeliveryNotAssigned = "DELIVERY_NOT_ASSIGNED";

    // ─────────────────────── Wallet ───────────────────────────────
    public const string WalletNotFound = "WALLET_NOT_FOUND";
    public const string InvalidAmount = "INVALID_AMOUNT";

    // ─────────────────────── Subscription ─────────────────────────
    public const string PlanNotFound = "PLAN_NOT_FOUND";
    public const string AlreadySubscribed = "ALREADY_SUBSCRIBED";

    // ─────────────────────── Referral ─────────────────────────────
    public const string InvalidReferralCode = "INVALID_REFERRAL_CODE";
    public const string SelfReferral = "SELF_REFERRAL";
    public const string AlreadyReferred = "ALREADY_REFERRED";

    // ─────────────────────── Chat Support ─────────────────────────
    public const string TicketNotFound = "TICKET_NOT_FOUND";
    public const string TicketClosed = "TICKET_CLOSED";

    // ─────────────────────── Dine-In ──────────────────────────────
    public const string SessionNotFound = "SESSION_NOT_FOUND";
    public const string SessionAlreadyEnded = "SESSION_ALREADY_ENDED";
    public const string TableNotAvailable = "TABLE_NOT_AVAILABLE";

    // ─────────────────────── Address ──────────────────────────────
    public const string AddressNotFound = "ADDRESS_NOT_FOUND";

    // ─────────────────────── Reviews ──────────────────────────────
    public const string ReviewAlreadySubmitted = "REVIEW_ALREADY_SUBMITTED";

    // ─────────────────────── Coupon ───────────────────────────────
    public const string CouponNotFound = "COUPON_NOT_FOUND";
    public const string CouponExpired = "COUPON_EXPIRED";
    public const string CouponAlreadyUsed = "COUPON_ALREADY_USED";

    // ─────────────────────── General ──────────────────────────────
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string Forbidden = "FORBIDDEN";
    public const string InternalError = "INTERNAL_ERROR";

    // ─────────────────────── Exception Titles ─────────────────────
    public const string TitleValidationError = "TITLE_VALIDATION_ERROR";
    public const string TitleNotFound = "TITLE_NOT_FOUND";
    public const string TitleConflict = "TITLE_CONFLICT";
    public const string TitleForbidden = "TITLE_FORBIDDEN";
    public const string TitleDomainError = "TITLE_DOMAIN_ERROR";
    public const string TitleInternalError = "TITLE_INTERNAL_ERROR";
}
