import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'package:badnews/main.dart';

void main() {
  testWidgets('BadNews app launches smoke test', (WidgetTester tester) async {
    // Build our app and trigger a frame.
    await tester.pumpWidget(const BadNewsApp());

    // Verify that the splash screen renders (app loads)
    expect(find.byType(MaterialApp), findsOneWidget);
  });
}
