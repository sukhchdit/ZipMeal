/// Application-wide string constants.
///
/// Keeping user-visible text in a single file simplifies future
/// internationalisation (i18n) efforts and avoids scattered literals.
abstract final class AppStrings {
  // ─────────────────────── General ──────────────────────────────

  static const String appName = 'Swiggy Clone';
  static const String appTagline = 'Order food & dine-in from the best restaurants';

  // ─────────────────────── Auth ─────────────────────────────────

  static const String login = 'Login';
  static const String signUp = 'Sign Up';
  static const String logout = 'Logout';
  static const String email = 'Email';
  static const String password = 'Password';
  static const String confirmPassword = 'Confirm Password';
  static const String phoneNumber = 'Phone Number';
  static const String enterOtp = 'Enter OTP';
  static const String verifyOtp = 'Verify OTP';
  static const String resendOtp = 'Resend OTP';
  static const String forgotPassword = 'Forgot Password?';
  static const String orContinueWith = 'or continue with';
  static const String agreeToTerms = 'By continuing, you agree to our';
  static const String termsOfService = 'Terms of Service';
  static const String privacyPolicy = 'Privacy Policy';

  // ─────────────────────── Home ─────────────────────────────────

  static const String home = 'Home';
  static const String searchForRestaurants = 'Search for restaurants or dishes';
  static const String topPicks = 'Top Picks For You';
  static const String popularCuisines = 'Popular Cuisines';
  static const String nearbyRestaurants = 'Nearby Restaurants';
  static const String allRestaurants = 'All Restaurants';
  static const String seeAll = 'See All';

  // ─────────────────────── Restaurant ───────────────────────────

  static const String menu = 'Menu';
  static const String reviews = 'Reviews';
  static const String deliveryTime = 'Delivery Time';
  static const String distance = 'Distance';
  static const String rating = 'Rating';
  static const String costForTwo = 'Cost for Two';
  static const String addToCart = 'Add to Cart';
  static const String customize = 'Customize';
  static const String veg = 'Veg';
  static const String nonVeg = 'Non-Veg';
  static const String bestSeller = 'Bestseller';

  // ─────────────────────── Cart & Checkout ──────────────────────

  static const String cart = 'Cart';
  static const String checkout = 'Checkout';
  static const String emptyCart = 'Your cart is empty';
  static const String addItems = 'Add items to get started';
  static const String browseRestaurants = 'Browse Restaurants';
  static const String itemTotal = 'Item Total';
  static const String deliveryFee = 'Delivery Fee';
  static const String taxes = 'Taxes & Charges';
  static const String grandTotal = 'Grand Total';
  static const String applyCoupon = 'Apply Coupon';
  static const String placeOrder = 'Place Order';
  static const String deliveryAddress = 'Delivery Address';
  static const String changeAddress = 'Change';
  static const String paymentMethod = 'Payment Method';

  // ─────────────────────── Orders ───────────────────────────────

  static const String myOrders = 'My Orders';
  static const String orderDetails = 'Order Details';
  static const String trackOrder = 'Track Order';
  static const String reorder = 'Reorder';
  static const String orderPlaced = 'Order Placed';
  static const String preparing = 'Preparing';
  static const String onTheWay = 'On the Way';
  static const String delivered = 'Delivered';
  static const String cancelled = 'Cancelled';
  static const String noOrdersYet = 'No orders yet';
  static const String noOrdersDescription = 'Looks like you haven\'t placed any orders.';

  // ─────────────────────── Dine-In ──────────────────────────────

  static const String dineIn = 'Dine-In';
  static const String scanQr = 'Scan QR Code';
  static const String tableNumber = 'Table Number';
  static const String activeSession = 'Active Session';
  static const String endSession = 'End Session';
  static const String callWaiter = 'Call Waiter';
  static const String splitBill = 'Split Bill';

  // ─────────────────────── Profile ──────────────────────────────

  static const String profile = 'Profile';
  static const String editProfile = 'Edit Profile';
  static const String savedAddresses = 'Saved Addresses';
  static const String payments = 'Payments';
  static const String favourites = 'Favourites';
  static const String settings = 'Settings';
  static const String helpAndSupport = 'Help & Support';
  static const String aboutUs = 'About Us';

  // ─────────────────────── Errors ───────────────────────────────

  static const String somethingWentWrong = 'Something went wrong';
  static const String noInternetConnection = 'No internet connection';
  static const String checkConnection =
      'Please check your internet connection and try again.';
  static const String retry = 'Retry';
  static const String sessionExpired = 'Session expired. Please login again.';
  static const String serverError =
      'We\'re having trouble connecting. Please try again later.';
  static const String requestTimeout = 'Request timed out. Please try again.';
  static const String unknownError = 'An unexpected error occurred.';

  // ─────────────────────── Location ─────────────────────────────

  static const String detectLocation = 'Detect my location';
  static const String enterManually = 'Enter location manually';
  static const String locationPermissionDenied =
      'Location permission is required to show nearby restaurants.';
  static const String enableLocation = 'Enable Location';

  // ─────────────────────── Misc ─────────────────────────────────

  static const String search = 'Search';
  static const String cancel = 'Cancel';
  static const String confirm = 'Confirm';
  static const String save = 'Save';
  static const String delete = 'Delete';
  static const String done = 'Done';
  static const String next = 'Next';
  static const String back = 'Back';
  static const String skip = 'Skip';
  static const String ok = 'OK';
  static const String yes = 'Yes';
  static const String no = 'No';
  static const String loading = 'Loading...';
  static const String noResults = 'No results found';
}
