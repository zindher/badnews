import 'package:dio/dio.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:sign_in_with_apple/sign_in_with_apple.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'dart:convert';

/// Authentication service handling Google and Apple sign-in for the mobile app.
class AuthService {
  static const String _baseUrl = 'http://localhost:5000/api';
  static const String _tokenKey = 'auth_token';
  static const String _userKey = 'user_data';

  final Dio _dio;
  final GoogleSignIn _googleSignIn;

  AuthService()
      : _dio = Dio(BaseOptions(
          baseUrl: _baseUrl,
          connectTimeout: const Duration(seconds: 15),
          receiveTimeout: const Duration(seconds: 15),
        )),
        _googleSignIn = GoogleSignIn(scopes: ['email', 'profile']);

  // ---------------------------------------------------------------------------
  // Google Sign-In
  // ---------------------------------------------------------------------------

  /// Sign in with Google and authenticate against the backend.
  Future<AuthResult> signInWithGoogle() async {
    try {
      final googleUser = await _googleSignIn.signIn();
      if (googleUser == null) {
        return AuthResult.cancelled();
      }

      final googleAuth = await googleUser.authentication;
      final idToken = googleAuth.idToken;

      if (idToken == null) {
        return AuthResult.error('No se pudo obtener el token de Google');
      }

      final response = await _dio.post('/auth/google-login', data: {
        'googleToken': idToken,
      });

      return _handleAuthResponse(response);
    } on DioException catch (e) {
      return AuthResult.error(
          e.response?.data?['message'] ?? 'Error al iniciar sesión con Google');
    } catch (e) {
      return AuthResult.error('Error inesperado: $e');
    }
  }

  // ---------------------------------------------------------------------------
  // Apple Sign-In
  // ---------------------------------------------------------------------------

  /// Sign in with Apple and authenticate against the backend.
  Future<AuthResult> signInWithApple() async {
    try {
      final credential = await SignInWithApple.getAppleIDCredential(
        scopes: [
          AppleIDAuthorizationScopes.email,
          AppleIDAuthorizationScopes.fullName,
        ],
      );

      final identityToken = credential.identityToken;
      if (identityToken == null) {
        return AuthResult.error('No se pudo obtener el token de Apple');
      }

      final response = await _dio.post('/auth/apple-login', data: {
        'identityToken': identityToken,
        if (credential.givenName != null) 'firstName': credential.givenName,
        if (credential.familyName != null) 'lastName': credential.familyName,
      });

      return _handleAuthResponse(response);
    } on SignInWithAppleAuthorizationException catch (e) {
      if (e.code == AuthorizationErrorCode.canceled) {
        return AuthResult.cancelled();
      }
      return AuthResult.error('Error de Apple Sign-In: ${e.message}');
    } on DioException catch (e) {
      return AuthResult.error(
          e.response?.data?['message'] ?? 'Error al iniciar sesión con Apple');
    } catch (e) {
      return AuthResult.error('Error inesperado: $e');
    }
  }

  // ---------------------------------------------------------------------------
  // Session management
  // ---------------------------------------------------------------------------

  /// Persist auth data locally.
  Future<void> saveAuthData(String token, Map<String, dynamic> user) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_tokenKey, token);
    await prefs.setString(_userKey, _encodeUser(user));
  }

  /// Load token from local storage.
  Future<String?> getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_tokenKey);
  }

  /// Clear stored session.
  Future<void> signOut() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_tokenKey);
    await prefs.remove(_userKey);
    await _googleSignIn.signOut();
  }

  // ---------------------------------------------------------------------------
  // Helpers
  // ---------------------------------------------------------------------------

  AuthResult _handleAuthResponse(Response response) {
    final data = response.data;
    if (data['success'] == true && data['data'] != null) {
      final token = data['data']['token'] as String;
      final user = Map<String, dynamic>.from(data['data']['user'] as Map);
      return AuthResult.success(token: token, user: user);
    }
    return AuthResult.error(data['message'] ?? 'Error de autenticación');
  }

  String _encodeUser(Map<String, dynamic> user) {
    return jsonEncode(user);
  }
}

// ---------------------------------------------------------------------------
// Result class
// ---------------------------------------------------------------------------

enum AuthResultStatus { success, error, cancelled }

class AuthResult {
  final AuthResultStatus status;
  final String? token;
  final Map<String, dynamic>? user;
  final String? errorMessage;

  const AuthResult._({
    required this.status,
    this.token,
    this.user,
    this.errorMessage,
  });

  factory AuthResult.success({
    required String token,
    required Map<String, dynamic> user,
  }) =>
      AuthResult._(status: AuthResultStatus.success, token: token, user: user);

  factory AuthResult.error(String message) =>
      AuthResult._(status: AuthResultStatus.error, errorMessage: message);

  factory AuthResult.cancelled() =>
      AuthResult._(status: AuthResultStatus.cancelled);

  bool get isSuccess => status == AuthResultStatus.success;
  bool get isCancelled => status == AuthResultStatus.cancelled;
}
