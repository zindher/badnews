import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/providers.dart';

class RecordingScreen extends StatefulWidget {
  final String callId;

  const RecordingScreen({Key? key, required this.callId}) : super(key: key);

  @override
  State<RecordingScreen> createState() => _RecordingScreenState();
}

class _RecordingScreenState extends State<RecordingScreen> {
  late CallProvider callProvider;
  bool isPlaying = false;

  @override
  void initState() {
    super.initState();
    callProvider = context.read<CallProvider>();
    Future.microtask(() {
      callProvider.fetchRecording(widget.callId);
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Grabación de Llamada'),
        backgroundColor: const Color(0xFF667eea),
      ),
      body: Consumer<CallProvider>(
        builder: (context, provider, _) {
          if (provider.isLoading) {
            return const Center(child: CircularProgressIndicator());
          }

          if (provider.currentRecording == null) {
            return const Center(
              child: Text('No se encontró la grabación'),
            );
          }

          return SingleChildScrollView(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  // Video player placeholder
                  Container(
                    height: 250,
                    decoration: BoxDecoration(
                      color: Colors.black,
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Center(
                      child: InkWell(
                        onTap: () {
                          setState(() => isPlaying = !isPlaying);
                        },
                        child: Container(
                          width: 70,
                          height: 70,
                          decoration: BoxDecoration(
                            shape: BoxShape.circle,
                            color: Colors.white.withOpacity(0.3),
                          ),
                          child: Center(
                            child: Icon(
                              isPlaying ? Icons.pause : Icons.play_arrow,
                              color: Colors.white,
                              size: 40,
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 24),
                  
                  // Recording details
                  Card(
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'Detalles de la Grabación',
                            style: TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          const SizedBox(height: 16),
                          _DetailRow(
                            label: 'ID de Llamada',
                            value: widget.callId,
                          ),
                          _DetailRow(
                            label: 'Duración',
                            value: '${provider.currentRecording?.duration ?? 0} minutos',
                          ),
                          _DetailRow(
                            label: 'Fecha',
                            value: provider.currentRecording?.createdAt ?? 'N/A',
                          ),
                          _DetailRow(
                            label: 'Mensajero',
                            value: provider.currentRecording?.messengerName ?? 'N/A',
                          ),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(height: 24),
                  
                  // Action buttons
                  ElevatedButton.icon(
                    onPressed: () {
                      provider.downloadRecording(widget.callId);
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(content: Text('Descargando grabación...')),
                      );
                    },
                    icon: const Icon(Icons.download),
                    label: const Text('Descargar Grabación'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: const Color(0xFF667eea),
                      padding: const EdgeInsets.symmetric(vertical: 12),
                    ),
                  ),
                  const SizedBox(height: 12),
                  OutlinedButton.icon(
                    onPressed: () {
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(content: Text('Grabación compartida')),
                      );
                    },
                    icon: const Icon(Icons.share),
                    label: const Text('Compartir Grabación'),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}

class _DetailRow extends StatelessWidget {
  final String label;
  final String value;

  const _DetailRow({
    required this.label,
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: const TextStyle(
            color: Color(0xFF999999),
            fontSize: 12,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          value,
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w500,
          ),
        ),
        const SizedBox(height: 16),
      ],
    );
  }
}
