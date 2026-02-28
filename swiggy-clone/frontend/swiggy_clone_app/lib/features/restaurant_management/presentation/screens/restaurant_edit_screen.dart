import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/cuisine_type_model.dart';
import '../providers/cuisine_types_provider.dart';
import '../providers/restaurant_detail_notifier.dart';
import '../providers/restaurant_detail_state.dart';

/// Form screen for editing an existing restaurant. Pre-filled with data.
///
/// Takes [restaurantId] as a parameter, loads the detail, and submits
/// updates via the notifier.
class RestaurantEditScreen extends ConsumerStatefulWidget {
  const RestaurantEditScreen({
    required this.restaurantId,
    super.key,
  });

  final String restaurantId;

  @override
  ConsumerState<RestaurantEditScreen> createState() =>
      _RestaurantEditScreenState();
}

class _RestaurantEditScreenState extends ConsumerState<RestaurantEditScreen> {
  final _formKey = GlobalKey<FormState>();

  final _nameController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _phoneController = TextEditingController();
  final _emailController = TextEditingController();
  final _addressLine1Controller = TextEditingController();
  final _addressLine2Controller = TextEditingController();
  final _cityController = TextEditingController();
  final _stateController = TextEditingController();
  final _postalCodeController = TextEditingController();
  final _latController = TextEditingController();
  final _lonController = TextEditingController();
  final _avgCostController = TextEditingController();

  bool _isVegOnly = false;
  bool _isSubmitting = false;
  bool _initialized = false;
  final Set<String> _selectedCuisineIds = {};

  @override
  void dispose() {
    _nameController.dispose();
    _descriptionController.dispose();
    _phoneController.dispose();
    _emailController.dispose();
    _addressLine1Controller.dispose();
    _addressLine2Controller.dispose();
    _cityController.dispose();
    _stateController.dispose();
    _postalCodeController.dispose();
    _latController.dispose();
    _lonController.dispose();
    _avgCostController.dispose();
    super.dispose();
  }

  void _populateFields() {
    final state =
        ref.read(restaurantDetailNotifierProvider(widget.restaurantId));
    if (state is! RestaurantDetailLoaded || _initialized) return;
    final r = state.restaurant;

    _nameController.text = r.name;
    _descriptionController.text = r.description ?? '';
    _phoneController.text = r.phoneNumber ?? '';
    _emailController.text = r.email ?? '';
    _addressLine1Controller.text = r.addressLine1 ?? '';
    _addressLine2Controller.text = r.addressLine2 ?? '';
    _cityController.text = r.city ?? '';
    _stateController.text = r.state ?? '';
    _postalCodeController.text = r.postalCode ?? '';
    _latController.text = r.latitude?.toString() ?? '';
    _lonController.text = r.longitude?.toString() ?? '';
    _avgCostController.text = r.avgCostForTwo?.toString() ?? '';
    _isVegOnly = r.isVegOnly;
    _selectedCuisineIds
      ..clear()
      ..addAll(r.cuisines.map((c) => c.id));
    _initialized = true;
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isSubmitting = true);

    final data = <String, dynamic>{
      'name': _nameController.text.trim(),
      'description': _descriptionController.text.trim(),
      'phoneNumber': _phoneController.text.trim(),
      'email': _emailController.text.trim(),
      'addressLine1': _addressLine1Controller.text.trim(),
      'addressLine2': _addressLine2Controller.text.trim(),
      'city': _cityController.text.trim(),
      'state': _stateController.text.trim(),
      'postalCode': _postalCodeController.text.trim(),
      'isVegOnly': _isVegOnly,
      if (_latController.text.trim().isNotEmpty)
        'latitude': double.tryParse(_latController.text.trim()),
      if (_lonController.text.trim().isNotEmpty)
        'longitude': double.tryParse(_lonController.text.trim()),
      if (_avgCostController.text.trim().isNotEmpty)
        'avgCostForTwo': int.tryParse(_avgCostController.text.trim()),
      'cuisineIds': _selectedCuisineIds.toList(),
    };

    final success = await ref
        .read(restaurantDetailNotifierProvider(widget.restaurantId).notifier)
        .updateRestaurant(data);

    if (!mounted) return;
    setState(() => _isSubmitting = false);

    if (success) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Restaurant updated successfully!'),
          backgroundColor: AppColors.success,
        ),
      );
      context.pop();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Failed to update restaurant. Please try again.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final detailState =
        ref.watch(restaurantDetailNotifierProvider(widget.restaurantId));
    final theme = Theme.of(context);
    final cuisinesAsync = ref.watch(cuisineTypesProvider);

    ref.listen(restaurantDetailNotifierProvider(widget.restaurantId),
        (_, next) {
      if (next is RestaurantDetailLoaded && !_initialized) {
        WidgetsBinding.instance.addPostFrameCallback((_) {
          if (mounted) {
            _populateFields();
            setState(() {});
          }
        });
      }
    });

    return Scaffold(
      appBar: AppBar(
        title: const Text('Edit Restaurant'),
      ),
      body: switch (detailState) {
        RestaurantDetailInitial() || RestaurantDetailLoading() =>
          const AppLoadingWidget(message: 'Loading restaurant details...'),
        RestaurantDetailError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(restaurantDetailNotifierProvider(widget.restaurantId)
                    .notifier)
                .loadRestaurant(),
          ),
        RestaurantDetailLoaded() => _buildForm(theme, cuisinesAsync),
      },
    );
  }

  Widget _buildForm(ThemeData theme, AsyncValue<List<CuisineTypeModel>> cuisinesAsync) {
    // Ensure fields are populated
    _populateFields();

    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // ──── Basic Info ────
            Text(
              'Basic Information',
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),

            TextFormField(
              controller: _nameController,
              decoration: const InputDecoration(
                labelText: 'Restaurant Name *',
                prefixIcon: Icon(Icons.restaurant),
              ),
              textCapitalization: TextCapitalization.words,
              validator: (v) =>
                  v == null || v.trim().isEmpty ? 'Name is required' : null,
            ),
            const SizedBox(height: 12),

            TextFormField(
              controller: _descriptionController,
              decoration: const InputDecoration(
                labelText: 'Description',
                prefixIcon: Icon(Icons.description_outlined),
              ),
              maxLines: 3,
              minLines: 1,
            ),
            const SizedBox(height: 12),

            TextFormField(
              controller: _phoneController,
              decoration: const InputDecoration(
                labelText: 'Phone Number',
                prefixIcon: Icon(Icons.phone_outlined),
              ),
              keyboardType: TextInputType.phone,
            ),
            const SizedBox(height: 12),

            TextFormField(
              controller: _emailController,
              decoration: const InputDecoration(
                labelText: 'Email',
                prefixIcon: Icon(Icons.email_outlined),
              ),
              keyboardType: TextInputType.emailAddress,
            ),
            const SizedBox(height: 24),

            // ──── Address ────
            Text(
              'Address',
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),

            TextFormField(
              controller: _addressLine1Controller,
              decoration: const InputDecoration(
                labelText: 'Address Line 1',
                prefixIcon: Icon(Icons.location_on_outlined),
              ),
            ),
            const SizedBox(height: 12),

            TextFormField(
              controller: _addressLine2Controller,
              decoration: const InputDecoration(
                labelText: 'Address Line 2',
                prefixIcon: Icon(Icons.location_city_outlined),
              ),
            ),
            const SizedBox(height: 12),

            Row(
              children: [
                Expanded(
                  child: TextFormField(
                    controller: _cityController,
                    decoration: const InputDecoration(labelText: 'City'),
                    textCapitalization: TextCapitalization.words,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: TextFormField(
                    controller: _stateController,
                    decoration: const InputDecoration(labelText: 'State'),
                    textCapitalization: TextCapitalization.words,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),

            TextFormField(
              controller: _postalCodeController,
              decoration: const InputDecoration(
                labelText: 'Postal Code',
                prefixIcon: Icon(Icons.pin_outlined),
              ),
              keyboardType: TextInputType.number,
              inputFormatters: [FilteringTextInputFormatter.digitsOnly],
            ),
            const SizedBox(height: 12),

            Row(
              children: [
                Expanded(
                  child: TextFormField(
                    controller: _latController,
                    decoration:
                        const InputDecoration(labelText: 'Latitude'),
                    keyboardType: const TextInputType.numberWithOptions(
                        decimal: true),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: TextFormField(
                    controller: _lonController,
                    decoration:
                        const InputDecoration(labelText: 'Longitude'),
                    keyboardType: const TextInputType.numberWithOptions(
                        decimal: true),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 24),

            // ──── Settings ────
            Text(
              'Settings',
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),

            SwitchListTile(
              title: const Text('Veg Only Restaurant'),
              subtitle: const Text('Only serves vegetarian food'),
              value: _isVegOnly,
              onChanged: (v) => setState(() => _isVegOnly = v),
              activeColor: AppColors.success,
              contentPadding: EdgeInsets.zero,
            ),

            TextFormField(
              controller: _avgCostController,
              decoration: const InputDecoration(
                labelText: 'Average Cost for Two (\u20B9)',
                prefixIcon: Icon(Icons.currency_rupee),
              ),
              keyboardType: TextInputType.number,
              inputFormatters: [FilteringTextInputFormatter.digitsOnly],
            ),
            const SizedBox(height: 24),

            // ──── Cuisine Types ────
            Text(
              'Cuisine Types',
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),

            cuisinesAsync.when(
              loading: () => const Padding(
                padding: EdgeInsets.symmetric(vertical: 8),
                child: LinearProgressIndicator(color: AppColors.primary),
              ),
              error: (e, _) => Text(
                'Failed to load cuisines',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: AppColors.error,
                ),
              ),
              data: (cuisines) => Wrap(
                spacing: 8,
                runSpacing: 4,
                children: cuisines.map((cuisine) {
                  final isSelected =
                      _selectedCuisineIds.contains(cuisine.id);
                  return FilterChip(
                    label: Text(cuisine.name),
                    selected: isSelected,
                    onSelected: (selected) {
                      setState(() {
                        if (selected) {
                          _selectedCuisineIds.add(cuisine.id);
                        } else {
                          _selectedCuisineIds.remove(cuisine.id);
                        }
                      });
                    },
                    selectedColor: AppColors.primaryLight,
                    checkmarkColor: AppColors.primary,
                  );
                }).toList(),
              ),
            ),
            const SizedBox(height: 32),

            // ──── Submit ────
            FilledButton(
              onPressed: _isSubmitting ? null : _submit,
              style: FilledButton.styleFrom(
                backgroundColor: AppColors.primary,
                minimumSize: const Size.fromHeight(52),
              ),
              child: _isSubmitting
                  ? const SizedBox(
                      height: 20,
                      width: 20,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Colors.white,
                      ),
                    )
                  : const Text('Save Changes'),
            ),
            const SizedBox(height: 24),
          ],
        ),
      ),
    );
  }
}
