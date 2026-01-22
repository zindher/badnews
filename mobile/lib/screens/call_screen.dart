import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/providers.dart';

class CallScreen extends StatefulWidget {
  final String orderId;

  const CallScreen({Key? key, required this.orderId}) : super(key: key);

  @override
  State<CallScreen> createState() => _CallScreenState();
}

class _CallScreenState extends State<CallScreen> {
  late CallProvider callProvider;
  bool isMuted = false;
  bool isSpeaker = true;
  int callDuration = 0;

  @override
  void initState() {
    super.initState();
    callProvider = context.read<CallProvider>();
    callProvider.initiateCall(widget.orderId);
    
    // Update call duration every second
    Future.delayed(const Duration(seconds: 1), _updateDuration);
  }

  void _updateDuration() {
    if (mounted) {
      setState(() => callDuration++);
      Future.delayed(const Duration(seconds: 1), _updateDuration);
    }
  }

  String _formatDuration(int seconds) {
    final minutes = seconds ~/ 60;
    final secs = seconds % 60;
    return '${minutes.toString().padLeft(2, '0')}:${secs.toString().padLeft(2, '0')}';
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: Container(
          decoration: const BoxDecoration(
            gradient: LinearGradient(
              colors: [Color(0xFF667eea), Color(0xFF764ba2)],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const SizedBox(height: 40),
              const Text(
                'Llamada en curso',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 18,
                  color: Colors.white,
                  fontWeight: FontWeight.w500,
                ),
              ),
              const SizedBox(height: 24),
              Expanded(
                child: Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Container(
                        width: 100,
                        height: 100,
                        decoration: BoxDecoration(
                          shape: BoxShape.circle,
                          color: Colors.white.withOpacity(0.2),
                        ),
                        child: const Center(
                          child: Icon(
                            Icons.call,
                            size: 50,
                            color: Colors.white,
                          ),
                        ),
                      ),
                      const SizedBox(height: 24),
                      Text(
                        _formatDuration(callDuration),
                        style: const TextStyle(
                          fontSize: 48,
                          color: Colors.white,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 32),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: [
                    _CallButton(
                      icon: isMuted ? Icons.mic_off : Icons.mic,
                      backgroundColor: Colors.white.withOpacity(0.2),
                      onPressed: () {
                        setState(() => isMuted = !isMuted);
                      },
                    ),
                    _CallButton(
                      icon: isSpeaker ? Icons.volume_up : Icons.volume_mute,
                      backgroundColor: Colors.white.withOpacity(0.2),
                      onPressed: () {
                        setState(() => isSpeaker = !isSpeaker);
                      },
                    ),
                    _CallButton(
                      icon: Icons.call_end,
                      backgroundColor: Colors.red,
                      onPressed: () {
                        callProvider.endCall(widget.orderId);
                        Navigator.of(context).pop();
                      },
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 40),
            ],
          ),
        ),
      ),
    );
  }

  @override
  void dispose() {
    callProvider.endCall(widget.orderId);
    super.dispose();
  }
}

class _CallButton extends StatelessWidget {
  final IconData icon;
  final Color backgroundColor;
  final VoidCallback onPressed;

  const _CallButton({
    required this.icon,
    required this.backgroundColor,
    required this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onPressed,
      child: Container(
        width: 60,
        height: 60,
        decoration: BoxDecoration(
          shape: BoxShape.circle,
          color: backgroundColor,
        ),
        child: Center(
          child: Icon(
            icon,
            color: Colors.white,
            size: 28,
          ),
        ),
      ),
    );
  }
}
import 'package:provider/provider.dart';
import '../providers/providers.dart';

class CallScreen extends StatefulWidget {
  final dynamic order;

  const CallScreen({Key? key, required this.order}) : super(key: key);

  @override
  State<CallScreen> createState() => _CallScreenState();
}

class _CallScreenState extends State<CallScreen> {
  bool _isRecording = false;
  int _callDuration = 0;

  @override
  void initState() {
    super.initState();
    _startRecording();
  }

  void _startRecording() {
    // TODO: Implement recording
    setState(() => _isRecording = true);
  }

  void _stopRecording() {
    // TODO: Stop recording and upload
    setState(() => _isRecording = false);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Llamada en Progreso'),
        backgroundColor: Colors.purple,
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              padding: const EdgeInsets.all(24),
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                color: _isRecording ? Colors.red : Colors.green,
              ),
              child: Text(
                widget.order.recipientName,
                style: const TextStyle(color: Colors.white, fontSize: 24, fontWeight: FontWeight.bold),
              ),
            ),
            const SizedBox(height: 32),
            Text('Teléfono: ${widget.order.recipientPhone}',
              style: const TextStyle(fontSize: 16)),
            const SizedBox(height: 16),
            Text('Duración: $_callDuration segundos',
              style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
            const SizedBox(height: 32),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                ElevatedButton.icon(
                  onPressed: _isRecording ? null : _startRecording,
                  icon: const Icon(Icons.mic),
                  label: const Text('Grabar'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.green,
                  ),
                ),
                ElevatedButton.icon(
                  onPressed: _isRecording ? _stopRecording : null,
                  icon: const Icon(Icons.stop),
                  label: const Text('Detener'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.red,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
