import 'package:flutter/material.dart';
import '../services/auth_service.dart';
import 'package:shared_preferences/shared_preferences.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({Key? key}) : super(key: key);

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  bool _isLoading = false;
  final AuthService _authService = AuthService();

  Future<void> _handleGoogleSignIn() async {
    setState(() => _isLoading = true);
    try {
      final result = await _authService.signInWithGoogle();

      if (!mounted) return;

      if (result.isSuccess) {
        await _authService.saveAuthData(result.token!, result.user!);
        Navigator.of(context).pushReplacementNamed('/home');
      } else if (!result.isCancelled) {
        _showError(result.errorMessage ?? 'Error al iniciar sesión con Google');
      }
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _handleAppleSignIn() async {
    setState(() => _isLoading = true);
    try {
      final result = await _authService.signInWithApple();

      if (!mounted) return;

      if (result.isSuccess) {
        await _authService.saveAuthData(result.token!, result.user!);
        Navigator.of(context).pushReplacementNamed('/home');
      } else if (!result.isCancelled) {
        _showError(result.errorMessage ?? 'Error al iniciar sesión con Apple');
      }
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  void _showError(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red.shade600,
        duration: const Duration(seconds: 3),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [
              Colors.purple.shade700,
              Colors.purple.shade900,
            ],
          ),
        ),
        child: SafeArea(
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 32.0),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                // Logo
                Container(
                  width: 100,
                  height: 100,
                  decoration: BoxDecoration(
                    color: Colors.white,
                    shape: BoxShape.circle,
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withOpacity(0.2),
                        blurRadius: 15,
                        spreadRadius: 3,
                      ),
                    ],
                  ),
                  child: Icon(
                    Icons.mail_outline,
                    size: 60,
                    color: Colors.purple.shade700,
                  ),
                ),
                const SizedBox(height: 24),
                Text(
                  'BadNews',
                  style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                        color: Colors.white,
                        fontWeight: FontWeight.bold,
                        fontSize: 36,
                      ),
                ),
                const SizedBox(height: 8),
                Text(
                  'Ingresa a tu cuenta',
                  style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                        color: Colors.white70,
                        fontSize: 16,
                      ),
                ),
                const SizedBox(height: 48),

                if (_isLoading)
                  CircularProgressIndicator(
                    valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                  )
                else ...[
                  // Google Sign-In button
                  _SocialSignInButton(
                    onPressed: _handleGoogleSignIn,
                    label: 'Continuar con Google',
                    icon: _GoogleIcon(),
                    backgroundColor: Colors.white,
                    textColor: Colors.black87,
                  ),
                  const SizedBox(height: 16),

                  // Divider
                  Row(
                    children: [
                      Expanded(child: Divider(color: Colors.white38)),
                      Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 12),
                        child: Text('O',
                            style: TextStyle(
                                color: Colors.white70, fontSize: 14)),
                      ),
                      Expanded(child: Divider(color: Colors.white38)),
                    ],
                  ),
                  const SizedBox(height: 16),

                  // Apple Sign-In button
                  _SocialSignInButton(
                    onPressed: _handleAppleSignIn,
                    label: 'Continuar con Apple',
                    icon: const Icon(Icons.apple, color: Colors.white, size: 24),
                    backgroundColor: Colors.black,
                    textColor: Colors.white,
                  ),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }
}

// ---------------------------------------------------------------------------
// Reusable social sign-in button widget
// ---------------------------------------------------------------------------

class _SocialSignInButton extends StatelessWidget {
  final VoidCallback onPressed;
  final String label;
  final Widget icon;
  final Color backgroundColor;
  final Color textColor;

  const _SocialSignInButton({
    required this.onPressed,
    required this.label,
    required this.icon,
    required this.backgroundColor,
    required this.textColor,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: double.infinity,
      height: 54,
      child: ElevatedButton(
        onPressed: onPressed,
        style: ElevatedButton.styleFrom(
          backgroundColor: backgroundColor,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          elevation: 4,
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            icon,
            const SizedBox(width: 12),
            Text(
              label,
              style: TextStyle(
                color: textColor,
                fontSize: 16,
                fontWeight: FontWeight.w600,
                letterSpacing: 0.3,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// ---------------------------------------------------------------------------
// Google "G" icon (SVG-like using CustomPaint)
// ---------------------------------------------------------------------------

class _GoogleIcon extends StatelessWidget {
  const _GoogleIcon();

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 24,
      height: 24,
      child: CustomPaint(painter: _GoogleIconPainter()),
    );
  }
}

class _GoogleIconPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final double cx = size.width / 2;
    final double cy = size.height / 2;
    final double r = size.width / 2;

    // Blue segment
    final paintBlue = Paint()..color = const Color(0xFF4285F4);
    // Red segment
    final paintRed = Paint()..color = const Color(0xFFEA4335);
    // Yellow segment
    final paintYellow = Paint()..color = const Color(0xFFFBBC05);
    // Green segment
    final paintGreen = Paint()..color = const Color(0xFF34A853);

    canvas.drawArc(
        Rect.fromCircle(center: Offset(cx, cy), radius: r),
        -1.57,
        3.14,
        true,
        paintBlue);
    canvas.drawArc(
        Rect.fromCircle(center: Offset(cx, cy), radius: r),
        1.57,
        1.57,
        true,
        paintRed);
    canvas.drawArc(
        Rect.fromCircle(center: Offset(cx, cy), radius: r),
        0,
        1.57,
        true,
        paintYellow);
    canvas.drawArc(
        Rect.fromCircle(center: Offset(cx, cy), radius: r),
        -1.57,
        -1.57,
        true,
        paintGreen);

    // White inner circle
    canvas.drawCircle(
        Offset(cx, cy), r * 0.62, Paint()..color = Colors.white);

    // Right bar for "G"
    canvas.drawRect(
      Rect.fromLTWH(cx, cy - r * 0.2, r * 0.9, r * 0.4),
      paintBlue,
    );
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

