import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:intl/intl.dart' as intl;

import 'app_localizations_en.dart';
import 'app_localizations_hi.dart';

// ignore_for_file: type=lint

/// Callers can lookup localized strings with an instance of AppLocalizations
/// returned by `AppLocalizations.of(context)`.
///
/// Applications need to include `AppLocalizations.delegate()` in their app's
/// `localizationDelegates` list, and the locales they support in the app's
/// `supportedLocales` list. For example:
///
/// ```dart
/// import 'l10n/app_localizations.dart';
///
/// return MaterialApp(
///   localizationsDelegates: AppLocalizations.localizationsDelegates,
///   supportedLocales: AppLocalizations.supportedLocales,
///   home: MyApplicationHome(),
/// );
/// ```
///
/// ## Update pubspec.yaml
///
/// Please make sure to update your pubspec.yaml to include the following
/// packages:
///
/// ```yaml
/// dependencies:
///   # Internationalization support.
///   flutter_localizations:
///     sdk: flutter
///   intl: any # Use the pinned version from flutter_localizations
///
///   # Rest of dependencies
/// ```
///
/// ## iOS Applications
///
/// iOS applications define key application metadata, including supported
/// locales, in an Info.plist file that is built into the application bundle.
/// To configure the locales supported by your app, you’ll need to edit this
/// file.
///
/// First, open your project’s ios/Runner.xcworkspace Xcode workspace file.
/// Then, in the Project Navigator, open the Info.plist file under the Runner
/// project’s Runner folder.
///
/// Next, select the Information Property List item, select Add Item from the
/// Editor menu, then select Localizations from the pop-up menu.
///
/// Select and expand the newly-created Localizations item then, for each
/// locale your application supports, add a new item and select the locale
/// you wish to add from the pop-up menu in the Value field. This list should
/// be consistent with the languages listed in the AppLocalizations.supportedLocales
/// property.
abstract class AppLocalizations {
  AppLocalizations(String locale)
      : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations? of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations);
  }

  static const LocalizationsDelegate<AppLocalizations> delegate =
      _AppLocalizationsDelegate();

  /// A list of this localizations delegate along with the default localizations
  /// delegates.
  ///
  /// Returns a list of localizations delegates containing this delegate along with
  /// GlobalMaterialLocalizations.delegate, GlobalCupertinoLocalizations.delegate,
  /// and GlobalWidgetsLocalizations.delegate.
  ///
  /// Additional delegates can be added by appending to this list in
  /// MaterialApp. This list does not have to be used at all if a custom list
  /// of delegates is preferred or required.
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates =
      <LocalizationsDelegate<dynamic>>[
    delegate,
    GlobalMaterialLocalizations.delegate,
    GlobalCupertinoLocalizations.delegate,
    GlobalWidgetsLocalizations.delegate,
  ];

  /// A list of this localizations delegate's supported locales.
  static const List<Locale> supportedLocales = <Locale>[
    Locale('en'),
    Locale('hi')
  ];

  /// Application name
  ///
  /// In en, this message translates to:
  /// **'ZipMeal'**
  String get appName;

  /// App tagline on splash / marketing screens
  ///
  /// In en, this message translates to:
  /// **'Order food & dine-in from the best restaurants'**
  String get appTagline;

  /// No description provided for @login.
  ///
  /// In en, this message translates to:
  /// **'Login'**
  String get login;

  /// No description provided for @signUp.
  ///
  /// In en, this message translates to:
  /// **'Sign Up'**
  String get signUp;

  /// No description provided for @logout.
  ///
  /// In en, this message translates to:
  /// **'Logout'**
  String get logout;

  /// No description provided for @email.
  ///
  /// In en, this message translates to:
  /// **'Email'**
  String get email;

  /// No description provided for @password.
  ///
  /// In en, this message translates to:
  /// **'Password'**
  String get password;

  /// No description provided for @confirmPassword.
  ///
  /// In en, this message translates to:
  /// **'Confirm Password'**
  String get confirmPassword;

  /// No description provided for @phoneNumber.
  ///
  /// In en, this message translates to:
  /// **'Phone Number'**
  String get phoneNumber;

  /// No description provided for @enterOtp.
  ///
  /// In en, this message translates to:
  /// **'Enter OTP'**
  String get enterOtp;

  /// No description provided for @verifyOtp.
  ///
  /// In en, this message translates to:
  /// **'Verify OTP'**
  String get verifyOtp;

  /// No description provided for @resendOtp.
  ///
  /// In en, this message translates to:
  /// **'Resend OTP'**
  String get resendOtp;

  /// No description provided for @forgotPassword.
  ///
  /// In en, this message translates to:
  /// **'Forgot Password?'**
  String get forgotPassword;

  /// No description provided for @orContinueWith.
  ///
  /// In en, this message translates to:
  /// **'or continue with'**
  String get orContinueWith;

  /// No description provided for @agreeToTerms.
  ///
  /// In en, this message translates to:
  /// **'By continuing, you agree to our'**
  String get agreeToTerms;

  /// No description provided for @termsOfService.
  ///
  /// In en, this message translates to:
  /// **'Terms of Service'**
  String get termsOfService;

  /// No description provided for @privacyPolicy.
  ///
  /// In en, this message translates to:
  /// **'Privacy Policy'**
  String get privacyPolicy;

  /// No description provided for @welcomeBack.
  ///
  /// In en, this message translates to:
  /// **'Welcome Back'**
  String get welcomeBack;

  /// No description provided for @signInToContinue.
  ///
  /// In en, this message translates to:
  /// **'Sign in to continue'**
  String get signInToContinue;

  /// No description provided for @sendOtp.
  ///
  /// In en, this message translates to:
  /// **'Send OTP'**
  String get sendOtp;

  /// No description provided for @signIn.
  ///
  /// In en, this message translates to:
  /// **'Sign In'**
  String get signIn;

  /// No description provided for @phone.
  ///
  /// In en, this message translates to:
  /// **'Phone'**
  String get phone;

  /// No description provided for @phoneHint.
  ///
  /// In en, this message translates to:
  /// **'+91 9876543210'**
  String get phoneHint;

  /// No description provided for @emailHint.
  ///
  /// In en, this message translates to:
  /// **'you@example.com'**
  String get emailHint;

  /// No description provided for @phoneRequired.
  ///
  /// In en, this message translates to:
  /// **'Phone number is required'**
  String get phoneRequired;

  /// No description provided for @emailRequired.
  ///
  /// In en, this message translates to:
  /// **'Email is required'**
  String get emailRequired;

  /// No description provided for @passwordRequired.
  ///
  /// In en, this message translates to:
  /// **'Password is required'**
  String get passwordRequired;

  /// No description provided for @dontHaveAccount.
  ///
  /// In en, this message translates to:
  /// **'Don\'t have an account? '**
  String get dontHaveAccount;

  /// No description provided for @home.
  ///
  /// In en, this message translates to:
  /// **'Home'**
  String get home;

  /// No description provided for @searchForRestaurants.
  ///
  /// In en, this message translates to:
  /// **'Search for restaurants or dishes'**
  String get searchForRestaurants;

  /// No description provided for @topPicks.
  ///
  /// In en, this message translates to:
  /// **'Top Picks For You'**
  String get topPicks;

  /// No description provided for @popularCuisines.
  ///
  /// In en, this message translates to:
  /// **'Popular Cuisines'**
  String get popularCuisines;

  /// No description provided for @nearbyRestaurants.
  ///
  /// In en, this message translates to:
  /// **'Nearby Restaurants'**
  String get nearbyRestaurants;

  /// No description provided for @allRestaurants.
  ///
  /// In en, this message translates to:
  /// **'All Restaurants'**
  String get allRestaurants;

  /// No description provided for @seeAll.
  ///
  /// In en, this message translates to:
  /// **'See All'**
  String get seeAll;

  /// No description provided for @deliverToHome.
  ///
  /// In en, this message translates to:
  /// **'Deliver to Home'**
  String get deliverToHome;

  /// No description provided for @whatsOnYourMind.
  ///
  /// In en, this message translates to:
  /// **'What\'s on your mind?'**
  String get whatsOnYourMind;

  /// No description provided for @specialOffer.
  ///
  /// In en, this message translates to:
  /// **'Special Offer'**
  String get specialOffer;

  /// No description provided for @menu.
  ///
  /// In en, this message translates to:
  /// **'Menu'**
  String get menu;

  /// No description provided for @reviews.
  ///
  /// In en, this message translates to:
  /// **'Reviews'**
  String get reviews;

  /// No description provided for @deliveryTime.
  ///
  /// In en, this message translates to:
  /// **'Delivery Time'**
  String get deliveryTime;

  /// No description provided for @distance.
  ///
  /// In en, this message translates to:
  /// **'Distance'**
  String get distance;

  /// No description provided for @rating.
  ///
  /// In en, this message translates to:
  /// **'Rating'**
  String get rating;

  /// No description provided for @costForTwo.
  ///
  /// In en, this message translates to:
  /// **'Cost for Two'**
  String get costForTwo;

  /// No description provided for @addToCart.
  ///
  /// In en, this message translates to:
  /// **'Add to Cart'**
  String get addToCart;

  /// No description provided for @customize.
  ///
  /// In en, this message translates to:
  /// **'Customize'**
  String get customize;

  /// No description provided for @veg.
  ///
  /// In en, this message translates to:
  /// **'Veg'**
  String get veg;

  /// No description provided for @nonVeg.
  ///
  /// In en, this message translates to:
  /// **'Non-Veg'**
  String get nonVeg;

  /// No description provided for @bestSeller.
  ///
  /// In en, this message translates to:
  /// **'Bestseller'**
  String get bestSeller;

  /// No description provided for @cart.
  ///
  /// In en, this message translates to:
  /// **'Cart'**
  String get cart;

  /// No description provided for @checkout.
  ///
  /// In en, this message translates to:
  /// **'Checkout'**
  String get checkout;

  /// No description provided for @emptyCart.
  ///
  /// In en, this message translates to:
  /// **'Your cart is empty'**
  String get emptyCart;

  /// No description provided for @addItems.
  ///
  /// In en, this message translates to:
  /// **'Add items to get started'**
  String get addItems;

  /// No description provided for @browseRestaurants.
  ///
  /// In en, this message translates to:
  /// **'Browse Restaurants'**
  String get browseRestaurants;

  /// No description provided for @itemTotal.
  ///
  /// In en, this message translates to:
  /// **'Item Total'**
  String get itemTotal;

  /// No description provided for @deliveryFee.
  ///
  /// In en, this message translates to:
  /// **'Delivery Fee'**
  String get deliveryFee;

  /// No description provided for @taxes.
  ///
  /// In en, this message translates to:
  /// **'Taxes & Charges'**
  String get taxes;

  /// No description provided for @grandTotal.
  ///
  /// In en, this message translates to:
  /// **'Grand Total'**
  String get grandTotal;

  /// No description provided for @applyCoupon.
  ///
  /// In en, this message translates to:
  /// **'Apply Coupon'**
  String get applyCoupon;

  /// No description provided for @placeOrder.
  ///
  /// In en, this message translates to:
  /// **'Place Order'**
  String get placeOrder;

  /// No description provided for @deliveryAddress.
  ///
  /// In en, this message translates to:
  /// **'Delivery Address'**
  String get deliveryAddress;

  /// No description provided for @changeAddress.
  ///
  /// In en, this message translates to:
  /// **'Change'**
  String get changeAddress;

  /// No description provided for @paymentMethod.
  ///
  /// In en, this message translates to:
  /// **'Payment Method'**
  String get paymentMethod;

  /// No description provided for @myOrders.
  ///
  /// In en, this message translates to:
  /// **'My Orders'**
  String get myOrders;

  /// No description provided for @orderDetails.
  ///
  /// In en, this message translates to:
  /// **'Order Details'**
  String get orderDetails;

  /// No description provided for @trackOrder.
  ///
  /// In en, this message translates to:
  /// **'Track Order'**
  String get trackOrder;

  /// No description provided for @reorder.
  ///
  /// In en, this message translates to:
  /// **'Reorder'**
  String get reorder;

  /// No description provided for @orderPlaced.
  ///
  /// In en, this message translates to:
  /// **'Order Placed'**
  String get orderPlaced;

  /// No description provided for @preparing.
  ///
  /// In en, this message translates to:
  /// **'Preparing'**
  String get preparing;

  /// No description provided for @onTheWay.
  ///
  /// In en, this message translates to:
  /// **'On the Way'**
  String get onTheWay;

  /// No description provided for @delivered.
  ///
  /// In en, this message translates to:
  /// **'Delivered'**
  String get delivered;

  /// No description provided for @cancelled.
  ///
  /// In en, this message translates to:
  /// **'Cancelled'**
  String get cancelled;

  /// No description provided for @noOrdersYet.
  ///
  /// In en, this message translates to:
  /// **'No orders yet'**
  String get noOrdersYet;

  /// No description provided for @noOrdersDescription.
  ///
  /// In en, this message translates to:
  /// **'Looks like you haven\'t placed any orders.'**
  String get noOrdersDescription;

  /// No description provided for @dineIn.
  ///
  /// In en, this message translates to:
  /// **'Dine-In'**
  String get dineIn;

  /// No description provided for @scanQr.
  ///
  /// In en, this message translates to:
  /// **'Scan QR Code'**
  String get scanQr;

  /// No description provided for @tableNumber.
  ///
  /// In en, this message translates to:
  /// **'Table Number'**
  String get tableNumber;

  /// No description provided for @activeSession.
  ///
  /// In en, this message translates to:
  /// **'Active Session'**
  String get activeSession;

  /// No description provided for @endSession.
  ///
  /// In en, this message translates to:
  /// **'End Session'**
  String get endSession;

  /// No description provided for @callWaiter.
  ///
  /// In en, this message translates to:
  /// **'Call Waiter'**
  String get callWaiter;

  /// No description provided for @splitBill.
  ///
  /// In en, this message translates to:
  /// **'Split Bill'**
  String get splitBill;

  /// No description provided for @profile.
  ///
  /// In en, this message translates to:
  /// **'Profile'**
  String get profile;

  /// No description provided for @editProfile.
  ///
  /// In en, this message translates to:
  /// **'Edit Profile'**
  String get editProfile;

  /// No description provided for @savedAddresses.
  ///
  /// In en, this message translates to:
  /// **'Saved Addresses'**
  String get savedAddresses;

  /// No description provided for @payments.
  ///
  /// In en, this message translates to:
  /// **'Payments'**
  String get payments;

  /// No description provided for @favourites.
  ///
  /// In en, this message translates to:
  /// **'Favourites'**
  String get favourites;

  /// No description provided for @settings.
  ///
  /// In en, this message translates to:
  /// **'Settings'**
  String get settings;

  /// No description provided for @helpAndSupport.
  ///
  /// In en, this message translates to:
  /// **'Help & Support'**
  String get helpAndSupport;

  /// No description provided for @aboutUs.
  ///
  /// In en, this message translates to:
  /// **'About Us'**
  String get aboutUs;

  /// No description provided for @somethingWentWrong.
  ///
  /// In en, this message translates to:
  /// **'Something went wrong'**
  String get somethingWentWrong;

  /// No description provided for @noInternetConnection.
  ///
  /// In en, this message translates to:
  /// **'No internet connection'**
  String get noInternetConnection;

  /// No description provided for @checkConnection.
  ///
  /// In en, this message translates to:
  /// **'Please check your internet connection and try again.'**
  String get checkConnection;

  /// No description provided for @retry.
  ///
  /// In en, this message translates to:
  /// **'Retry'**
  String get retry;

  /// No description provided for @sessionExpired.
  ///
  /// In en, this message translates to:
  /// **'Session expired. Please login again.'**
  String get sessionExpired;

  /// No description provided for @serverError.
  ///
  /// In en, this message translates to:
  /// **'We\'re having trouble connecting. Please try again later.'**
  String get serverError;

  /// No description provided for @requestTimeout.
  ///
  /// In en, this message translates to:
  /// **'Request timed out. Please try again.'**
  String get requestTimeout;

  /// No description provided for @unknownError.
  ///
  /// In en, this message translates to:
  /// **'An unexpected error occurred.'**
  String get unknownError;

  /// No description provided for @detectLocation.
  ///
  /// In en, this message translates to:
  /// **'Detect my location'**
  String get detectLocation;

  /// No description provided for @enterManually.
  ///
  /// In en, this message translates to:
  /// **'Enter location manually'**
  String get enterManually;

  /// No description provided for @locationPermissionDenied.
  ///
  /// In en, this message translates to:
  /// **'Location permission is required to show nearby restaurants.'**
  String get locationPermissionDenied;

  /// No description provided for @enableLocation.
  ///
  /// In en, this message translates to:
  /// **'Enable Location'**
  String get enableLocation;

  /// No description provided for @search.
  ///
  /// In en, this message translates to:
  /// **'Search'**
  String get search;

  /// No description provided for @cancel.
  ///
  /// In en, this message translates to:
  /// **'Cancel'**
  String get cancel;

  /// No description provided for @confirm.
  ///
  /// In en, this message translates to:
  /// **'Confirm'**
  String get confirm;

  /// No description provided for @save.
  ///
  /// In en, this message translates to:
  /// **'Save'**
  String get save;

  /// No description provided for @delete.
  ///
  /// In en, this message translates to:
  /// **'Delete'**
  String get delete;

  /// No description provided for @done.
  ///
  /// In en, this message translates to:
  /// **'Done'**
  String get done;

  /// No description provided for @next.
  ///
  /// In en, this message translates to:
  /// **'Next'**
  String get next;

  /// No description provided for @back.
  ///
  /// In en, this message translates to:
  /// **'Back'**
  String get back;

  /// No description provided for @skip.
  ///
  /// In en, this message translates to:
  /// **'Skip'**
  String get skip;

  /// No description provided for @ok.
  ///
  /// In en, this message translates to:
  /// **'OK'**
  String get ok;

  /// No description provided for @yes.
  ///
  /// In en, this message translates to:
  /// **'Yes'**
  String get yes;

  /// No description provided for @no.
  ///
  /// In en, this message translates to:
  /// **'No'**
  String get no;

  /// No description provided for @loading.
  ///
  /// In en, this message translates to:
  /// **'Loading...'**
  String get loading;

  /// No description provided for @noResults.
  ///
  /// In en, this message translates to:
  /// **'No results found'**
  String get noResults;

  /// No description provided for @account.
  ///
  /// In en, this message translates to:
  /// **'Account'**
  String get account;

  /// No description provided for @notifications.
  ///
  /// In en, this message translates to:
  /// **'Notifications'**
  String get notifications;

  /// No description provided for @wallet.
  ///
  /// In en, this message translates to:
  /// **'Wallet'**
  String get wallet;

  /// No description provided for @addMoney.
  ///
  /// In en, this message translates to:
  /// **'Add Money'**
  String get addMoney;

  /// No description provided for @balance.
  ///
  /// In en, this message translates to:
  /// **'Balance'**
  String get balance;

  /// No description provided for @transactionHistory.
  ///
  /// In en, this message translates to:
  /// **'Transaction History'**
  String get transactionHistory;

  /// No description provided for @subscription.
  ///
  /// In en, this message translates to:
  /// **'Subscription'**
  String get subscription;

  /// No description provided for @subscribePlans.
  ///
  /// In en, this message translates to:
  /// **'Subscribe to Plans'**
  String get subscribePlans;

  /// No description provided for @currentPlan.
  ///
  /// In en, this message translates to:
  /// **'Current Plan'**
  String get currentPlan;

  /// No description provided for @upgradePlan.
  ///
  /// In en, this message translates to:
  /// **'Upgrade Plan'**
  String get upgradePlan;

  /// No description provided for @referralProgram.
  ///
  /// In en, this message translates to:
  /// **'Referral Program'**
  String get referralProgram;

  /// No description provided for @referralCode.
  ///
  /// In en, this message translates to:
  /// **'Referral Code'**
  String get referralCode;

  /// No description provided for @inviteFriends.
  ///
  /// In en, this message translates to:
  /// **'Invite Friends'**
  String get inviteFriends;

  /// No description provided for @referralRewards.
  ///
  /// In en, this message translates to:
  /// **'Referral Rewards'**
  String get referralRewards;

  /// No description provided for @chatSupport.
  ///
  /// In en, this message translates to:
  /// **'Chat Support'**
  String get chatSupport;

  /// No description provided for @newTicket.
  ///
  /// In en, this message translates to:
  /// **'New Ticket'**
  String get newTicket;

  /// No description provided for @typeMessage.
  ///
  /// In en, this message translates to:
  /// **'Type a message...'**
  String get typeMessage;

  /// No description provided for @sendMessage.
  ///
  /// In en, this message translates to:
  /// **'Send'**
  String get sendMessage;

  /// No description provided for @activeSessions.
  ///
  /// In en, this message translates to:
  /// **'Active Sessions'**
  String get activeSessions;

  /// No description provided for @changePassword.
  ///
  /// In en, this message translates to:
  /// **'Change Password'**
  String get changePassword;

  /// No description provided for @addresses.
  ///
  /// In en, this message translates to:
  /// **'Addresses'**
  String get addresses;

  /// No description provided for @language.
  ///
  /// In en, this message translates to:
  /// **'Language'**
  String get language;

  /// No description provided for @selectLanguage.
  ///
  /// In en, this message translates to:
  /// **'Select Language'**
  String get selectLanguage;

  /// No description provided for @deleteAccount.
  ///
  /// In en, this message translates to:
  /// **'Delete Account'**
  String get deleteAccount;

  /// No description provided for @deleteAccountWarning.
  ///
  /// In en, this message translates to:
  /// **'This action cannot be undone. All your data will be permanently deleted. Are you sure?'**
  String get deleteAccountWarning;

  /// No description provided for @areYouSureLogout.
  ///
  /// In en, this message translates to:
  /// **'Are you sure you want to logout?'**
  String get areYouSureLogout;

  /// No description provided for @deliveryPartner.
  ///
  /// In en, this message translates to:
  /// **'Delivery Partner'**
  String get deliveryPartner;

  /// No description provided for @activeDelivery.
  ///
  /// In en, this message translates to:
  /// **'Active Delivery'**
  String get activeDelivery;

  /// No description provided for @deliveryDashboard.
  ///
  /// In en, this message translates to:
  /// **'Delivery Dashboard'**
  String get deliveryDashboard;

  /// No description provided for @assignedOrders.
  ///
  /// In en, this message translates to:
  /// **'Assigned Orders'**
  String get assignedOrders;

  /// No description provided for @tipDeliveryPartner.
  ///
  /// In en, this message translates to:
  /// **'Tip Delivery Partner'**
  String get tipDeliveryPartner;

  /// No description provided for @scheduledOrder.
  ///
  /// In en, this message translates to:
  /// **'Scheduled Order'**
  String get scheduledOrder;

  /// No description provided for @scheduleDelivery.
  ///
  /// In en, this message translates to:
  /// **'Schedule Delivery'**
  String get scheduleDelivery;

  /// No description provided for @selectDateTime.
  ///
  /// In en, this message translates to:
  /// **'Select Date & Time'**
  String get selectDateTime;

  /// No description provided for @favouriteDishes.
  ///
  /// In en, this message translates to:
  /// **'Favourite Dishes'**
  String get favouriteDishes;

  /// No description provided for @quickReorder.
  ///
  /// In en, this message translates to:
  /// **'Quick Reorder'**
  String get quickReorder;

  /// No description provided for @addToFavourites.
  ///
  /// In en, this message translates to:
  /// **'Add to Favourites'**
  String get addToFavourites;

  /// No description provided for @removeFromFavourites.
  ///
  /// In en, this message translates to:
  /// **'Remove from Favourites'**
  String get removeFromFavourites;

  /// No description provided for @activityFeed.
  ///
  /// In en, this message translates to:
  /// **'Activity Feed'**
  String get activityFeed;

  /// No description provided for @follow.
  ///
  /// In en, this message translates to:
  /// **'Follow'**
  String get follow;

  /// No description provided for @unfollow.
  ///
  /// In en, this message translates to:
  /// **'Unfollow'**
  String get unfollow;

  /// No description provided for @following.
  ///
  /// In en, this message translates to:
  /// **'Following'**
  String get following;

  /// No description provided for @followers.
  ///
  /// In en, this message translates to:
  /// **'Followers'**
  String get followers;

  /// No description provided for @noActivityYet.
  ///
  /// In en, this message translates to:
  /// **'No activity yet'**
  String get noActivityYet;

  /// No description provided for @followPeopleToSeeActivity.
  ///
  /// In en, this message translates to:
  /// **'Follow people to see their activity here'**
  String get followPeopleToSeeActivity;

  /// No description provided for @userProfile.
  ///
  /// In en, this message translates to:
  /// **'User Profile'**
  String get userProfile;

  /// No description provided for @shareRestaurant.
  ///
  /// In en, this message translates to:
  /// **'Share Restaurant'**
  String get shareRestaurant;

  /// No description provided for @shareReview.
  ///
  /// In en, this message translates to:
  /// **'Share Review'**
  String get shareReview;

  /// No description provided for @shareOrder.
  ///
  /// In en, this message translates to:
  /// **'Share Order'**
  String get shareOrder;

  /// No description provided for @shareVia.
  ///
  /// In en, this message translates to:
  /// **'Share via'**
  String get shareVia;

  /// No description provided for @copiedToClipboard.
  ///
  /// In en, this message translates to:
  /// **'Copied to clipboard'**
  String get copiedToClipboard;

  /// No description provided for @reviewedRestaurant.
  ///
  /// In en, this message translates to:
  /// **'reviewed a restaurant'**
  String get reviewedRestaurant;

  /// No description provided for @favouritedRestaurant.
  ///
  /// In en, this message translates to:
  /// **'favourited'**
  String get favouritedRestaurant;

  /// No description provided for @placedOrder.
  ///
  /// In en, this message translates to:
  /// **'placed an order'**
  String get placedOrder;

  /// No description provided for @startedFollowing.
  ///
  /// In en, this message translates to:
  /// **'started following'**
  String get startedFollowing;

  /// No description provided for @viewProfile.
  ///
  /// In en, this message translates to:
  /// **'View Profile'**
  String get viewProfile;

  /// No description provided for @cannotFollowSelf.
  ///
  /// In en, this message translates to:
  /// **'You cannot follow yourself'**
  String get cannotFollowSelf;

  /// No description provided for @alreadyFollowing.
  ///
  /// In en, this message translates to:
  /// **'Already following'**
  String get alreadyFollowing;

  /// No description provided for @socialFeatures.
  ///
  /// In en, this message translates to:
  /// **'Social'**
  String get socialFeatures;

  /// No description provided for @recentActivity.
  ///
  /// In en, this message translates to:
  /// **'Recent Activity'**
  String get recentActivity;

  /// No description provided for @followersCount.
  ///
  /// In en, this message translates to:
  /// **'Followers'**
  String get followersCount;

  /// No description provided for @followingCount.
  ///
  /// In en, this message translates to:
  /// **'Following'**
  String get followingCount;

  /// No description provided for @reviewsCount.
  ///
  /// In en, this message translates to:
  /// **'Reviews'**
  String get reviewsCount;

  /// No description provided for @promotions.
  ///
  /// In en, this message translates to:
  /// **'Promotions'**
  String get promotions;

  /// No description provided for @myPromotions.
  ///
  /// In en, this message translates to:
  /// **'My Promotions'**
  String get myPromotions;

  /// No description provided for @createPromotion.
  ///
  /// In en, this message translates to:
  /// **'Create Promotion'**
  String get createPromotion;

  /// No description provided for @editPromotion.
  ///
  /// In en, this message translates to:
  /// **'Edit Promotion'**
  String get editPromotion;

  /// No description provided for @flashDeal.
  ///
  /// In en, this message translates to:
  /// **'Flash Deal'**
  String get flashDeal;

  /// No description provided for @happyHour.
  ///
  /// In en, this message translates to:
  /// **'Happy Hour'**
  String get happyHour;

  /// No description provided for @comboOffer.
  ///
  /// In en, this message translates to:
  /// **'Combo Offer'**
  String get comboOffer;

  /// No description provided for @promotionTitle.
  ///
  /// In en, this message translates to:
  /// **'Promotion Title'**
  String get promotionTitle;

  /// No description provided for @promotionDescription.
  ///
  /// In en, this message translates to:
  /// **'Promotion Description'**
  String get promotionDescription;

  /// No description provided for @discountValue.
  ///
  /// In en, this message translates to:
  /// **'Discount Value'**
  String get discountValue;

  /// No description provided for @maxDiscount.
  ///
  /// In en, this message translates to:
  /// **'Max Discount'**
  String get maxDiscount;

  /// No description provided for @minOrderAmount.
  ///
  /// In en, this message translates to:
  /// **'Min Order Amount'**
  String get minOrderAmount;

  /// No description provided for @validFrom.
  ///
  /// In en, this message translates to:
  /// **'Valid From'**
  String get validFrom;

  /// No description provided for @validUntil.
  ///
  /// In en, this message translates to:
  /// **'Valid Until'**
  String get validUntil;

  /// No description provided for @recurringSchedule.
  ///
  /// In en, this message translates to:
  /// **'Recurring Schedule'**
  String get recurringSchedule;

  /// No description provided for @recurringDays.
  ///
  /// In en, this message translates to:
  /// **'Recurring Days'**
  String get recurringDays;

  /// No description provided for @comboPrice.
  ///
  /// In en, this message translates to:
  /// **'Combo Price'**
  String get comboPrice;

  /// No description provided for @selectMenuItems.
  ///
  /// In en, this message translates to:
  /// **'Select Menu Items'**
  String get selectMenuItems;

  /// No description provided for @promotionCreated.
  ///
  /// In en, this message translates to:
  /// **'Promotion created successfully'**
  String get promotionCreated;

  /// No description provided for @promotionUpdated.
  ///
  /// In en, this message translates to:
  /// **'Promotion updated successfully'**
  String get promotionUpdated;

  /// No description provided for @promotionDeleted.
  ///
  /// In en, this message translates to:
  /// **'Promotion deleted successfully'**
  String get promotionDeleted;

  /// No description provided for @promotionToggled.
  ///
  /// In en, this message translates to:
  /// **'Promotion status updated'**
  String get promotionToggled;

  /// No description provided for @noPromotionsYet.
  ///
  /// In en, this message translates to:
  /// **'No promotions yet'**
  String get noPromotionsYet;

  /// No description provided for @createYourFirstPromotion.
  ///
  /// In en, this message translates to:
  /// **'Create your first promotion!'**
  String get createYourFirstPromotion;

  /// No description provided for @flashDealEndsIn.
  ///
  /// In en, this message translates to:
  /// **'Ends in'**
  String get flashDealEndsIn;

  /// No description provided for @happyHourActive.
  ///
  /// In en, this message translates to:
  /// **'Happy Hour Active'**
  String get happyHourActive;

  /// No description provided for @comboSavings.
  ///
  /// In en, this message translates to:
  /// **'Combo Savings'**
  String get comboSavings;

  /// No description provided for @addComboToCart.
  ///
  /// In en, this message translates to:
  /// **'Add Combo to Cart'**
  String get addComboToCart;

  /// No description provided for @menuItemsRequired.
  ///
  /// In en, this message translates to:
  /// **'At least one menu item is required'**
  String get menuItemsRequired;

  /// No description provided for @deletePromotionConfirm.
  ///
  /// In en, this message translates to:
  /// **'Are you sure you want to delete this promotion?'**
  String get deletePromotionConfirm;

  /// No description provided for @promotionExpired.
  ///
  /// In en, this message translates to:
  /// **'Promotion Expired'**
  String get promotionExpired;

  /// No description provided for @specialInstructions.
  ///
  /// In en, this message translates to:
  /// **'Special Instructions'**
  String get specialInstructions;

  /// No description provided for @addSpecialInstructions.
  ///
  /// In en, this message translates to:
  /// **'Add special instructions'**
  String get addSpecialInstructions;

  /// No description provided for @specialInstructionsHint.
  ///
  /// In en, this message translates to:
  /// **'e.g. No onions, extra spicy, allergies...'**
  String get specialInstructionsHint;

  /// No description provided for @orderInstructions.
  ///
  /// In en, this message translates to:
  /// **'Order Instructions'**
  String get orderInstructions;

  /// No description provided for @allergens.
  ///
  /// In en, this message translates to:
  /// **'Allergens'**
  String get allergens;

  /// No description provided for @allergenWarning.
  ///
  /// In en, this message translates to:
  /// **'Allergen Warning'**
  String get allergenWarning;

  /// No description provided for @allergenWarningMessage.
  ///
  /// In en, this message translates to:
  /// **'Select allergens you want to be warned about'**
  String get allergenWarningMessage;

  /// No description provided for @dietaryPreferences.
  ///
  /// In en, this message translates to:
  /// **'Dietary Preferences'**
  String get dietaryPreferences;

  /// No description provided for @dietaryProfile.
  ///
  /// In en, this message translates to:
  /// **'Dietary Profile'**
  String get dietaryProfile;

  /// No description provided for @saveDietaryProfile.
  ///
  /// In en, this message translates to:
  /// **'Save Dietary Profile'**
  String get saveDietaryProfile;

  /// No description provided for @spiceLevel.
  ///
  /// In en, this message translates to:
  /// **'Spice Level'**
  String get spiceLevel;

  /// No description provided for @calories.
  ///
  /// In en, this message translates to:
  /// **'Calories'**
  String get calories;

  /// No description provided for @kcal.
  ///
  /// In en, this message translates to:
  /// **'kcal'**
  String get kcal;

  /// No description provided for @maxSpiceLevel.
  ///
  /// In en, this message translates to:
  /// **'Max Spice Level'**
  String get maxSpiceLevel;

  /// No description provided for @allergenAlerts.
  ///
  /// In en, this message translates to:
  /// **'Allergen Alerts'**
  String get allergenAlerts;

  /// No description provided for @noAllergenAlerts.
  ///
  /// In en, this message translates to:
  /// **'No allergen alerts set'**
  String get noAllergenAlerts;

  /// No description provided for @proceedWithAllergens.
  ///
  /// In en, this message translates to:
  /// **'Proceed Anyway'**
  String get proceedWithAllergens;

  /// No description provided for @allergenFound.
  ///
  /// In en, this message translates to:
  /// **'Allergen found in your cart'**
  String get allergenFound;

  /// No description provided for @filterByDiet.
  ///
  /// In en, this message translates to:
  /// **'Filter by Diet'**
  String get filterByDiet;

  /// No description provided for @allergenGluten.
  ///
  /// In en, this message translates to:
  /// **'Gluten'**
  String get allergenGluten;

  /// No description provided for @allergenDairy.
  ///
  /// In en, this message translates to:
  /// **'Dairy'**
  String get allergenDairy;

  /// No description provided for @allergenNuts.
  ///
  /// In en, this message translates to:
  /// **'Nuts'**
  String get allergenNuts;

  /// No description provided for @allergenPeanuts.
  ///
  /// In en, this message translates to:
  /// **'Peanuts'**
  String get allergenPeanuts;

  /// No description provided for @allergenShellfish.
  ///
  /// In en, this message translates to:
  /// **'Shellfish'**
  String get allergenShellfish;

  /// No description provided for @allergenSoy.
  ///
  /// In en, this message translates to:
  /// **'Soy'**
  String get allergenSoy;

  /// No description provided for @allergenEggs.
  ///
  /// In en, this message translates to:
  /// **'Eggs'**
  String get allergenEggs;

  /// No description provided for @allergenFish.
  ///
  /// In en, this message translates to:
  /// **'Fish'**
  String get allergenFish;

  /// No description provided for @allergenSesame.
  ///
  /// In en, this message translates to:
  /// **'Sesame'**
  String get allergenSesame;

  /// No description provided for @allergenMustard.
  ///
  /// In en, this message translates to:
  /// **'Mustard'**
  String get allergenMustard;

  /// No description provided for @allergenCelery.
  ///
  /// In en, this message translates to:
  /// **'Celery'**
  String get allergenCelery;

  /// No description provided for @allergenLupin.
  ///
  /// In en, this message translates to:
  /// **'Lupin'**
  String get allergenLupin;

  /// No description provided for @allergenMolluscs.
  ///
  /// In en, this message translates to:
  /// **'Molluscs'**
  String get allergenMolluscs;

  /// No description provided for @allergenSulfites.
  ///
  /// In en, this message translates to:
  /// **'Sulfites'**
  String get allergenSulfites;

  /// No description provided for @dietaryVegan.
  ///
  /// In en, this message translates to:
  /// **'Vegan'**
  String get dietaryVegan;

  /// No description provided for @dietaryGlutenFree.
  ///
  /// In en, this message translates to:
  /// **'Gluten-Free'**
  String get dietaryGlutenFree;

  /// No description provided for @dietaryDairyFree.
  ///
  /// In en, this message translates to:
  /// **'Dairy-Free'**
  String get dietaryDairyFree;

  /// No description provided for @dietaryNutFree.
  ///
  /// In en, this message translates to:
  /// **'Nut-Free'**
  String get dietaryNutFree;

  /// No description provided for @dietaryKeto.
  ///
  /// In en, this message translates to:
  /// **'Keto'**
  String get dietaryKeto;

  /// No description provided for @dietaryHalal.
  ///
  /// In en, this message translates to:
  /// **'Halal'**
  String get dietaryHalal;

  /// No description provided for @dietaryJain.
  ///
  /// In en, this message translates to:
  /// **'Jain'**
  String get dietaryJain;

  /// No description provided for @dietaryOrganic.
  ///
  /// In en, this message translates to:
  /// **'Organic'**
  String get dietaryOrganic;

  /// No description provided for @dietarySugarFree.
  ///
  /// In en, this message translates to:
  /// **'Sugar-Free'**
  String get dietarySugarFree;

  /// No description provided for @dietaryHighProtein.
  ///
  /// In en, this message translates to:
  /// **'High Protein'**
  String get dietaryHighProtein;

  /// No description provided for @spiceNone.
  ///
  /// In en, this message translates to:
  /// **'None'**
  String get spiceNone;

  /// No description provided for @spiceMild.
  ///
  /// In en, this message translates to:
  /// **'Mild'**
  String get spiceMild;

  /// No description provided for @spiceMedium.
  ///
  /// In en, this message translates to:
  /// **'Medium'**
  String get spiceMedium;

  /// No description provided for @spiceHot.
  ///
  /// In en, this message translates to:
  /// **'Hot'**
  String get spiceHot;

  /// No description provided for @spiceExtraHot.
  ///
  /// In en, this message translates to:
  /// **'Extra Hot'**
  String get spiceExtraHot;

  /// No description provided for @groupOrder.
  ///
  /// In en, this message translates to:
  /// **'Group Order'**
  String get groupOrder;

  /// No description provided for @groupOrderCreate.
  ///
  /// In en, this message translates to:
  /// **'Create Group Order'**
  String get groupOrderCreate;

  /// No description provided for @groupOrderJoin.
  ///
  /// In en, this message translates to:
  /// **'Join Group Order'**
  String get groupOrderJoin;

  /// No description provided for @groupOrderLobby.
  ///
  /// In en, this message translates to:
  /// **'Group Order Lobby'**
  String get groupOrderLobby;

  /// No description provided for @groupOrderMenu.
  ///
  /// In en, this message translates to:
  /// **'Group Order Menu'**
  String get groupOrderMenu;

  /// No description provided for @groupOrderCheckout.
  ///
  /// In en, this message translates to:
  /// **'Group Order Checkout'**
  String get groupOrderCheckout;

  /// No description provided for @groupOrderInviteCode.
  ///
  /// In en, this message translates to:
  /// **'Invite Code'**
  String get groupOrderInviteCode;

  /// No description provided for @groupOrderShareCode.
  ///
  /// In en, this message translates to:
  /// **'Share this code with friends'**
  String get groupOrderShareCode;

  /// No description provided for @groupOrderCodeCopied.
  ///
  /// In en, this message translates to:
  /// **'Invite code copied!'**
  String get groupOrderCodeCopied;

  /// No description provided for @groupOrderEnterCode.
  ///
  /// In en, this message translates to:
  /// **'Enter invite code'**
  String get groupOrderEnterCode;

  /// No description provided for @groupOrderJoinButton.
  ///
  /// In en, this message translates to:
  /// **'Join'**
  String get groupOrderJoinButton;

  /// No description provided for @groupOrderParticipants.
  ///
  /// In en, this message translates to:
  /// **'Participants'**
  String get groupOrderParticipants;

  /// No description provided for @groupOrderYourItems.
  ///
  /// In en, this message translates to:
  /// **'Your Items'**
  String get groupOrderYourItems;

  /// No description provided for @groupOrderCombinedCart.
  ///
  /// In en, this message translates to:
  /// **'Combined Cart'**
  String get groupOrderCombinedCart;

  /// No description provided for @groupOrderGrandTotal.
  ///
  /// In en, this message translates to:
  /// **'Grand Total'**
  String get groupOrderGrandTotal;

  /// No description provided for @groupOrderMarkReady.
  ///
  /// In en, this message translates to:
  /// **'I\'m Ready'**
  String get groupOrderMarkReady;

  /// No description provided for @groupOrderLeave.
  ///
  /// In en, this message translates to:
  /// **'Leave Group'**
  String get groupOrderLeave;

  /// No description provided for @groupOrderFinalize.
  ///
  /// In en, this message translates to:
  /// **'Finalize Order'**
  String get groupOrderFinalize;

  /// No description provided for @groupOrderCancel.
  ///
  /// In en, this message translates to:
  /// **'Cancel Group Order'**
  String get groupOrderCancel;

  /// No description provided for @groupOrderExpired.
  ///
  /// In en, this message translates to:
  /// **'Group Order Expired'**
  String get groupOrderExpired;

  /// No description provided for @groupOrderCancelled.
  ///
  /// In en, this message translates to:
  /// **'Group Order Cancelled'**
  String get groupOrderCancelled;

  /// No description provided for @groupOrderFinalized.
  ///
  /// In en, this message translates to:
  /// **'Group Order Finalized'**
  String get groupOrderFinalized;

  /// No description provided for @groupOrderWaitingForReady.
  ///
  /// In en, this message translates to:
  /// **'Waiting for all participants to be ready'**
  String get groupOrderWaitingForReady;

  /// No description provided for @groupOrderAllReady.
  ///
  /// In en, this message translates to:
  /// **'All participants are ready!'**
  String get groupOrderAllReady;

  /// No description provided for @groupOrderExpiresIn.
  ///
  /// In en, this message translates to:
  /// **'Expires in'**
  String get groupOrderExpiresIn;

  /// No description provided for @groupOrderPaymentSplit.
  ///
  /// In en, this message translates to:
  /// **'Payment Split'**
  String get groupOrderPaymentSplit;

  /// No description provided for @groupOrderIllPay.
  ///
  /// In en, this message translates to:
  /// **'I\'ll Pay'**
  String get groupOrderIllPay;

  /// No description provided for @groupOrderSplitEqual.
  ///
  /// In en, this message translates to:
  /// **'Split Equal'**
  String get groupOrderSplitEqual;

  /// No description provided for @groupOrderPayYourShare.
  ///
  /// In en, this message translates to:
  /// **'Pay Your Share'**
  String get groupOrderPayYourShare;

  /// No description provided for @groupOrderConfirmLeave.
  ///
  /// In en, this message translates to:
  /// **'Are you sure you want to leave this group order?'**
  String get groupOrderConfirmLeave;

  /// No description provided for @groupOrderConfirmCancel.
  ///
  /// In en, this message translates to:
  /// **'Are you sure you want to cancel this group order?'**
  String get groupOrderConfirmCancel;

  /// No description provided for @groupOrderConfirmFinalize.
  ///
  /// In en, this message translates to:
  /// **'Are you sure you want to finalize this group order?'**
  String get groupOrderConfirmFinalize;

  /// No description provided for @groupOrderNoItems.
  ///
  /// In en, this message translates to:
  /// **'No items in cart yet'**
  String get groupOrderNoItems;

  /// No description provided for @groupOrderHostLabel.
  ///
  /// In en, this message translates to:
  /// **'Host'**
  String get groupOrderHostLabel;

  /// No description provided for @groupOrderStatusJoined.
  ///
  /// In en, this message translates to:
  /// **'Joined'**
  String get groupOrderStatusJoined;

  /// No description provided for @groupOrderStatusReady.
  ///
  /// In en, this message translates to:
  /// **'Ready'**
  String get groupOrderStatusReady;

  /// No description provided for @groupOrderStatusLeft.
  ///
  /// In en, this message translates to:
  /// **'Left'**
  String get groupOrderStatusLeft;

  /// No description provided for @groupOrderItemsCount.
  ///
  /// In en, this message translates to:
  /// **'{count} items'**
  String groupOrderItemsCount(int count);

  /// No description provided for @groupOrderStartFromRestaurant.
  ///
  /// In en, this message translates to:
  /// **'Start a group order from a restaurant page'**
  String get groupOrderStartFromRestaurant;

  /// No description provided for @loyaltyRewards.
  ///
  /// In en, this message translates to:
  /// **'Loyalty & Rewards'**
  String get loyaltyRewards;

  /// No description provided for @loyaltyPoints.
  ///
  /// In en, this message translates to:
  /// **'Points'**
  String get loyaltyPoints;

  /// No description provided for @loyaltyBalance.
  ///
  /// In en, this message translates to:
  /// **'Current Balance'**
  String get loyaltyBalance;

  /// No description provided for @loyaltyTier.
  ///
  /// In en, this message translates to:
  /// **'Tier'**
  String get loyaltyTier;

  /// No description provided for @loyaltyBronze.
  ///
  /// In en, this message translates to:
  /// **'Bronze'**
  String get loyaltyBronze;

  /// No description provided for @loyaltySilver.
  ///
  /// In en, this message translates to:
  /// **'Silver'**
  String get loyaltySilver;

  /// No description provided for @loyaltyGold.
  ///
  /// In en, this message translates to:
  /// **'Gold'**
  String get loyaltyGold;

  /// No description provided for @loyaltyPlatinum.
  ///
  /// In en, this message translates to:
  /// **'Platinum'**
  String get loyaltyPlatinum;

  /// No description provided for @loyaltyNextTier.
  ///
  /// In en, this message translates to:
  /// **'Next Tier'**
  String get loyaltyNextTier;

  /// No description provided for @loyaltyPointsToNext.
  ///
  /// In en, this message translates to:
  /// **'points to'**
  String get loyaltyPointsToNext;

  /// No description provided for @loyaltyLifetimeEarned.
  ///
  /// In en, this message translates to:
  /// **'Lifetime Earned'**
  String get loyaltyLifetimeEarned;

  /// No description provided for @loyaltyRedeemRewards.
  ///
  /// In en, this message translates to:
  /// **'Redeem Rewards'**
  String get loyaltyRedeemRewards;

  /// No description provided for @loyaltyPointsHistory.
  ///
  /// In en, this message translates to:
  /// **'Points History'**
  String get loyaltyPointsHistory;

  /// No description provided for @loyaltyRecentActivity.
  ///
  /// In en, this message translates to:
  /// **'Recent Activity'**
  String get loyaltyRecentActivity;

  /// No description provided for @loyaltyEarned.
  ///
  /// In en, this message translates to:
  /// **'Earned'**
  String get loyaltyEarned;

  /// No description provided for @loyaltyRedeemed.
  ///
  /// In en, this message translates to:
  /// **'Redeemed'**
  String get loyaltyRedeemed;

  /// No description provided for @loyaltyExpired.
  ///
  /// In en, this message translates to:
  /// **'Expired'**
  String get loyaltyExpired;

  /// No description provided for @loyaltyAdjustment.
  ///
  /// In en, this message translates to:
  /// **'Adjustment'**
  String get loyaltyAdjustment;

  /// No description provided for @loyaltyRedeemConfirm.
  ///
  /// In en, this message translates to:
  /// **'Confirm Redemption'**
  String get loyaltyRedeemConfirm;

  /// No description provided for @loyaltyRedeemSuccess.
  ///
  /// In en, this message translates to:
  /// **'Reward redeemed successfully!'**
  String get loyaltyRedeemSuccess;

  /// No description provided for @loyaltyInsufficientPoints.
  ///
  /// In en, this message translates to:
  /// **'Insufficient loyalty points'**
  String get loyaltyInsufficientPoints;

  /// No description provided for @loyaltyNoTransactions.
  ///
  /// In en, this message translates to:
  /// **'No transactions yet'**
  String get loyaltyNoTransactions;

  /// No description provided for @loyaltyNoRewards.
  ///
  /// In en, this message translates to:
  /// **'No rewards available'**
  String get loyaltyNoRewards;

  /// No description provided for @loyaltyRewardPoints.
  ///
  /// In en, this message translates to:
  /// **'Points Cost'**
  String get loyaltyRewardPoints;

  /// No description provided for @loyaltyFilterAll.
  ///
  /// In en, this message translates to:
  /// **'All'**
  String get loyaltyFilterAll;

  /// No description provided for @loyaltyFilterEarn.
  ///
  /// In en, this message translates to:
  /// **'Earned'**
  String get loyaltyFilterEarn;

  /// No description provided for @loyaltyFilterRedeem.
  ///
  /// In en, this message translates to:
  /// **'Redeemed'**
  String get loyaltyFilterRedeem;

  /// No description provided for @disputes.
  ///
  /// In en, this message translates to:
  /// **'Disputes'**
  String get disputes;

  /// No description provided for @reportIssue.
  ///
  /// In en, this message translates to:
  /// **'Report Issue'**
  String get reportIssue;

  /// No description provided for @reportOrderIssue.
  ///
  /// In en, this message translates to:
  /// **'Report Order Issue'**
  String get reportOrderIssue;

  /// No description provided for @disputeCreated.
  ///
  /// In en, this message translates to:
  /// **'Dispute created successfully'**
  String get disputeCreated;

  /// No description provided for @disputeResolved.
  ///
  /// In en, this message translates to:
  /// **'Dispute resolved'**
  String get disputeResolved;

  /// No description provided for @disputeRejected.
  ///
  /// In en, this message translates to:
  /// **'Dispute rejected'**
  String get disputeRejected;

  /// No description provided for @disputeEscalated.
  ///
  /// In en, this message translates to:
  /// **'Dispute escalated'**
  String get disputeEscalated;

  /// No description provided for @selectOrder.
  ///
  /// In en, this message translates to:
  /// **'Select Order'**
  String get selectOrder;

  /// No description provided for @selectIssueType.
  ///
  /// In en, this message translates to:
  /// **'Select Issue Type'**
  String get selectIssueType;

  /// No description provided for @issueDescription.
  ///
  /// In en, this message translates to:
  /// **'Issue Description'**
  String get issueDescription;

  /// No description provided for @issueDescriptionHint.
  ///
  /// In en, this message translates to:
  /// **'Describe the issue in detail...'**
  String get issueDescriptionHint;

  /// No description provided for @issueWrongItems.
  ///
  /// In en, this message translates to:
  /// **'Wrong Items'**
  String get issueWrongItems;

  /// No description provided for @issueMissingItems.
  ///
  /// In en, this message translates to:
  /// **'Missing Items'**
  String get issueMissingItems;

  /// No description provided for @issueQualityIssue.
  ///
  /// In en, this message translates to:
  /// **'Quality Issue'**
  String get issueQualityIssue;

  /// No description provided for @issueLateDelivery.
  ///
  /// In en, this message translates to:
  /// **'Late Delivery'**
  String get issueLateDelivery;

  /// No description provided for @issueNeverDelivered.
  ///
  /// In en, this message translates to:
  /// **'Never Delivered'**
  String get issueNeverDelivered;

  /// No description provided for @issueWrongOrder.
  ///
  /// In en, this message translates to:
  /// **'Wrong Order'**
  String get issueWrongOrder;

  /// No description provided for @issueDamagedPackaging.
  ///
  /// In en, this message translates to:
  /// **'Damaged Packaging'**
  String get issueDamagedPackaging;

  /// No description provided for @issueOther.
  ///
  /// In en, this message translates to:
  /// **'Other'**
  String get issueOther;

  /// No description provided for @disputeOpened.
  ///
  /// In en, this message translates to:
  /// **'Opened'**
  String get disputeOpened;

  /// No description provided for @disputeUnderReview.
  ///
  /// In en, this message translates to:
  /// **'Under Review'**
  String get disputeUnderReview;

  /// No description provided for @disputeAwaitingResponse.
  ///
  /// In en, this message translates to:
  /// **'Awaiting Response'**
  String get disputeAwaitingResponse;

  /// No description provided for @disputeStatusResolved.
  ///
  /// In en, this message translates to:
  /// **'Resolved'**
  String get disputeStatusResolved;

  /// No description provided for @disputeStatusClosed.
  ///
  /// In en, this message translates to:
  /// **'Closed'**
  String get disputeStatusClosed;

  /// No description provided for @disputeStatusEscalated.
  ///
  /// In en, this message translates to:
  /// **'Escalated'**
  String get disputeStatusEscalated;

  /// No description provided for @disputeStatusRejected.
  ///
  /// In en, this message translates to:
  /// **'Rejected'**
  String get disputeStatusRejected;

  /// No description provided for @noDisputesYet.
  ///
  /// In en, this message translates to:
  /// **'No disputes filed yet'**
  String get noDisputesYet;

  /// No description provided for @noDisputesDescription.
  ///
  /// In en, this message translates to:
  /// **'If you have an issue with an order, report it here.'**
  String get noDisputesDescription;

  /// No description provided for @resolutionDetails.
  ///
  /// In en, this message translates to:
  /// **'Resolution Details'**
  String get resolutionDetails;

  /// No description provided for @rejectionReason.
  ///
  /// In en, this message translates to:
  /// **'Rejection Reason'**
  String get rejectionReason;

  /// No description provided for @disputeAutoResolved.
  ///
  /// In en, this message translates to:
  /// **'Auto-resolved with wallet credit'**
  String get disputeAutoResolved;

  /// No description provided for @submitDispute.
  ///
  /// In en, this message translates to:
  /// **'Submit Dispute'**
  String get submitDispute;

  /// No description provided for @helpful.
  ///
  /// In en, this message translates to:
  /// **'Helpful'**
  String get helpful;

  /// No description provided for @markHelpful.
  ///
  /// In en, this message translates to:
  /// **'Mark as helpful'**
  String get markHelpful;

  /// No description provided for @helpfulCount.
  ///
  /// In en, this message translates to:
  /// **'Helpful'**
  String get helpfulCount;

  /// No description provided for @reportReview.
  ///
  /// In en, this message translates to:
  /// **'Report Review'**
  String get reportReview;

  /// No description provided for @reportReason.
  ///
  /// In en, this message translates to:
  /// **'Report Reason'**
  String get reportReason;

  /// No description provided for @reportSubmitted.
  ///
  /// In en, this message translates to:
  /// **'Report submitted'**
  String get reportSubmitted;

  /// No description provided for @reportReasonSpam.
  ///
  /// In en, this message translates to:
  /// **'Spam'**
  String get reportReasonSpam;

  /// No description provided for @reportReasonInappropriate.
  ///
  /// In en, this message translates to:
  /// **'Inappropriate'**
  String get reportReasonInappropriate;

  /// No description provided for @reportReasonFakeReview.
  ///
  /// In en, this message translates to:
  /// **'Fake Review'**
  String get reportReasonFakeReview;

  /// No description provided for @reportReasonHarassment.
  ///
  /// In en, this message translates to:
  /// **'Harassment'**
  String get reportReasonHarassment;

  /// No description provided for @reportReasonOther.
  ///
  /// In en, this message translates to:
  /// **'Other'**
  String get reportReasonOther;

  /// No description provided for @reportDescription.
  ///
  /// In en, this message translates to:
  /// **'Additional details'**
  String get reportDescription;

  /// No description provided for @reviewAnalytics.
  ///
  /// In en, this message translates to:
  /// **'Review Analytics'**
  String get reviewAnalytics;

  /// No description provided for @ratingDistribution.
  ///
  /// In en, this message translates to:
  /// **'Rating Distribution'**
  String get ratingDistribution;

  /// No description provided for @monthlyTrend.
  ///
  /// In en, this message translates to:
  /// **'Monthly Trend'**
  String get monthlyTrend;

  /// No description provided for @photoReviews.
  ///
  /// In en, this message translates to:
  /// **'Photo Reviews'**
  String get photoReviews;

  /// No description provided for @addPhotos.
  ///
  /// In en, this message translates to:
  /// **'Add Photos'**
  String get addPhotos;

  /// No description provided for @maxPhotosReached.
  ///
  /// In en, this message translates to:
  /// **'Maximum 5 photos allowed'**
  String get maxPhotosReached;

  /// No description provided for @editReply.
  ///
  /// In en, this message translates to:
  /// **'Edit Reply'**
  String get editReply;

  /// No description provided for @deleteReply.
  ///
  /// In en, this message translates to:
  /// **'Delete Reply'**
  String get deleteReply;

  /// No description provided for @deleteReplyConfirm.
  ///
  /// In en, this message translates to:
  /// **'Are you sure you want to delete this reply?'**
  String get deleteReplyConfirm;

  /// No description provided for @recommendedForYou.
  ///
  /// In en, this message translates to:
  /// **'Recommended For You'**
  String get recommendedForYou;

  /// No description provided for @dishesYoullLove.
  ///
  /// In en, this message translates to:
  /// **'Dishes You\'ll Love'**
  String get dishesYoullLove;

  /// No description provided for @trendingNearYou.
  ///
  /// In en, this message translates to:
  /// **'Trending Near You'**
  String get trendingNearYou;

  /// No description provided for @similarRestaurants.
  ///
  /// In en, this message translates to:
  /// **'Similar Restaurants'**
  String get similarRestaurants;

  /// No description provided for @customersAlsoOrdered.
  ///
  /// In en, this message translates to:
  /// **'Customers Also Ordered'**
  String get customersAlsoOrdered;

  /// No description provided for @becauseYouLike.
  ///
  /// In en, this message translates to:
  /// **'Because you like {cuisine}'**
  String becauseYouLike(String cuisine);

  /// No description provided for @popularInYourArea.
  ///
  /// In en, this message translates to:
  /// **'Popular in your area'**
  String get popularInYourArea;

  /// No description provided for @quickDeliveryReason.
  ///
  /// In en, this message translates to:
  /// **'Quick delivery'**
  String get quickDeliveryReason;

  /// No description provided for @greatDealAvailable.
  ///
  /// In en, this message translates to:
  /// **'Great deal available'**
  String get greatDealAvailable;

  /// No description provided for @trendingNow.
  ///
  /// In en, this message translates to:
  /// **'Trending now'**
  String get trendingNow;

  /// No description provided for @ordersToday.
  ///
  /// In en, this message translates to:
  /// **'{count} orders today'**
  String ordersToday(int count);

  /// No description provided for @noRecommendationsYet.
  ///
  /// In en, this message translates to:
  /// **'No recommendations yet'**
  String get noRecommendationsYet;

  /// No description provided for @basedOnYourOrders.
  ///
  /// In en, this message translates to:
  /// **'Based on your orders'**
  String get basedOnYourOrders;

  /// No description provided for @trendingRank.
  ///
  /// In en, this message translates to:
  /// **'#{rank}'**
  String trendingRank(int rank);

  /// No description provided for @matchScore.
  ///
  /// In en, this message translates to:
  /// **'Match score'**
  String get matchScore;

  /// No description provided for @customersAlsoLoved.
  ///
  /// In en, this message translates to:
  /// **'Customers also loved'**
  String get customersAlsoLoved;

  /// No description provided for @loadingRecommendations.
  ///
  /// In en, this message translates to:
  /// **'Loading recommendations'**
  String get loadingRecommendations;
}

class _AppLocalizationsDelegate
    extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) =>
      <String>['en', 'hi'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {
  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'en':
      return AppLocalizationsEn();
    case 'hi':
      return AppLocalizationsHi();
  }

  throw FlutterError(
      'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
      'an issue with the localizations generation tool. Please file an issue '
      'on GitHub with a reproducible sample app and the gen-l10n configuration '
      'that was used.');
}
