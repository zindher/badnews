import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../models/models.dart';
import '../services/api_service.dart';

class OrderProvider extends ChangeNotifier {
  final ApiService _apiService = ApiService();

  List<OrderModel> _availableOrders = [];
  OrderModel? _currentOrder;
  bool _isLoading = false;
  String? _error;

  List<OrderModel> get availableOrders => _availableOrders;
  OrderModel? get currentOrder => _currentOrder;
  bool get isLoading => _isLoading;
  String? get error => _error;

  Future<void> fetchAvailableOrders() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      _availableOrders = await _apiService.getAvailableOrders();
    } catch (e) {
      _error = e.toString();
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> acceptOrder(String orderId, String messengerId) async {
    try {
      await _apiService.acceptOrder(orderId, messengerId);
      _currentOrder = _availableOrders.firstWhere((o) => o.id == orderId);
      _availableOrders.removeWhere((o) => o.id == orderId);
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      notifyListeners();
    }
  }

  Future<void> completeOrder(String recordingUrl) async {
    if (_currentOrder == null) return;

    try {
      await _apiService.completeOrder(_currentOrder!.id, recordingUrl);
      _currentOrder = null;
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      notifyListeners();
    }
  }
}

class MessengerProvider extends ChangeNotifier {
  final ApiService _apiService = ApiService();

  MessengerModel? _messenger;
  bool _isAvailable = false;

  MessengerModel? get messenger => _messenger;
  bool get isAvailable => _isAvailable;

  Future<void> setAvailability(String messengerId, bool available) async {
    try {
      await _apiService.updateMessengerAvailability(messengerId, available);
      _isAvailable = available;
      notifyListeners();
    } catch (e) {
      rethrow;
    }
  }
}
