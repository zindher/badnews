import 'package:record/record.dart';
import 'package:path_provider/path_provider.dart';
import 'dart:io';

class RecordingService {
  final Record _record = Record();
  String? _recordingPath;

  Future<void> startRecording(String orderId) async {
    try {
      if (await _record.hasPermission()) {
        final directory = await getApplicationDocumentsDirectory();
        _recordingPath = '${directory.path}/order_$orderId.m4a';
        
        await _record.start(
          path: _recordingPath!,
          encoder: AudioEncoder.aacLc,
        );
      }
    } catch (e) {
      throw Exception('Error starting recording: $e');
    }
  }

  Future<String?> stopRecording() async {
    try {
      final path = await _record.stop();
      return path;
    } catch (e) {
      throw Exception('Error stopping recording: $e');
    }
  }

  Future<bool> hasRecordingPermission() async {
    return await _record.hasPermission();
  }

  Future<void> requestRecordingPermission() async {
    await _record.hasPermission();
  }
}
