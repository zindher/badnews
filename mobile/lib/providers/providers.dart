import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/models.dart';
import '../services/api_service.dart';

class AuthProvider extends ChangeNotifier {
  final ApiService _apiService = ApiService();

  UserModel? _user;
  bool _isLoading = false;
  String? _error;

  UserModel? get user => _user;
  bool get isLoading => _isLoading;
  String? get error => _error;

  Future<void> loadUserProfile() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString('auth_token');
      if (token != null) {
        _user = await _apiService.getUserProfile();
      }
    } catch (e) {
      _error = e.toString();
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> logout() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.clear();
    _user = null;
    notifyListeners();
  }
}

class CallProvider extends ChangeNotifier {
  final ApiService _apiService = ApiService();

  bool _isLoading = false;
  RecordingModel? _currentRecording;
  String? _error;

  bool get isLoading => _isLoading;
  RecordingModel? get currentRecording => _currentRecording;
  String? get error => _error;

  Future<void> initiateCall(String orderId) async {
    _error = null;
    notifyListeners();
    try {
      await _apiService.initiateCall(orderId);
    } catch (e) {
      _error = e.toString();
      notifyListeners();
    }
  }

  Future<void> endCall(String orderId) async {
    _error = null;
    try {
      await _apiService.endCall(orderId);
    } catch (e) {
      _error = e.toString();
    }
    notifyListeners();
  }

  Future<void> fetchRecording(String callId) async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      _currentRecording = await _apiService.getRecording(callId);
    } catch (e) {
      _error = e.toString();
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> downloadRecording(String callId) async {
    _error = null;
    try {
      await _apiService.downloadRecording(callId);
    } catch (e) {
      _error = e.toString();
      notifyListeners();
    }
  }
}

class ChatProvider extends ChangeNotifier {
  final ApiService _apiService = ApiService();

  List<ChatMessage> _messages = [];
  bool _isLoading = false;
  String? _currentUserId;
  String? _error;

  List<ChatMessage> get messages => _messages;
  bool get isLoading => _isLoading;
  String? get currentUserId => _currentUserId;
  String? get error => _error;

  ChatProvider() {
    _loadCurrentUserId();
  }

  Future<void> _loadCurrentUserId() async {
    final prefs = await SharedPreferences.getInstance();
    _currentUserId = prefs.getString('user_id');
    notifyListeners();
  }

  Future<void> loadMessages(String orderId) async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      _messages = await _apiService.getChatMessages(orderId);
    } catch (e) {
      _error = e.toString();
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> sendMessage(String orderId, String content) async {
    _error = null;
    try {
      final message = await _apiService.sendChatMessage(orderId, content);
      _messages.add(message);
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      notifyListeners();
    }
  }
}

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
  double _totalEarnings = 0.0;
  double _pendingEarnings = 0.0;
  int _completedOrders = 0;
  List<TransactionModel> _transactions = [];
  bool _isLoading = false;
  String? _error;

  MessengerModel? get messenger => _messenger;
  bool get isAvailable => _isAvailable;
  double get totalEarnings => _totalEarnings;
  double get pendingEarnings => _pendingEarnings;
  int get completedOrders => _completedOrders;
  List<TransactionModel> get transactions => _transactions;
  bool get isLoading => _isLoading;
  String? get error => _error;

  Future<void> fetchEarnings() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      final data = await _apiService.getEarnings();
      _totalEarnings = (data['totalEarnings'] as num?)?.toDouble() ?? 0.0;
      _pendingEarnings = (data['pendingBalance'] as num?)?.toDouble() ?? 0.0;
      _completedOrders = data['totalCompletedOrders'] ?? 0;
      final rawTransactions = data['transactions'] as List<dynamic>? ?? [];
      _transactions = rawTransactions
          .map((t) => TransactionModel.fromJson(t as Map<String, dynamic>))
          .toList();
    } catch (e) {
      _error = e.toString();
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> requestWithdraw() async {
    _error = null;
    try {
      await _apiService.requestWithdrawal(_pendingEarnings);
      _pendingEarnings = 0.0;
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      notifyListeners();
    }
  }

  Future<void> updateAvailability(bool available) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final messengerId = prefs.getString('messenger_id') ?? '';
      await _apiService.updateMessengerAvailability(messengerId, available);
      _isAvailable = available;
      notifyListeners();
    } catch (e) {
      _error = e.toString();
      notifyListeners();
    }
  }
}
