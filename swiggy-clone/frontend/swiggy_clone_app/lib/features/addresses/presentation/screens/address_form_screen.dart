import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:geolocator/geolocator.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/address_model.dart';
import '../providers/address_form_notifier.dart';
import '../providers/address_form_state.dart';
import '../providers/address_list_notifier.dart';
import '../providers/address_list_state.dart';

class AddressFormScreen extends ConsumerStatefulWidget {
  const AddressFormScreen({super.key, this.addressId});

  final String? addressId;

  @override
  ConsumerState<AddressFormScreen> createState() => _AddressFormScreenState();
}

class _AddressFormScreenState extends ConsumerState<AddressFormScreen> {
  final _formKey = GlobalKey<FormState>();
  final _labelController = TextEditingController();
  final _line1Controller = TextEditingController();
  final _line2Controller = TextEditingController();
  final _cityController = TextEditingController();
  final _stateController = TextEditingController();
  final _postalCodeController = TextEditingController();
  final _countryController = TextEditingController(text: 'India');

  LatLng _selectedLocation = const LatLng(12.9716, 77.5946); // Default: Bengaluru
  GoogleMapController? _mapController;
  bool _isLoadingLocation = false;

  bool get _isEditing => widget.addressId != null;

  @override
  void initState() {
    super.initState();
    if (_isEditing) {
      _loadExistingAddress();
    }
  }

  void _loadExistingAddress() {
    final listState = ref.read(addressListNotifierProvider);
    if (listState is AddressListLoaded) {
      final existing = listState.addresses.cast<AddressModel?>().firstWhere(
            (a) => a!.id == widget.addressId,
            orElse: () => null,
          );
      if (existing != null) {
        _labelController.text = existing.label;
        _line1Controller.text = existing.addressLine1;
        _line2Controller.text = existing.addressLine2 ?? '';
        _cityController.text = existing.city ?? '';
        _stateController.text = existing.state ?? '';
        _postalCodeController.text = existing.postalCode ?? '';
        _countryController.text = existing.country ?? 'India';
        _selectedLocation = LatLng(existing.latitude, existing.longitude);
      }
    }
  }

  @override
  void dispose() {
    _labelController.dispose();
    _line1Controller.dispose();
    _line2Controller.dispose();
    _cityController.dispose();
    _stateController.dispose();
    _postalCodeController.dispose();
    _countryController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final formState = ref.watch(addressFormNotifierProvider);
    final theme = Theme.of(context);

    ref.listen<AddressFormState>(addressFormNotifierProvider, (prev, next) {
      if (next is AddressFormSaved) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(_isEditing ? 'Address updated' : 'Address saved'),
          ),
        );
        Navigator.of(context).pop();
      } else if (next is AddressFormError) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(next.failure.message)),
        );
      }
    });

    return Scaffold(
      appBar: AppBar(
        title: Text(_isEditing ? 'Edit Address' : 'Add Address'),
      ),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            // Map section
            ClipRRect(
              borderRadius: BorderRadius.circular(12),
              child: SizedBox(
                height: 250,
                child: Stack(
                  children: [
                    GoogleMap(
                      initialCameraPosition: CameraPosition(
                        target: _selectedLocation,
                        zoom: 15,
                      ),
                      onMapCreated: (controller) {
                        _mapController = controller;
                      },
                      onCameraMove: (position) {
                        setState(() {
                          _selectedLocation = position.target;
                        });
                      },
                      myLocationEnabled: true,
                      myLocationButtonEnabled: false,
                      zoomControlsEnabled: false,
                      mapToolbarEnabled: false,
                    ),
                    // Center pin overlay
                    const Center(
                      child: Padding(
                        padding: EdgeInsets.only(bottom: 36),
                        child: Icon(
                          Icons.location_pin,
                          size: 48,
                          color: AppColors.primary,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 8),
            Align(
              alignment: Alignment.centerLeft,
              child: TextButton.icon(
                onPressed: _isLoadingLocation ? null : _useCurrentLocation,
                icon: _isLoadingLocation
                    ? const SizedBox(
                        width: 16,
                        height: 16,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.my_location, size: 18),
                label: const Text('Use Current Location'),
              ),
            ),
            const SizedBox(height: 16),

            // Label with quick-select chips
            Text('Label', style: theme.textTheme.titleSmall),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              children: ['Home', 'Work', 'Other'].map((label) {
                final isSelected =
                    _labelController.text.toLowerCase() == label.toLowerCase();
                return ChoiceChip(
                  label: Text(label),
                  selected: isSelected,
                  onSelected: (_) {
                    setState(() => _labelController.text = label);
                  },
                  selectedColor: AppColors.primary.withValues(alpha: 0.15),
                );
              }).toList(),
            ),
            const SizedBox(height: 8),
            TextFormField(
              controller: _labelController,
              decoration: const InputDecoration(
                hintText: 'e.g., Home, Work, Mom\'s Place',
                border: OutlineInputBorder(),
              ),
              validator: (v) =>
                  (v == null || v.trim().isEmpty) ? 'Label is required' : null,
            ),
            const SizedBox(height: 16),

            // Address Line 1
            TextFormField(
              controller: _line1Controller,
              decoration: const InputDecoration(
                labelText: 'Address Line 1',
                hintText: 'House/Flat no., Street name',
                border: OutlineInputBorder(),
              ),
              validator: (v) => (v == null || v.trim().isEmpty)
                  ? 'Address line 1 is required'
                  : null,
            ),
            const SizedBox(height: 12),

            // Address Line 2
            TextFormField(
              controller: _line2Controller,
              decoration: const InputDecoration(
                labelText: 'Address Line 2 (Optional)',
                hintText: 'Landmark, Floor, Building',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 12),

            // City + State
            Row(
              children: [
                Expanded(
                  child: TextFormField(
                    controller: _cityController,
                    decoration: const InputDecoration(
                      labelText: 'City',
                      border: OutlineInputBorder(),
                    ),
                    validator: (v) => (v == null || v.trim().isEmpty)
                        ? 'City is required'
                        : null,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: TextFormField(
                    controller: _stateController,
                    decoration: const InputDecoration(
                      labelText: 'State',
                      border: OutlineInputBorder(),
                    ),
                    validator: (v) => (v == null || v.trim().isEmpty)
                        ? 'State is required'
                        : null,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),

            // Postal Code + Country
            Row(
              children: [
                Expanded(
                  child: TextFormField(
                    controller: _postalCodeController,
                    decoration: const InputDecoration(
                      labelText: 'Postal Code',
                      border: OutlineInputBorder(),
                    ),
                    keyboardType: TextInputType.number,
                    validator: (v) => (v == null || v.trim().isEmpty)
                        ? 'Postal code is required'
                        : null,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: TextFormField(
                    controller: _countryController,
                    decoration: const InputDecoration(
                      labelText: 'Country',
                      border: OutlineInputBorder(),
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 24),

            // Save button
            FilledButton(
              onPressed: formState is AddressFormSaving ? null : _save,
              style: FilledButton.styleFrom(
                backgroundColor: AppColors.primary,
                minimumSize: const Size.fromHeight(52),
              ),
              child: formState is AddressFormSaving
                  ? const SizedBox(
                      width: 24,
                      height: 24,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Colors.white,
                      ),
                    )
                  : Text(
                      _isEditing ? 'Update Address' : 'Save Address',
                      style: const TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 16,
                      ),
                    ),
            ),
          ],
        ),
      ),
    );
  }

  void _save() {
    if (!_formKey.currentState!.validate()) return;

    ref.read(addressFormNotifierProvider.notifier).saveAddress(
          addressId: widget.addressId,
          label: _labelController.text.trim(),
          addressLine1: _line1Controller.text.trim(),
          addressLine2: _line2Controller.text.trim().isEmpty
              ? null
              : _line2Controller.text.trim(),
          city: _cityController.text.trim(),
          stateProvince: _stateController.text.trim(),
          postalCode: _postalCodeController.text.trim(),
          country: _countryController.text.trim().isEmpty
              ? null
              : _countryController.text.trim(),
          latitude: _selectedLocation.latitude,
          longitude: _selectedLocation.longitude,
        );
  }

  Future<void> _useCurrentLocation() async {
    setState(() => _isLoadingLocation = true);
    try {
      final permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        final requested = await Geolocator.requestPermission();
        if (requested == LocationPermission.denied ||
            requested == LocationPermission.deniedForever) {
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('Location permission denied')),
            );
          }
          return;
        }
      }

      final position = await Geolocator.getCurrentPosition(
        locationSettings: const LocationSettings(
          accuracy: LocationAccuracy.high,
        ),
      );

      final newLocation = LatLng(position.latitude, position.longitude);
      setState(() => _selectedLocation = newLocation);
      _mapController?.animateCamera(
        CameraUpdate.newLatLngZoom(newLocation, 16),
      );
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Could not get current location')),
        );
      }
    } finally {
      if (mounted) setState(() => _isLoadingLocation = false);
    }
  }
}
