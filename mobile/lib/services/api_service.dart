import 'package:dio/dio.dart';
import '../models/models.dart';

class ApiService {
  static const String baseUrl = 'http://localhost:5000/api';
  late Dio _dio;

  ApiService() {
    _dio = Dio(
      BaseOptions(
        baseUrl: baseUrl,
        connectTimeout: const Duration(seconds: 10),
        receiveTimeout: const Duration(seconds: 10),
      ),
    );
  }

  Future<Map<String, dynamic>> login(String email, String password) async {
    try {
      // TODO: Replace with actual backend endpoint
      // final response = await _dio.post('/auth/login', data: {
      //   'email': email,
      //   'password': password,
      // });
      // return response.data;
      
      // For now, return mock response
      return {
        'token': 'mock_token_${DateTime.now().millisecondsSinceEpoch}',
        'userId': 'user_${DateTime.now().millisecondsSinceEpoch}',
        'name': email.split('@')[0],
        'email': email,
      };
    } on DioException catch (e) {
      throw Exception('Error en login: ${e.message}');
    }
  }

  Future<List<OrderModel>> getAvailableOrders() async {
    try {
      final response = await _dio.get('/orders/available');
      final orders = (response.data as List)
          .map((order) => OrderModel.fromJson(order))
          .toList();
      return orders;
    } on DioException catch (e) {
      throw Exception('Error fetching orders: ${e.message}');
    }
  }

  Future<void> acceptOrder(String orderId, String messengerId) async {
    try {
      await _dio.put(
        '/orders/$orderId/assign',
        data: {'messengerId': messengerId},
      );
    } on DioException catch (e) {
      throw Exception('Error accepting order: ${e.message}');
    }
  }

  Future<void> completeOrder(String orderId, String recordingUrl) async {
    try {
      await _dio.put(
        '/orders/$orderId/complete',
        data: {'recordingUrl': recordingUrl},
      );
    } on DioException catch (e) {
      throw Exception('Error completing order: ${e.message}');
    }
  }

  Future<void> updateMessengerAvailability(String messengerId, bool isAvailable) async {
    try {
      await _dio.put(
        '/messengers/$messengerId/availability',
        data: {'isAvailable': isAvailable},
      );
    } on DioException catch (e) {
      throw Exception('Error updating availability: ${e.message}');
    }
  }
}
