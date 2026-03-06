import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/models.dart';

class ApiService {
  static const String baseUrl = 'http://10.0.2.2:5000/api';
  late Dio _dio;

  ApiService() {
    _dio = Dio(
      BaseOptions(
        baseUrl: baseUrl,
        connectTimeout: const Duration(seconds: 10),
        receiveTimeout: const Duration(seconds: 10),
      ),
    );

    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final prefs = await SharedPreferences.getInstance();
          final token = prefs.getString('auth_token');
          if (token != null) {
            options.headers['Authorization'] = 'Bearer $token';
          }
          handler.next(options);
        },
      ),
    );
  }

  Future<Map<String, dynamic>> login(String email, String password) async {
    try {
      final response = await _dio.post('/auth/login', data: {
        'email': email,
        'password': password,
      });
      return response.data as Map<String, dynamic>;
    } on DioException catch (e) {
      throw Exception('Error en login: ${e.message}');
    }
  }

  Future<UserModel> getUserProfile() async {
    try {
      final response = await _dio.get('/users/me');
      return UserModel.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      throw Exception('Error fetching profile: ${e.message}');
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

  Future<Map<String, dynamic>> getEarnings() async {
    try {
      final response = await _dio.get('/messengers/me/earnings');
      return response.data as Map<String, dynamic>;
    } on DioException catch (e) {
      throw Exception('Error fetching earnings: ${e.message}');
    }
  }

  Future<void> requestWithdrawal(double amount) async {
    try {
      await _dio.post('/withdrawals', data: {'amount': amount});
    } on DioException catch (e) {
      throw Exception('Error requesting withdrawal: ${e.message}');
    }
  }

  Future<void> initiateCall(String orderId) async {
    try {
      await _dio.post('/calls/$orderId/initiate', data: {});
    } on DioException catch (e) {
      throw Exception('Error initiating call: ${e.message}');
    }
  }

  Future<void> endCall(String orderId) async {
    try {
      await _dio.post('/calls/$orderId/end', data: {});
    } on DioException catch (e) {
      throw Exception('Error ending call: ${e.message}');
    }
  }

  Future<RecordingModel> getRecording(String callId) async {
    try {
      final response = await _dio.get('/calls/$callId/recording');
      return RecordingModel.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      throw Exception('Error fetching recording: ${e.message}');
    }
  }

  Future<void> downloadRecording(String callId) async {
    try {
      await _dio.get('/calls/$callId/recording/download');
    } on DioException catch (e) {
      throw Exception('Error downloading recording: ${e.message}');
    }
  }

  Future<List<ChatMessage>> getChatMessages(String orderId) async {
    try {
      final response = await _dio.get('/orders/$orderId/messages');
      final messages = (response.data as List)
          .map((m) => ChatMessage.fromJson(m))
          .toList();
      return messages;
    } on DioException catch (e) {
      throw Exception('Error fetching messages: ${e.message}');
    }
  }

  Future<ChatMessage> sendChatMessage(String orderId, String content) async {
    try {
      final response = await _dio.post(
        '/orders/$orderId/messages',
        data: {'content': content},
      );
      return ChatMessage.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      throw Exception('Error sending message: ${e.message}');
    }
  }
}
