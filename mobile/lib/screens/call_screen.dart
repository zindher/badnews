import 'package:flutter/material.dart';
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
