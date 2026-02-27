import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:smooth_page_indicator/smooth_page_indicator.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/storage/local_storage_service.dart';
import '../../../../routing/route_names.dart';

class OnboardingScreen extends StatefulWidget {
  const OnboardingScreen({super.key});

  @override
  State<OnboardingScreen> createState() => _OnboardingScreenState();
}

class _OnboardingScreenState extends State<OnboardingScreen> {
  final _pageController = PageController();
  int _currentPage = 0;

  static const _pages = <_OnboardingPageData>[
    _OnboardingPageData(
      icon: Icons.restaurant_menu_rounded,
      title: 'Discover Restaurants',
      description:
          'Browse thousands of restaurants near you and find your next favourite meal.',
      color: AppColors.primary,
    ),
    _OnboardingPageData(
      icon: Icons.delivery_dining_rounded,
      title: 'Fast Delivery',
      description:
          'Get your food delivered to your doorstep in minutes with real-time tracking.',
      color: AppColors.secondary,
    ),
    _OnboardingPageData(
      icon: Icons.table_restaurant_rounded,
      title: 'Dine-In Experience',
      description:
          'Scan, order, and pay right from your table — no waiting for the server.',
      color: AppColors.info,
    ),
  ];

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final isLastPage = _currentPage == _pages.length - 1;

    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Column(
          children: [
            // Skip button
            Align(
              alignment: Alignment.centerRight,
              child: Padding(
                padding: const EdgeInsets.only(right: 8, top: 8),
                child: TextButton(
                  onPressed: _completeOnboarding,
                  child: Text(
                    'Skip',
                    style: theme.textTheme.bodyLarge?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                  ),
                ),
              ),
            ),

            // Pages
            Expanded(
              child: PageView.builder(
                controller: _pageController,
                itemCount: _pages.length,
                onPageChanged: (index) {
                  setState(() => _currentPage = index);
                },
                itemBuilder: (context, index) {
                  final page = _pages[index];
                  return _OnboardingPage(data: page);
                },
              ),
            ),

            // Dot indicator
            Padding(
              padding: const EdgeInsets.only(bottom: 24),
              child: SmoothPageIndicator(
                controller: _pageController,
                count: _pages.length,
                effect: WormEffect(
                  dotWidth: 10,
                  dotHeight: 10,
                  spacing: 12,
                  activeDotColor: AppColors.primary,
                  dotColor: AppColors.primary.withValues(alpha: 0.2),
                ),
              ),
            ),

            // Navigation button
            Padding(
              padding: const EdgeInsets.fromLTRB(24, 0, 24, 32),
              child: FilledButton(
                onPressed: isLastPage
                    ? _completeOnboarding
                    : () => _pageController.nextPage(
                          duration: const Duration(milliseconds: 300),
                          curve: Curves.easeInOut,
                        ),
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  minimumSize: const Size.fromHeight(56),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(16),
                  ),
                ),
                child: Text(
                  isLastPage ? 'Get Started' : 'Next',
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _completeOnboarding() async {
    await LocalStorageService.instance.put<bool>(
      LocalStorageKeys.onboardingCompleted,
      true,
    );
    if (mounted) context.go(RouteNames.login);
  }
}

// ────────────────────────────────────────────────────────────────────────────
// Data & Page Widget
// ────────────────────────────────────────────────────────────────────────────

class _OnboardingPageData {
  const _OnboardingPageData({
    required this.icon,
    required this.title,
    required this.description,
    required this.color,
  });

  final IconData icon;
  final String title;
  final String description;
  final Color color;
}

class _OnboardingPage extends StatelessWidget {
  const _OnboardingPage({required this.data});

  final _OnboardingPageData data;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 32),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          // Icon container
          Container(
            width: 160,
            height: 160,
            decoration: BoxDecoration(
              color: data.color.withValues(alpha: 0.1),
              shape: BoxShape.circle,
            ),
            child: Icon(
              data.icon,
              size: 80,
              color: data.color,
            ),
          ),
          const SizedBox(height: 48),
          // Title
          Text(
            data.title,
            style: theme.textTheme.headlineMedium?.copyWith(
              fontWeight: FontWeight.bold,
              color: AppColors.textPrimaryLight,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 16),
          // Description
          Text(
            data.description,
            style: theme.textTheme.bodyLarge?.copyWith(
              color: AppColors.textSecondaryLight,
              height: 1.5,
            ),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }
}
