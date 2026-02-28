import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/cuisine_type_model.dart';
import '../../data/repositories/restaurant_repository.dart';
import '../providers/cuisine_types_provider.dart';
import '../providers/restaurant_list_notifier.dart';

/// Form screen for registering a new restaurant.
///
/// Collects all required fields and submits to the repository. On success,
/// navigates back and invalidates the restaurant list so it reloads.
class RestaurantRegisterScreen extends ConsumerStatefulWidget {
  const RestaurantRegisterScreen({super.key});

  @override
  ConsumerState<RestaurantRegisterScreen> createState() =>
      _RestaurantRegisterScreenState();
}

class _RestaurantRegisterScreenState
    extends ConsumerState<RestaurantRegisterScreen> {
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

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isSubmitting = true);

    final data = <String, dynamic>{
      'name': _nameController.text.trim(),
      if (_descriptionController.text.trim().isNotEmpty)
        'description': _descriptionController.text.trim(),
      if (_phoneController.text.trim().isNotEmpty)
        'phoneNumber': _phoneController.text.trim(),
      if (_emailController.text.trim().isNotEmpty)
        'email': _emailController.text.trim(),
      if (_addressLine1Controller.text.trim().isNotEmpty)
        'addressLine1': _addressLine1Controller.text.trim(),
      if (_addressLine2Controller.text.trim().isNotEmpty)
        'addressLine2': _addressLine2Controller.text.trim(),
      if (_cityController.text.trim().isNotEmpty)
        'city': _cityController.text.trim(),
      if (_stateController.text.trim().isNotEmpty)
        'state': _stateController.text.trim(),
      if (_postalCodeController.text.trim().isNotEmpty)
        'postalCode': _postalCodeController.text.trim(),
      if (_latController.text.trim().isNotEmpty)
        'latitude': double.tryParse(_latController.text.trim()),
      if (_lonController.text.trim().isNotEmpty)
        'longitude': double.tryParse(_lonController.text.trim()),
      'isVegOnly': _isVegOnly,
      if (_avgCostController.text.trim().isNotEmpty)
        'avgCostForTwo': int.tryParse(_avgCostController.text.trim()),
      if (_selectedCuisineIds.isNotEmpty)
        'cuisineIds': _selectedCuisineIds.toList(),
    };

    final repository = ref.read(restaurantRepositoryProvider);
    final result = await repository.registerRestaurant(data: data);

    if (!mounted) return;
    setState(() => _isSubmitting = false);

    if (result.failure != null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(result.failure!.message),
          backgroundColor: Colors.red,
        ),
      );
    } else {
      ref.invalidate(restaurantListNotifierProvider);
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Restaurant registered successfully!'),
          backgroundColor: AppColors.success,
        ),
      );
      context.pop();
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final cuisinesAsync = ref.watch(cuisineTypesProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Register Restaurant'),
      ),
      body: SingleChildScrollView(
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
                  hintText: '+91 9876543210',
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
                      decoration: const InputDecoration(
                        labelText: 'City',
                      ),
                      textCapitalization: TextCapitalization.words,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: TextFormField(
                      controller: _stateController,
                      decoration: const InputDecoration(
                        labelText: 'State',
                      ),
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
                      decoration: const InputDecoration(
                        labelText: 'Latitude',
                      ),
                      keyboardType:
                          const TextInputType.numberWithOptions(decimal: true),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: TextFormField(
                      controller: _lonController,
                      decoration: const InputDecoration(
                        labelText: 'Longitude',
                      ),
                      keyboardType:
                          const TextInputType.numberWithOptions(decimal: true),
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
                data: (cuisines) => _CuisineSelector(
                  cuisines: cuisines,
                  selectedIds: _selectedCuisineIds,
                  onChanged: (ids) =>
                      setState(() {
                        _selectedCuisineIds
                          ..clear()
                          ..addAll(ids);
                      }),
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
                    : const Text('Register Restaurant'),
              ),
              const SizedBox(height: 24),
            ],
          ),
        ),
      ),
    );
  }
}

/// Multi-select cuisine chips.
class _CuisineSelector extends StatelessWidget {
  const _CuisineSelector({
    required this.cuisines,
    required this.selectedIds,
    required this.onChanged,
  });

  final List<CuisineTypeModel> cuisines;
  final Set<String> selectedIds;
  final ValueChanged<Set<String>> onChanged;

  @override
  Widget build(BuildContext context) {
    return Wrap(
      spacing: 8,
      runSpacing: 4,
      children: cuisines.map((cuisine) {
        final isSelected = selectedIds.contains(cuisine.id);
        return FilterChip(
          label: Text(cuisine.name),
          selected: isSelected,
          onSelected: (selected) {
            final newSet = Set<String>.from(selectedIds);
            if (selected) {
              newSet.add(cuisine.id);
            } else {
              newSet.remove(cuisine.id);
            }
            onChanged(newSet);
          },
          selectedColor: AppColors.primaryLight,
          checkmarkColor: AppColors.primary,
        );
      }).toList(),
    );
  }
}
